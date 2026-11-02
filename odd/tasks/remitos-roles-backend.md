# Feature: remitos-roles-backend
Objective: align the backend with the new remito model (Interno/Externo) and with role-based access (Ventas/EncargadoDeposito).
Branch: feat/remitos-roles-backend. Delivery: exception-ok, one commit per high-risk unit; push and PR are the user's. Test runner: none in the repo (test-first exception: no runner; E2E checks by HTTP against an isolated database).

## Specs
Verbatim from the user's task cards.

### S1 Remitos
Los remitos pasan a ser de dos tipos. Interno mueve mercadería entre depósitos de la misma empresa. Externo la recibe de una empresa a la que le compramos, y esa empresa se escribe a mano. Todo remito nace Emitido, y el stock del depósito destino se suma solamente cuando se marca como Recibido.

Entidades y migración
- Remito.Tipo: reemplazar Entrada, Salida y Transferencia por Interno y Externo.
- Remito: agregar OrigenExterno (texto, nullable, máximo 100 caracteres).
- ItemRemito: agregar Unidad (texto, obligatorio, máximo 20 caracteres).
- Migración de datos existentes: Transferencia pasa a Interno. Entrada pasa a Externo, con OrigenExterno = "Sin especificar" y unidad "unidad" en sus ítems. Salida: ver "Decisiones a confirmar".

DTOs
- RemitoDTO: agregar OrigenExterno. Numero, Estado, FechaEmision y FechaRecepcion los decide el servidor y se ignoran en el POST.
- RemitoDTO: exponer también DepositoOrigenNombre y DepositoDestinoNombre.
- ItemRemitoDTO: agregar Unidad y ProductoNombre.
- Nuevo RemitoListadoDTO: Id, Numero, Origen, Destino, Tipo, Fecha, Estado, DepositoOrigenId, DepositoDestinoId. Origen es el nombre del depósito o el texto de OrigenExterno, según el tipo.

Repository
- RegistrarMovimiento: Interno descuenta el stock del origen y no toca el destino. Externo no mueve stock. Los dos quedan Emitido.
- RegistrarRecepcion: suma todos los ítems al destino. Validar el estado dentro de la transacción para que no se pueda sumar dos veces.
- ObtenerDetalle: devolver también los nombres de los productos.

Controller (api/remito)
- GET /api/remito: devolver List<RemitoListadoDTO>. Hoy devuelve entidades.
- GET /api/remito/{id}: devolver el remito con sus ítems y los nombres.
- POST /api/remito: registrar el remito con el formato nuevo.
- POST /api/remito/{id}/recepcion: habilitarlo para los dos tipos. Hoy solo acepta Transferencia.
- Generar Numero en el servidor, con formato R-0001.
- No se agregan PUT ni DELETE.

Criterios de aceptación
- Interno exige depósito origen y destino, distintos y de la misma empresa.
- Interno rechaza OrigenExterno.
- Externo exige OrigenExterno y depósito destino de la empresa.
- Externo rechaza depósito origen.
- Un tipo distinto de Interno o Externo devuelve 400.
- El remito tiene al menos un ítem, con cantidad mayor a cero.
- Un remito puede tener varios productos.
- Al crear cualquier remito, el destino no suma stock.
- Un remito interno sin stock suficiente en el origen devuelve 400 y no guarda nada.
- Al recibir, el stock del destino suma cada ítem y crea la fila si no existía.
- Recibir pasa el estado a Recibido y guarda FechaRecepcion.
- Recibir dos veces devuelve 400, y un remito inexistente devuelve 404.
- Remito, ítems y stock se guardan en una sola transacción.
- CLAUDE.md y AGENTS.md quedan actualizados.

### S2 Roles y permisos
La API tiene que aplicar lo que puede ver y hacer cada rol. Hoy solo existen SuperAdmin y Administrador, y ningún endpoint controla permisos.

Roles
- IBUY.Shared/DTO/Roles: agregar Ventas y EncargadoDeposito, con sus helpers.
- AuthController: el primer usuario de una empresa registrada pasa a ser Ventas.
- Migración de datos sobre Usuarios.Rol: los Administrador con depósitos a cargo pasan a EncargadoDeposito; los que no tienen, a Ventas.

Identificar al usuario
- El cliente envía el encabezado X-Usuario-Id. Se lee con [FromHeader] en los endpoints protegidos.
- Crear un helper que busque en la base el usuario, su rol y sus depósitos.
- Sin encabezado, o con un usuario inexistente o inactivo: 401. Sin permiso: 403.
- api/auth/login y api/auth/registro siguen siendo públicos.

Vistas por rol
- Filtrar según el usuario: GET api/deposito; GET api/stock; GET api/necesidad (el usuario es solicitante o destino); GET api/remito (cuando exista la tarea de remitos); GET api/nota-pedido (según la necesidad de origen).
- Si llega depositoId, tiene que ser un depósito permitido para ese usuario; si no, 403.
- Los detalles por {id} aplican la misma regla.
- Nuevo GET api/deposito/opciones: devuelve solo Id y Nombre de los depósitos de la empresa, para los selectores de destino.

Acciones solo para Ventas o SuperAdmin
- Crear, editar y eliminar usuarios, depósitos y productos.
- Nuevo POST api/nota-pedido/{id}/publicar: pasa la nota de Borrador a Publicada.
- El PUT de nota de pedido ya no puede cambiar el estado a Publicada.

