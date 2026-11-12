# Feature: frontend-necesidades

## Objective
Expose "Necesidades de reposición" in the Blazor client on top of the existing backend: list with filters, detail with stock review, manual create form fed by critical-stock suggestions, state change, and generating a purchase order (nota de pedido) from a need.

## Problem / Why
- `Necesidad` / `NecesidadController` / `NecesidadRepositorio` exist in the backend (see `backend-version-final` T3), but the client has no service, pages or menu entry — `GET api/necesidad` even returned raw entities, not a DTO.
- `NotaPedido.NecesidadId` existed but nothing produced a nota from a need, and nothing prevented a second one.
- The prototype (`docs/IBUY Prototipo.html`) has Necesidades and "Revisar Necesidad" screens, but its markup is inline-styled React, unusable verbatim: translated to Bootstrap classes + `tema.css` per `AGENTS.md`.

## Decisions (user-confirmed 2026-10-20)
- **Nota de pedido: real, not simulated.** Reuse `api/nota-pedido` and the existing `NotaPedido.NecesidadId` FK, so the linkage persists.
- **Generation: manual + suggestions.** Keep the manual form and add a critical-stock suggestion list (no new `CantidadMinima` column, no migration).
- **States: `Pendiente` / `En nota` / `Cubierta`**, matching the prototype; badge reads `En nota N.º X` when a nota exists.
- **Critical stock = `Stock.CantidadActual <= 0`** in the destination depot; suggested quantity 1 (no minimums table exists).
- **Duplicate = same producto + same solicitante + same destino while `Pendiente`.**
- **Nota creator `UsuarioId`** is picked in the modal from the company's active users (no login yet).

## Checks
- No test project exists (project-wide choice): checks are `dotnet build IBUY.slnx` (0 errors; pre-existing IBUY.BD nullable warnings are known) plus an end-to-end API smoke against LocalDB, and a grep that no custom CSS/style was added.
- Verification runner: none.

## Tasks
Route: inline (single coherent feature; writer used for the parallel page authoring).

- [x] T1 `IBUY.Shared` contracts: `NecesidadDTO` gains `Id` + read-only resolved fields (`ProductoNombre`, `ProductoCategoria`, depot names, `EmpresaId`, `NotaPedidoId`, `StockDisponible`); new `SugerenciaNecesidadDTO`, `CrearNotaDesdeNecesidadDTO`; `EstadosNecesidad` / `EstadosNotaPedido` constants in Shared so the client does not reference `IBUY.BD`.
- [x] T2 `IBUY.Repository`: `INecesidadRepositorio` gains `ExistePendienteDuplicada`, `ObtenerDeEmpresa`, `ObtenerDepositosDeEmpresa`, `ObtenerStockDeDeposito`, `ObtenerNotasPorNecesidad`, `GenerarNotaPedido` (single transaction: nota + one item + need to `En nota`); `INotaPedidoRepositorio.ObtenerPorNecesidad`.
- [x] T3 `IBUY.Server`: `NecesidadController` list/detail return enriched DTOs with `empresaId`/`estado`/`depositoId` filters; POST/PUT validate quantity > 0, valid state, FKs, and duplicate pending; new `GET api/necesidad/sugerencias?depositoId=` and `POST api/necesidad/{id}/nota-pedido`; DELETE blocked when a nota is linked. `NotaPedidoController` rejects a second nota for the same need.
- [x] T4 `IBUY.Servicios`: `INecesidadServicio` + `NecesidadServicioHttp` (list, detail, create, edit, revision, suggestions, generate nota); registered in `IBUY.Cliente/Program.cs`.
- [x] T5 Client pages: `Necesidades.razor` (search + estado + depósito filters, badge states, row actions, three distinct empty-state messages), `NecesidadDetalle.razor` (detail, stock review, state change, nota modal), `NecesidadFormulario.razor` (EditForm + DataAnnotations, depot/product combos, suggestion panel refreshed on destination change); `NavMenu` gains an `Inventario` group with Necesidades (icon already existed in `Icono.razor`).

## Progress / evidence
- `dotnet build IBUY.slnx`: 0 errors; only the pre-existing `IBUY.BD` CS8618 nullable-navigation and `Usuarios.razor` CS0649 warnings remain.
- API smoke against LocalDB (seeded via the API: 1 empresa, 2 depósitos, 1 usuario, 2 productos, stock 0 in destino):
  - `POST /api/necesidad` with `CantidadRequerida: 0` → 400 "La cantidad requerida debe ser mayor a cero."
  - Valid create → id 1; repeating the same producto/depósitos in `Pendiente` → 400 "Ya existe una necesidad pendiente para ese producto y esos depósitos."
  - `GET /api/necesidad?empresaId=1`, `&estado=Pendiente`, `&depositoId=2` all return the enriched DTO (producto, depósitos, `notaPedidoId: 0`, `stockDisponible: 0`).
  - `GET /api/necesidad/1/revision` → `alcanza: false`, `faltante: 20`.
  - `POST /api/necesidad/1/nota-pedido` → 1; need becomes `En nota` with `notaPedidoId: 1`; `GET /api/nota-pedido/1` shows exactly one item `{productoId: 1, cantidadSolicitada: 20}` — the need's product and quantity, auto-filled.
  - Second nota for the same need → 400 "La necesidad ya tiene una nota de pedido generada."; DELETE of a linked need → 400; `GET /api/necesidad/sugerencias?depositoId=2` marks `yaTienePendiente` correctly; `Estado: "Raro"` on PUT → 400 listing the valid states; same solicitante/destino → 400.
  - `PUT` to `Cubierta` → true.
- Client: `/necesidades`, `/necesidades/1`, `/necesidades/nuevo` all serve the SPA shell; the new types are present in the compiled `IBUY.Cliente.dll` / `IBUY.Shared.dll` under `%LOCALAPPDATA%\IBUY\artifacts`.
- Bug fixed during self-review: the form's save button was `type="button"` with `@onclick`, which bypassed `EditForm` validation; it now submits inside the form, and the suggestion panel reloads when the destination depot changes.
- Not exercised: browser-level rendering (no Playwright installed; installing a browser stack was out of scope), so the visual result relies on build + API verification and the existing `tema.css` conventions.

## Next step
Optional: browser smoke of the three routes with real data once a browser automation tool is available, and wiring the "Nota de pedido N.º X" link to a future notas page.
