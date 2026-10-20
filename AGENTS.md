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
- `IBUY.Cliente/Estado/EstadoSesion` holds only the header's selected empresa, not an authenticated user. The client does not yet call the backend login endpoint.

## Client UI

- The client only contains features backed by the real API (Depósitos, Productos, and the header empresa selector). No mock services or fake data: new pages depend on service interfaces, with `*ServicioHttp` implementations registered in `IBUY.Cliente/Program.cs`.
- Use Bootstrap 5 classes (`wwwroot/lib/bootstrap`) for layout and components. The visual identity (palette, typography, sidebar/header look) follows the prototype `docs/IBUY Prototipo.html` (open it in a browser; it is a self-contained bundle, the markup lives inside its `__bundler` script blocks) and lives in a single theme file, `wwwroot/css/tema.css`, loaded after Bootstrap.
- `tema.css` overrides Bootstrap CSS variables (`--bs-*`, including component-level ones like `--bs-btn-bg`) rather than redefining Bootstrap rules. Add a rule there only when no Bootstrap class or variable covers the need.
- No other custom CSS: no extra `.css` files, `<style>` blocks, `.razor.css` scoped styles, or `style=""` attributes. Preserve `style="display:none"` on `#blazor-error-ui` in `index.html`: Blazor toggles it inline; Bootstrap's `d-none` would prevent it from appearing.

## Entity Framework

- Migrations live in `IBUY.BD`, while the startup project and connection string live in `IBUY.Server`. Use both project arguments, for example `dotnet ef migrations add <Name> --project IBUY.BD --startup-project IBUY.Server` and `dotnet ef database update --project IBUY.BD --startup-project IBUY.Server`.
- `dotnet-ef` is not installed as a repo-local tool or declared in a tool manifest. Check availability before migration work; do not edit generated migration designer/snapshot files by hand.
