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
- User must be assigned ≥1 depósito on create (user request 2026-09-23); depósitos must belong to the user's empresa; an already-assigned depósito is reassigned.

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
- [x] T3 Necesidad: DTO/controller for solicitante/destino (validation: both exist, differ, product exists); `GET api/necesidad/{id}/revision` → `RevisionNecesidadDTO` (stock disponible en destino, alcanza, faltante).
- [x] T4 NotaPedido: rename marketplace DTOs; `NotaPedidoDTO` + `ItemNotaDTO` (no navigation entities); `INotaPedidoRepositorio`/`NotaPedidoRepositorio : Repositorio<NotaPedido>` with `InsertarConItems` (transaction) + `ObtenerDetalle`; `NotaPedidoController` `api/nota-pedido` (route fixed to `api/nota-pedido` in T9) GET, GET{id}, POST, PUT{id} (header/Estado), DELETE{id}; necesidad stock rule above.
- [x] T5 Cotizacion (spec below).
- [x] T6 Remito (spec below).
- [x] T7 Update CLAUDE.md architecture notes (auth, specific repositories, necesidad flow).
- [x] T8 Usuario: assign depósitos on create/update (DepositoIds, IUsuarioRepositorio with transaction, same-empresa validation, reassign if already assigned)
- [x] T9 Route: `NotaPedidoController` route `api/notapedido` → `api/nota-pedido`.
- [x] T10 Remito two-step: emisión (POST, decrements stock) / recepción (`POST api/remito/{id}/recepcion`, increments stock) for transferencias; Entrada = reception-only from external supplier; Salida = emission-only.

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

T2: fec53d5 — build: 0 errors — `HashContrasenas` (PBKDF2/SHA256, 100k iter, 16B salt) in `IBUY.Server/Seguridad`; hash fits existing `Contrasena` StringLength(8,200) so no entity/migration change needed. `AuthController` (`api/auth/login`) uses generic repos (no query-by-email on `IRepositorio<E>`, so it does `Select()` + LINQ — fine at demo scale). `DepositoDTO`/`DepositoController` gained `UsuarioResponsableId` with FK validation via a new `ValidarRelaciones` helper (same pattern as `NecesidadController`).

T3: ae50fd6 — build: 0 errors — made `Repositorio<E>.context` protected; added `INecesidadRepositorio`/`NecesidadRepositorio : Repositorio<Necesidad>` with `ObtenerStockDisponible(depositoId, productoId)` (queries `context.Set<Stock>()` directly — reused by T4 for the NotaPedido stock rule); registered in Program.cs. `GET api/necesidad/{id}/revision` added to `NecesidadController`. The "both exist / differ" validation was already done in T1 to keep that build green; this task didn't need to repeat it.

T4: 2435a2b — build: 0 errors — renamed marketplace DTOs (`NotaPedidoDTO`→`NotaPedidoMarketplaceDTO`, `ItemNotaDTO`→`ItemNotaMarketplaceDTO`) and updated all references (`IMarketplaceServicio`, `MarketplaceServicioMock`, `Marketplace.razor`). New backend `NotaPedidoDTO`/`ItemNotaDTO` (`NecesidadId` is a required int, not nullable, matching "created manually from a need" — entity FK stays nullable int? for schema flexibility). `INotaPedidoRepositorio`/`NotaPedidoRepositorio` with `InsertarConItems` (transaction) and `ObtenerDetalle` (tuple, no DTOs in the repo layer). `NotaPedidoController` enforces ≥1 item, FK existence, and the stock rule via `INecesidadRepositorio.ObtenerStockDisponible` (reused from T3); PUT only updates header/Estado, not items. No migration needed (no entity changes).

T5: dce6c0b — build: 0 errors — `CotizacionDTO`/`ItemCotizacionDTO` mirror the entities' existing Range validations (cantidad > 0, precios >= 0 — matches the actual entity attributes; the feature doc's "prices > 0" is a slight simplification). `ICotizacionRepositorio`/`CotizacionRepositorio`: `InsertarConItems` (transaction), `ObtenerDetalle` (tuple), `ObtenerPorNotaPedido`. `CotizacionController` (`api/cotizacion`): GET, GET{id} with items, GET nota/{notaPedidoId} (list of entities, same pattern as root GET), POST (validates NotaPedido/Empresa/ItemNota FKs, ≥1 item), PUT (header incl. Estado, items untouched), DELETE. No entity/migration changes.

T6: 88b57e3 — build: 0 errors — `IRemitoRepositorio`/`RemitoRepositorio.RegistrarMovimiento` does remito + items + stock impact in one transaction; stock rows found/created by (DepositoId, ProductoId); insufficient origin stock throws `StockInsuficienteException` (repo layer), caught in `RemitoController.Post` and mapped to `BadRequest` — the transaction rolls back automatically on dispose without `CommitAsync()` (EF Core default), so no partial writes. `RemitoController` validates Tipo case-insensitively and stores the canonical value; per-type required-deposit and Transferencia-must-differ checks live in the controller (`ValidarTipoYDepositos`). No PUT/DELETE, per spec. No entity/migration changes (Item_Remito's decimal Cantidad and NoAction FKs were already done in T1).

T7: <recorded in final report — this is the last task, no following task line to carry it> — build: 0 errors — Architecture section updated: IBUY.Repository bullet now covers specific repositories + transactions; IBUY.Server bullet covers AuthController/PBKDF2 (no Identity/JWT); new "Domain flow" subsection covers necesidad→revision→manual NotaPedido/remito and the remito stock rules. Kept concise (2 edits, ~10 added lines), no restructuring of the rest of the file.

T8: 7eb8e72 — build: 0 errors — `CrearUsuarioDTO.DepositoIds` (`[MinLength(1)]`, empty list default) and `UsuarioDTO.DepositoIds` (response only). New `IUsuarioRepositorio`/`UsuarioRepositorio : Repositorio<Usuario>` with `InsertarConDepositos` (transaction: insert user, then set `UsuarioResponsableId` on the given depósitos), `ActualizarConDepositos` (transaction: update user, clear `UsuarioResponsableId` on depósitos no longer listed, set it on the listed ones; returns false if the user doesn't exist), `ObtenerDepositoIds`; registered in Program.cs. `UsuarioController` now injects `IUsuarioRepositorio` + `IRepositorio<Empresa>` + `IRepositorio<Deposito>`; `ValidarRelaciones` (same pattern as `NecesidadController`/`DepositoController`) checks empresa exists and every (deduplicated) depósito exists and belongs to that empresa; POST/PUT use it before hashing/saving. GET{id} fills `DepositoIds` via `ObtenerDepositoIds`. Build initially failed only on file-lock copy errors from a leftover running `IBUY.Server.exe` dev process (not a compile error); stopped that process and rebuilt clean.

## Next step
None — all T1..T8 done. Feature ready for user review / PR.
