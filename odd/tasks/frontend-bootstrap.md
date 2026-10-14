# Feature: frontend-bootstrap

## Objective
Normalize the Blazor client: keep only what consumes the real API (Depósitos), remove every mock, and style everything with Bootstrap classes only — no custom CSS.

## Problem / Why
- The client mixes mock-backed pages (Productos, Marketplace, Dashboard, fake session identity/profile) with real API pages (Depósitos), which hides what actually works.
- `IBUY.Cliente/wwwroot/css/app.css` (~1164 lines) reimplements what Bootstrap (already loaded from `wwwroot/lib/bootstrap`) provides.

## Decisions (user-confirmed 2026-10-13)
- CSS: **zero custom CSS**. Delete `app.css`; use Bootstrap classes only. Keep the current structure (sidebar + header + content), accepting that exact brand colors become Bootstrap's palette (e.g. dark sidebar `bg-dark`, `btn-primary`).
- Scope: **only Depósitos**. Delete Productos, Marketplace, Dashboard and placeholder pages (Empresas, Usuarios, Stock, Necesidades, Remitos, NotasPedido, Cotizaciones), their mocks/interfaces/mock-only DTOs, and the Comprador/Proveedor toggle. Menu shows only Depósitos; `/` redirects to `/depositos`. Header empresa selector stays (real API via `ICatalogoServicio`).
- Rule in `AGENTS.md`: use Bootstrap classes, no custom CSS.

## Assumptions (orchestrator defaults)
- The fake user identity in `EstadoSesion` (`Usuario`, `Rol`, `Iniciales`, avatar) is mock data and is removed too; `EstadoSesion` keeps only the selected empresa (`EmpresaId`, `Empresa`, `OnCambio`).
- Only allowed inline style: `#blazor-error-ui` needs `style="display:none"` because Blazor shows it by setting `style.display`, and Bootstrap's `d-none`/`[hidden]` use `!important`, which would keep it hidden forever.
- Artifacts language: Spanish identifiers/UI copy (project convention).

## TDD / checks
- TDD: off for this feature — no test project exists (consistent with user choice in `backend-version-final`); markup-only change. Runner: none.
- Checks per task: `dotnet build IBUY.slnx` (0 errors; pre-existing IBUY.BD nullable warnings are known) + grep that no removed symbol remains. T2 adds a visual smoke run of `/depositos` via IBUY.Server when LocalDB is available.

## Delivery
- Branch `feat/frontend-bootstrap` (from `feat/backend-version-final`). One Conventional Commit per task.
- Forecast: mostly deletions (~1700 deleted, ~600 added) — above the ~400 budget; delivery strategy is asked before any PR. RDD: on (global).

## Tasks
- [x] T1 Remove mocks and non-DB pages (route: delegated direct — writer trigger, many files).
- [x] T2 Rewrite layout, components and Depósitos pages with Bootstrap classes; delete `app.css`; Bootstrap spinner/alert for loading and error UI in `index.html` (route: delegated direct, same writer).
- [x] T3 Docs: Bootstrap rule in `AGENTS.md`; update `CLAUDE.md` client section and stale `AGENTS.md` notes (route: inline — docs only).

- [x] T4 Theme from prototype (user 2026-10-14: zero-CSS result did not match the design; reference `docs/IBUY Prototipo.html`, in the repo). Add one theme stylesheet that overrides Bootstrap CSS variables with the prototype palette (navy `#0f1b35`, primary `#2563eb`, slate grays, Nunito Sans) plus minimal layout rules (sidebar/header); keep Bootstrap markup (route: delegated direct — preparation + 2+ files).
- [x] T5 Docs: change the rule in `AGENTS.md`/`CLAUDE.md` from "zero custom CSS" to "Bootstrap for structure; colors/typography only via the theme file" (route: inline — docs only).

## Decision change (user 2026-10-14)
- Supersedes "zero custom CSS": Bootstrap classes for structure and components, plus a single theme file (`wwwroot/css/tema.css`) that redefines Bootstrap variables with the prototype palette. No other custom CSS files, `<style>` blocks, `.razor.css` or inline styles.

## Progress / evidence
- T1 `5903a9f`: deleted 10 pages, PaginaEnConstruccion, 2 mocks + interfaces, 2 marketplace DTOs (grep: unused by IBUY.Server). EstadoSesion keeps only empresa; `/` routes to Depositos. `dotnet build IBUY.slnx`: 0 errors, 23 known warnings. Grep removed symbols: no code hits.
- T2 `2a3c98f`: app.css deleted; layout/components/Depósitos pages on Bootstrap 5.3.3 classes; offcanvas-lg sidebar; spinner in index.html. Build: 0 errors. Grep `style=`/`<style`/old classes: only `#blazor-error-ui` (allowed). Playwright smoke: /depositos at 1280px and 400px renders with real data, no console errors. Not exercised: form, detail page, offcanvas close button. Tradeoffs: mobile card layout for tables replaced by `table-responsive`; h1 focus outline from FocusOnNavigate left as is.
- Parent spot check: `dotnet build IBUY.slnx`: 0 errors, 23 warnings; inline-style grep: 1 allowed hit. Branch diff (T1+T2) 30 files, +288 / -1878.
- T3 `docs` commit: AGENTS.md Bootstrap/no-mocks rule + fixed stale notes (Servicios, specific repositories); CLAUDE.md client section rewritten.

- T4 `59e29d3`: `wwwroot/css/tema.css` (255 lines) overrides `--bs-*` and component variables with the prototype palette + Nunito Sans; hook classes `ibuy-menu`/`ibuy-grupo`/`ibuy-logo`/`ibuy-marca`; soft pill badges. `html { font-size: 18px }` to match the prototype's scale; sidebar `--ibuy-menu-ancho` 18.3rem. Playwright comparison vs prototype at 1280px/400px: palette, typography, sidebar, header match. Parent spot check: build 0 errors / 23 known warnings; inline-style grep 1 allowed hit; `GET api/empresa` and `GET api/deposito` return data (the "Sin empresas" seen by the writer was transient).
- T5 (docs commit): rule changed to "Bootstrap for structure + single theme file `tema.css`".
- RDD: review preflight blocked at untracked selection (`.atl/` untracked, selection JSON schema undocumented) — pending user decision on `.atl/`.

## Next step
User decides delivery (push/PR; strategy for >400 lines) and `.atl/` handling.
