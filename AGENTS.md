# Repository Guide

## Toolchain and Verification

- All six projects target .NET 10; use a .NET 10 SDK. The root solution is `IBUY.slnx`, not a traditional `.sln`.
- Restore/build the whole repository with `dotnet restore IBUY.slnx` and `dotnet build IBUY.slnx`. For a focused check, build the changed `.csproj` directly.
- On Windows, `Directory.Build.props` redirects build and intermediate output to `%LOCALAPPDATA%\IBUY\artifacts`; do not assume project-local `bin/` or `obj/` paths.
- No test projects, lint/format configuration, or CI workflows are checked in. Do not claim test coverage or invent additional verification commands.
- The entity project currently emits nullable-navigation warnings during builds; distinguish these existing warnings from regressions introduced by your changes.

## Runtime

- Run the complete app with `dotnet run --project IBUY.Server/IBUY.Server.csproj --launch-profile https`. `IBUY.Server` hosts both the API and the compiled Blazor WebAssembly client; running `IBUY.Cliente` alone makes its same-origin `HttpClient` target the client dev server instead of the API.
- The server launch profile listens on `https://localhost:7209` and `http://localhost:5132`; Swagger is enabled at `/swagger` in every environment.
- `IBUY.Server/appsettings.json` points `ConnSqlServer` at Windows SQL Server LocalDB database `IBUY`. Database-backed endpoints and EF updates require that service; override the connection string rather than committing machine-specific credentials.

## Project Boundaries

- Domain identifiers, API routes, UI copy, validation messages, and comments use Spanish; preserve that convention when extending existing code.
- `IBUY.Server/Program.cs` wires EF, repositories, controllers, and client hosting. Controllers map entities to DTOs manually and validate foreign-key references before writes (see `DepositoController.ValidarRelaciones`).
- `IBUY.Cliente`: Blazor WebAssembly UI. Shared API contracts belong in `IBUY.Shared`, not in this project.
- `IBUY.BD`: EF entities, `AppDbContext`, and migrations. Despite the project name, `AppDbContext`, `EntityBase`, and `IEntityBase` currently use namespace `Proyecto2026.BD.Datos`.
- `IBUY.Repository`: generic EF CRUD abstraction. `Program.cs` registers the open generic `IRepositorio<>` to `Repositorio<>` for plain CRUD; domain-specific repositories (`INecesidadRepositorio`, `IRemitoRepositorio`, etc.) derive from `Repositorio<E>` and are registered individually.
- `IBUY.Servicios`: client-side services that call the API through `ServiciosHTTP/IHttpService` (responses wrapped in `HttpResp<T>`). It does not contain server business services.
- `IBUY.Cliente/Estado/EstadoSesion` holds the logged user (`LoginRespuestaDTO` from `POST api/auth/login`, persisted in `localStorage`); `Empresa`/`EmpresaId` derive from it. `/login` plus the `MainLayout` guard are UI-only: the client guard is UI-only. Companies sign up at `/registro` (`POST api/auth/registro`: empresa + first `Ventas` user in one transaction, logs in directly). A `SuperAdmin` user is seeded at server startup from the `SuperAdmin` config section (`appsettings.Development.json`); only `SuperAdmin` sees Empresas (menu filter + `Layout/SuperAdminLayout`). Roles (`IBUY.Shared/DTO/Roles`): `SuperAdmin`, `Ventas`, `EncargadoDeposito`.

## Permisos en la API

- Protected endpoints read the `X-Usuario-Id` header (`[FromHeader]`) and use `IBUY.Server/Seguridad/PermisosUsuario` (`IPermisosUsuario`, scoped) to load the user, role and deposits in charge: no/invalid header, unknown or inactive user is 401; no permission is 403; unknown id stays 404.
- `SuperAdmin` sees everything; `Ventas` sees its whole empresa (foreign `empresaId`: 403); `EncargadoDeposito` only the deposits it has in charge. Listings of deposito, stock, necesidad, remito and nota-pedido are filtered this way and accept an optional `depositoId` (not allowed: 403). `GET api/deposito/opciones` returns only `Id`/`Nombre`.
- Only `Ventas`/`SuperAdmin` write usuarios, depósitos and productos and call `POST api/nota-pedido/{id}/publicar`. Usuario POST/PUT rules: `EncargadoDeposito` needs at least one deposit, `Ventas` none, any other role is 400.
- The header is sent by the client and can be spoofed: this is not real security (JWT/cookies are out of scope). Endpoints not listed above (GET productos/empresas/usuarios, writes on stock, necesidad, remito, cotización, empresa) are still unguarded. Details in `CLAUDE.md`.

## Remitos

- `Remito.Tipo` is `Interno` (between depósitos of the same empresa) or `Externo` (received from a company typed by hand in `OrigenExterno`). Every remito is born `Emitido`; the server generates `Numero` (`R-0001`) and ignores `Estado`, `FechaEmision` and `FechaRecepcion` from the client. `Interno` decrements origin stock on creation, `Externo` moves none, and the destination is credited only by `POST api/remito/{id}/recepcion` (total reception, once; a second one is 400). Items carry a required `Unidad`. There is no PUT/DELETE. Details in `CLAUDE.md`.

## Client UI

- The client only contains features backed by the real API (Depósitos, Productos, Necesidades, Empresas, Usuarios, and login). No mock services or fake data: new pages depend on service interfaces, with `*ServicioHttp` implementations registered in `IBUY.Cliente/Program.cs`.
- Use Bootstrap 5 classes (`wwwroot/lib/bootstrap`) for layout and components. The visual identity (palette, typography, sidebar/header look) follows the prototype `docs/IBUY Prototipo.html` (open it in a browser; it is a self-contained bundle, the markup lives inside its `__bundler` script blocks) and lives in a single theme file, `wwwroot/css/tema.css`, loaded after Bootstrap.
- `tema.css` overrides Bootstrap CSS variables (`--bs-*`, including component-level ones like `--bs-btn-bg`) rather than redefining Bootstrap rules. Add a rule there only when no Bootstrap class or variable covers the need.
- No other custom CSS: no extra `.css` files, `<style>` blocks, `.razor.css` scoped styles, or `style=""` attributes. Preserve `style="display:none"` on `#blazor-error-ui` in `index.html`: Blazor toggles it inline; Bootstrap's `d-none` would prevent it from appearing.

## Entity Framework

- Migrations live in `IBUY.BD`, while the startup project and connection string live in `IBUY.Server`. Use both project arguments, for example `dotnet ef migrations add <Name> --project IBUY.BD --startup-project IBUY.Server` and `dotnet ef database update --project IBUY.BD --startup-project IBUY.Server`.
- `dotnet-ef` is not installed as a repo-local tool or declared in a tool manifest. Check availability before migration work; do not edit generated migration designer/snapshot files by hand.
