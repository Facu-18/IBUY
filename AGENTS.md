# Repository Guide

## Toolchain and Verification

- All six projects target .NET 10; use a .NET 10 SDK. The root solution is `IBUY.slnx`, not a traditional `.sln`.
- Restore/build the whole repository with `dotnet restore IBUY.slnx` and `dotnet build IBUY.slnx`. For a focused check, build the changed `.csproj` directly.
- No test projects, lint/format configuration, or CI workflows are checked in. Do not claim test coverage or invent additional verification commands.
- The entity project currently emits nullable-navigation warnings during builds; distinguish these existing warnings from regressions introduced by your changes.

## Runtime

- Run the complete app with `dotnet run --project IBUY.Server/IBUY.Server.csproj --launch-profile https`. `IBUY.Server` hosts both the API and the compiled Blazor WebAssembly client; running `IBUY.Cliente` alone makes its same-origin `HttpClient` target the client dev server instead of the API.
- The server launch profile listens on `https://localhost:7209` and `http://localhost:5132`; Swagger is enabled at `/swagger` in every environment.
- `IBUY.Server/appsettings.json` points `ConnSqlServer` at Windows SQL Server LocalDB database `IBUY`. Database-backed endpoints and EF updates require that service; override the connection string rather than committing machine-specific credentials.

## Project Boundaries

- `IBUY.Server`: composition root, API controllers, static client hosting, and EF registration.
- `IBUY.Cliente`: Blazor WebAssembly UI. Shared API contracts belong in `IBUY.Shared`, not in this project.
- `IBUY.BD`: EF entities, `AppDbContext`, and migrations. Despite the project name, `AppDbContext`, `EntityBase`, and `IEntityBase` currently use namespace `Proyecto2026.BD.Datos`.
- `IBUY.Repository`: generic EF CRUD abstraction. `Program.cs` registers the open generic `IRepositorio<>` to `Repositorio<>`; new entity repositories do not need individual DI registrations.
- `IBUY.Servicios` is currently an empty client-referenced class library; do not assume it contains server business services.

## Entity Framework

- Migrations live in `IBUY.BD`, while the startup project and connection string live in `IBUY.Server`. Use both project arguments, for example `dotnet ef migrations add <Name> --project IBUY.BD --startup-project IBUY.Server` and `dotnet ef database update --project IBUY.BD --startup-project IBUY.Server`.
- `dotnet-ef` is not installed as a repo-local tool or declared in a tool manifest. Check availability before migration work; do not edit generated migration designer/snapshot files by hand.
