# Feature: backend-version-final

## Objective
Bring the IBUY backend to its final (demo) version following the existing structure: depots with a responsible user, need-driven purchase orders, simple login, quotations and stock movements via delivery notes (remitos).

## Problem / Why
- Stock currently carries `CantidadMinima`; the purchase-order flow must be driven by depot needs instead of stock minimums.
- A depot (e.g. Deposito 1) opens a need to another depot (e.g. Central) with product, quantity and required date. The destination depot reviews it; if it lacks stock, a purchase order (NotaPedido) is created **manually** from that need.
- There is no login; users must know their company and the depots they are in charge of.
- Supplier companies must be able to quote purchase orders (Cotizacion), and stock must move via remitos (entrada / salida / transferencia).

## Decisions (user-confirmed)
- Nothing is automatic: need review only reports stock (`GET /api/necesidad/{id}/revision`); the transfer remito or the NotaPedido are created by hand. NotaPedido links to the need via `NecesidadId`.
- Tests: none (user choice). TDD: off — source: explicit user choice 2026-09-23. Checks: `dotnet build IBUY.slnx` + migration generated and applied to LocalDB + Swagger smoke where useful.
- Delivery: single PR to `master` from branch `feat/backend-version-final`, one Conventional Commit per task. Strategy: `single-pr` (forecast ~1200 authored lines, user accepted).
- Artifacts language: domain identifiers, routes, validation messages and comments stay in Spanish (existing project convention).

## Assumptions (defaults chosen by orchestrator, not yet contradicted)
- Depot responsible: 1:N — `Deposito.UsuarioResponsableId` (nullable FK to Usuario, NoAction), so existing rows migrate cleanly.
- Login: `POST /api/auth/login` with Email + Contrasena. Passwords hashed with BCL PBKDF2 (`Rfc2898DeriveBytes.Pbkdf2`, no extra packages); no JWT/cookies — response returns user, company and depots in charge.
- Necesidad gets `DepositoSolicitanteId` (replaces `DepositoId`) and `DepositoDestinoId`; they must differ.
- NotaPedido POST with `NecesidadId` is rejected (400) when the destination depot already has enough stock for that need.
- `ItemRemito.Cantidad` becomes `decimal(18,2)` to match `Stock.CantidadActual`.
- Existing marketplace mock DTOs (`NotaPedidoDTO`, `ItemNotaDTO` used by IBUY.Cliente/IBUY.Servicios) are renamed to `NotaPedidoMarketplaceDTO` / `ItemNotaMarketplaceDTO` so the backend can use the conventional names.

## Tasks
Route for all tasks: delegated direct (one writer; 2+ non-trivial files per task — writer trigger).

- [x] T1 Entities + migration: remove `Stock.CantidadMinima` (+ StockDTO, `GET api/stock/critico`); add `Deposito.UsuarioResponsableId`; Necesidad solicitante/destino; `ItemRemito.Cantidad` decimal; NoAction on Remito/ItemRemito FKs; new EF migration applied to LocalDB.
- [x] T2 Login + users: PBKDF2 hashing on Usuario create/update, `UsuarioController.Post` returns id; `AuthController` `POST api/auth/login` → `LoginRespuestaDTO` (user, empresa, depósitos a cargo); DepositoDTO/controller handle `UsuarioResponsableId` with FK validation.
- [ ] T3 Necesidad: DTO/controller for solicitante/destino (validation: both exist, differ, product exists); `GET api/necesidad/{id}/revision` → `RevisionNecesidadDTO` (stock disponible en destino, alcanza, faltante).
- [ ] T4 NotaPedido: rename marketplace DTOs; `NotaPedidoDTO` + `ItemNotaDTO` (no navigation entities); `INotaPedidoRepositorio`/`NotaPedidoRepositorio : Repositorio<NotaPedido>` with `InsertarConItems` (transaction) + `ObtenerDetalle`; `NotaPedidoController` `api/notapedido` GET, GET{id}, POST, PUT{id} (header/Estado), DELETE{id}; necesidad stock rule above.
- [ ] T5 Cotizacion (spec below).
- [ ] T6 Remito (spec below).
- [ ] T7 Update CLAUDE.md architecture notes (auth, specific repositories, necesidad flow).

### T5 Cotizacion spec
DTOs `CotizacionDTO` (header with item list) and `ItemCotizacionDTO`. `ICotizacionRepositorio`/`CotizacionRepositorio : Repositorio<Cotizacion>` with `InsertarConItems(Cotizacion, List<ItemCotizacion>)` in a transaction, `ObtenerDetalle(int id)`, `ObtenerPorNotaPedido(int notaPedidoId)`. `CotizacionController` `api/cotizacion`: GET, GET {id} (with items), GET nota/{notaPedidoId}, POST, PUT {id}, DELETE {id}.
Acceptance: quantity/prices > 0 (entity validation); header + items saved in one transaction; Estado managed via PUT; 404 when missing; DTOs contain no navigation entities.
Out of scope: provider/compare endpoints, PATCH estado/seleccionar, only-published-notes validation, preventing self-quotation.

### T6 Remito spec
DTOs `RemitoDTO` (header with items, includes Tipo entrada/salida/transferencia) and `ItemRemitoDTO`. `IRemitoRepositorio`/`RemitoRepositorio : Repositorio<Remito>` with `RegistrarMovimiento(Remito, List<ItemRemito>)` — impacts stock by Tipo; remito, items and stock in one transaction; `ObtenerDetalle(int id)`. `RemitoController` `api/remito`: GET, GET {id} (with items), POST.
Acceptance: at least one item; entrada increments destination stock; salida decrements origin; transferencia decrements origin and increments destination; no negative stock; single transaction; no update/delete of a registered remito.
Out of scope: per-company/per-depot listing, three separate POSTs, same-company transfer validation.

## Checks
- `dotnet build IBUY.slnx` → 0 errors after every task.
- T1: `dotnet ef migrations add <Name> --project IBUY.BD --startup-project IBUY.Server` and `dotnet ef database update ...` succeed.

## Progress / Evidence
(updated per task: commit hash, check results, review tier)

T1: 5a05021 — build: 0 errors — migration `VersionFinal` applied to LocalDB. Backfilled `DepositoDestinoId` (existing rows had no destino) with the first Deposito id different from the solicitante, via a data-migration SQL step (RenameColumn used for Deposito→DepositoSolicitanteId, no data loss besides the intentionally dropped `CantidadMinima`). Minimal NecesidadController/DTO rename included (full revision endpoint deferred to T3).

T2: <pending, recorded in T3 line> — build: 0 errors — `HashContrasenas` (PBKDF2/SHA256, 100k iter, 16B salt) in `IBUY.Server/Seguridad`; hash fits existing `Contrasena` StringLength(8,200) so no entity/migration change needed. `AuthController` (`api/auth/login`) uses generic repos (no query-by-email on `IRepositorio<E>`, so it does `Select()` + LINQ — fine at demo scale). `DepositoDTO`/`DepositoController` gained `UsuarioResponsableId` with FK validation via a new `ValidarRelaciones` helper (same pattern as `NecesidadController`).

## Next step
T3.
