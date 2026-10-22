# Feature: login

## Objective
Simple client login on top of the existing `POST api/auth/login`: the session holds the logged user (name, role, company, depots) and the whole app uses the user's company instead of the header company selector.

## Problem / Why
- `AuthController.Login` already returns `LoginRespuestaDTO` (user, `Rol`, `EmpresaId`, `EmpresaRazonSocial`, depots in charge) and 401 "Email o contraseña incorrectos." for bad credentials or inactive users, but the client never calls it.
- `EstadoSesion` only holds the company picked in the header `<select>`; it lives in memory, so F5 loses it.
- `AuthController` loads every user and filters in memory.

## Scope (Trello ticket "Login simple en el cliente")
- In: `IAuthServicio`/`AuthServicioHttp`, `EstadoSesion` with logged user + `localStorage` persistence, `/login` page with its own layout, guard in `MainLayout`, header shows user/company/role + "Cerrar sesión", email lookup in `UsuarioRepositorio`, docs on creating the first user.
- Out: JWT/cookies, `[Authorize]` on the API, role permissions, password recovery. The API stays open (UI-only guard) — accepted trade-off.

## Constraints
- Spanish identifiers/UI copy/comments (project convention). Bootstrap + `tema.css` only, no other CSS/`style=""`.
- Pages keep consuming `Sesion.EmpresaId`, `Sesion.Empresa`, `Sesion.OnCambio`: those stay, now derived from the logged user.
- `localStorage` via `IJSRuntime` (`localStorage.getItem/setItem/removeItem`), no extra JS file.

## TDD
Off — source: project-wide choice (no test project, see `backend-version-final`/`frontend-necesidades`). Runner: none.

## Checks
- `dotnet build IBUY.slnx` (0 errors; pre-existing nullable warnings are known).
- API smoke against LocalDB: login OK → 200 with Rol/EmpresaId; wrong password → 401 with message.
- grep: no new `<style>`, `.razor.css`, `style=""`.

## Tasks
- [x] T1 Backend: `IUsuarioRepositorio.SelectPorEmail(email)`; `AuthController` uses it instead of loading all users.
- [x] T2 Client session: `IAuthServicio`/`AuthServicioHttp` (+ register), `EstadoSesion` with `Usuario` (`LoginRespuestaDTO?`), `EstaLogueado`, `IniciarSesionAsync`, `CerrarSesionAsync`, `RestaurarAsync` (localStorage), restore before `RunAsync`.
- [ ] T3 UI: `LoginLayout` + `/login` page, guard in `MainLayout`, header without company selector (user/company/role + "Cerrar sesión"), page copy no longer says "seleccione una empresa en el encabezado".
- [ ] T4 Docs: CLAUDE.md/AGENTS.md client section (login, session) + how to create the first user via Swagger.

## Route
- T1 inline candidate (1–2 files) but bundled with T2–T4 in one delegated writer: writer trigger (2+ non-trivial files across client/servicios/server).

## Delivery
Strategy `ask-on-risk`; forecast ~300–400 authored lines, single PR expected.

## Progress / Evidence
- T1: 5c7dceb
- T2: 70f807c (includes the Encabezado rewrite so the build compiles once CambiarEmpresa was removed)

## Next step
Delegate writer for T1–T4, one commit per task.