Alta de usuario
- Sacar [MinLength(1)] de CrearUsuarioDTO.DepositoIds y validar en UsuarioController: EncargadoDeposito: al menos un depósito de su empresa. Ventas: sin depósitos. Si tenía, quedan sin responsable. Cualquier otro rol, incluido SuperAdmin: 400.

Criterios de aceptación
- Un encargado no obtiene datos de depósitos ajenos, ni por listado, ni por {id}, ni por depositoId.
- Ventas ve todo lo de su empresa y nada de otra.
- Un encargado recibe 403 al publicar una nota y al gestionar usuarios, depósitos o productos.
- Ventas puede publicar una nota en Borrador; publicarla en otro estado da 400.
- GET api/deposito/opciones devuelve únicamente id y nombre.
- Las reglas de alta de usuario se cumplen en POST y PUT.
- CLAUDE.md y AGENTS.md quedan actualizados.

Verificación: dotnet build IBUY.slnx y probar cada criterio por Swagger con cada rol.
Fuera de alcance: JWT o cookies.

### Assumptions (not stated by the user; report them at close)
- A1 Salida: the user's LocalDB has 0 remitos, so no Salida row exists there. The migration still fails loudly if it finds a Salida row, so no data is converted silently.
- A2 Free-text roles: the database also holds Administradora, Aprobadora, Comprador, Deposito, Vendedor and Vendedora. The migration applies the same deposit rule to every role except SuperAdmin.
- A3 Publishing loophole: creating a nota directly as Publicada is rejected too (POST api/nota-pedido), since otherwise the publish rule is bypassed.
- A4 empresaId query parameter: a non-SuperAdmin asking for another company gets 403.
- A5 SuperAdmin sees everything.
- A6 The migration is generated and tested on an isolated database. It is NOT applied to the user's IBUY database without their go-ahead.

## Tasks
- T1 | S1 | delegated (context backstop) | done | commit: 5e74dbd
- T2 | S2 | delegated (context backstop) | done | commit: 51fe0d0
- T3 | S1,S2 | delegated independent verifier (high-risk items 1, 2, 3) | pending

## Log
L1 (2026-11-02, user): "bien apliquemos los 2 cambios en el backend [...] para dejar el backend alineado con estos cambios" followed by both task cards, quoted in Specs.
L2: Checked LocalDB read-only: 0 remitos, 0 items; 12 companies; roles listed in A2. Branch created from origin/master (9e76aa1).
L3: The uncommitted docs/IBUY Prototipo.html change belongs to the earlier mock work and stays out of these commits.
L4 (T1 evidence, commit 5e74dbd): Release build 0 errors. Migration RemitosInternosExternos on scratch DB: Transferencia->Interno, Entrada->Externo + OrigenExterno 'Sin especificar', items Unidad 'unidad'; a Salida row makes the migration THROW 50001 and nothing is converted (no Unidad column). HTTP on scratch DB (port 5392): multi-item Interno 200 (origin stock 10->7 / 4->3, destination unchanged, Numero R-0003, client Numero/Estado/Fecha ignored); insufficient stock 400 with remito/item counts unchanged; Externo 200 no stock move; every validation rule 400; Entrada/Salida types 400; reception 200 creates the missing destination rows and sums each item, FechaRecepcion saved; second reception 400; 3 parallel receptions -> one 200 and two 400, stock added once; unknown id 404; PUT/DELETE 405; list Origen = depot name (Interno) / OrigenExterno (Externo). Scratch DBs IBUY_Verif_T1 and IBUY_Verif_T1S dropped; server stopped. Notes: a rolled-back insert consumes an Id, so Numero can skip a value (R-0004 absent); Unidad keeps a DB default 'unidad' from the migration; user IBUY database untouched (A6).
L5 (T2 evidence, commit 51fe0d0): Release build 0 errors, 0 warnings. Migration RolesVentasEncargado (data only, snapshot unchanged) on scratch DB IBUY_Verif_T2: Administrador with deposit -> EncargadoDeposito, Administrador without -> Ventas, Comprador -> Ventas, Vendedor with deposit -> EncargadoDeposito, SuperAdmin unchanged. HTTP on port 5393 against the scratch DB (two empresas, roles SuperAdmin, Ventas, Encargado with 2 depots, legacy role Comprador): 164 of 166 checks OK in the first run and the 2 FAIL were my test DTO lacking items; re-run with valid items passed (PUT to Publicada 400, Publicada to Publicada 200, POST as Publicada 400, POST Borrador 200). Covered: 401 (no header, bad header, unknown, inactive), login and registro public, first registered user Ventas, 403 for legacy role, Encargado sees only own depots in deposito, stock, necesidad, remito, nota-pedido by list, id and depositoId, Ventas own empresa only (empresaId B 403), SuperAdmin all, opciones only id and nombre, Encargado 403 on usuario, deposito, producto writes and publicar, Ventas CRUD in own empresa and 403 in B, publicar Borrador 200 and Publicada 400 and 404, alta rules in POST and PUT (Ventas PUT clears deposits). POST api/necesidad/{id}/nota-pedido always creates Borrador. Scratch DB dropped and server stopped; user IBUY database untouched (A6; migration RolesVentasEncargado not applied there).
Next step: T3 (independent verifier).
