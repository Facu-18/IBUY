# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

IBUY is a B2B purchasing platform (companies, warehouses, products, stock, purchase needs, quotations, purchase orders, delivery notes) built as a hosted Blazor WebAssembly app on .NET 10 with EF Core + SQL Server. The domain language is Spanish: identifiers, routes, UI copy, validation messages, and comments are written in Spanish — keep new code consistent with that.

## Commands

```bash
dotnet build IBUY.slnx                      # build everything
dotnet run --project IBUY.Server            # run API + hosted client (http://localhost:5132, https://localhost:7209)
# Swagger UI is always enabled at /swagger

# EF Core migrations (DbContext lives in IBUY.BD, startup/config in IBUY.Server)
dotnet ef migrations add <Name> --project IBUY.BD --startup-project IBUY.Server
dotnet ef database update --project IBUY.BD --startup-project IBUY.Server
```

- There is no test project yet.
- Connection string `ConnSqlServer` in `IBUY.Server/appsettings.json` points at LocalDB (`(localdb)\MSSQLLocalDB`, database `IBUY`).
- On Windows, `Directory.Build.props` redirects all build output to `%LOCALAPPDATA%\IBUY\artifacts` (`UseArtifactsOutput`), so there are no `bin/`/`obj/` folders inside the project directories.

## Architecture

Project dependency graph:

```
IBUY.Server ──> IBUY.Cliente ──> IBUY.Servicios ──> IBUY.Shared
     │                                                  ▲
     ├──> IBUY.Repository ──> IBUY.BD ──────────────────┤
     └──> IBUY.BD                                       │
```

- **IBUY.BD** — EF Core `AppDbContext`, entities under `Datos/Entity`, and migrations. All entities inherit `EntityBase` (`int Id`, implements `IEntityBase`). Validation and schema live in data annotations on the entities (`[Required]`, `[Range]`, `[Column(TypeName="decimal(18,2)")]`, `[DeleteBehavior(NoAction)]`) — there is no `OnModelCreating` fluent config. Note: `AppDbContext`/`EntityBase` use the legacy namespace `Proyecto2026.BD.Datos`, while entities use `IBUY.BD.Datos.Entity`.
- **IBUY.Repository** — a generic `IRepositorio<E>`/`Repositorio<E>` (Select, SelectById, Insert, Update, Delete, Existe) constrained to `IEntityBase`, registered as an open generic in `IBUY.Server/Program.cs` for plain CRUD. Domain operations that need more than CRUD (multi-entity transactions, direct queries) get a specific repository under `Repositorios/` — `INecesidadRepositorio`, `INotaPedidoRepositorio`, `ICotizacionRepositorio`, `IRemitoRepositorio`, `IUsuarioRepositorio` — each deriving from `Repositorio<E>` (whose `context` is `protected` for this) and registered individually in `Program.cs`. Header+items inserts (`InsertarConItems`/`RegistrarMovimiento`) run inside `context.Database.BeginTransactionAsync()` / `CommitAsync()`; an uncommitted transaction rolls back on dispose (EF Core default), so a thrown exception mid-operation leaves no partial writes.
- **IBUY.Server** — ASP.NET Core API. Controllers under `Controllers/` follow a fixed pattern (see `NecesidadController`): route `api/<entity>` in lowercase singular, manual mapping between entity and DTO (no AutoMapper), `Existe` checks returning `NotFound` with a Spanish message, and explicit FK existence checks (`ValidarRelaciones`) before insert/update so FK violations become 400s instead of 500s. There is no ASP.NET Identity or JWT: `AuthController` (`POST api/auth/login`) checks email/password against `Usuario.Contrasena`, hashed with PBKDF2 (`IBUY.Server/Seguridad/HashContrasenas`: `Rfc2898DeriveBytes.Pbkdf2`/SHA256, 100k iterations, random salt, `CryptographicOperations.FixedTimeEquals` to verify) on every Usuario create/update; a successful login returns the user, empresa and the depósitos where `UsuarioResponsableId` matches. The server also hosts the Blazor client (`UseBlazorFrameworkFiles` + `MapFallbackToFile("index.html")`).
- **IBUY.Shared** — DTOs (`DTO/`) shared by server and client.
- **IBUY.Servicios** — service interfaces consumed by the client (`IProductoServicio`, `IMarketplaceServicio`) and their in-memory `*Mock` implementations.
- **IBUY.Cliente** — Blazor WASM UI. Pages under `Pages/`, reusable components under `Componentes/`, layout under `Layout/`.

### Domain flow: necesidad → revisión → nota de pedido / remito

Nothing here is automatic. A `Necesidad` links a `DepositoSolicitanteId` (who needs it) to a `DepositoDestinoId` (who should have it) plus a product and quantity. `GET api/necesidad/{id}/revision` only reports whether the destination depot's stock covers it — read-only, changes nothing. If it doesn't, a `NotaPedido` is created by hand from that necesidad (`POST api/notapedido` with `NecesidadId`, rejected with 400 if the destination already has enough stock). Stock only moves through `Remito` (`POST api/remito`, no PUT/DELETE): `Tipo` is `Entrada`/`Salida`/`Transferencia` (validated case-insensitively, stored canonical). Stock rows are keyed by `(DepositoId, ProductoId)` — a missing row is created on an entrada/transferencia destination, and a salida/transferencia-out without enough stock is rejected with 400 (`StockInsuficienteException` thrown in the repository, mapped to `BadRequest` in the controller).

### Client is currently mock-backed

The client does **not** call the API yet. `IBUY.Cliente/Program.cs` registers the `*ServicioMock` services as singletons (in-memory data, reset on page reload), and `Estado/EstadoSesion` fakes the logged-in company/user/profile (Comprador/Proveedor) since there is no authentication. Wiring a page to real data means adding an HTTP-backed implementation of the service interface and swapping the DI registration — pages depend only on the interface.
