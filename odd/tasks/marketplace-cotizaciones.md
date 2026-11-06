# Feature: marketplace-cotizaciones
Objective: adapt the Front-Mariano Marketplace (trimmed 5-point card) to master's role permissions, after merging master into the branch.
Branch: Front-Mariano. Delivery: exception-ok, one commit per unit; push and PR are the user's. Test runner: none in the repo (no-runner exception; E2E checks over HTTP against an isolated database).

## Specs
### S1 Card (verbatim, trimmed version)
Objetivo: que una empresa vea las notas de pedido publicadas por otras empresas y les envíe una cotización, siguiendo la pantalla Marketplace de docs/IBUY Prototipo.html.
- Grilla de tarjetas, una por nota publicada: la empresa compradora; la fecha de vencimiento, con un badge de días restantes: ámbar si faltan 3 días o menos, verde si faltan más; las observaciones y los productos con su cantidad; el botón Cotizar.
- Arriba, un buscador por producto y un contador ("N oportunidades").
- Al tocar Cotizar se abre un modal con los ítems de la nota. Por cada ítem se carga la cantidad ofertada y el precio unitario, y el total se calcula solo. Además se carga el plazo de entrega y se envía.
- Una vez enviada, la tarjeta muestra Cotizada ✓ y queda deshabilitada.
- Backend: GET api/nota-pedido/{id} agrega el Id de cada ítem (ItemNotaDTO.Id) y el nombre del producto. POST api/cotizacion rechaza con 400 si la nota no está Publicada, si está vencida o si la empresa proveedora es la misma que la compradora.
- Criterios: Solo se muestran notas Publicada, no vencidas y de otras empresas. La empresa no puede cotizar sus propias notas. Se valida también en el servidor. La cantidad y el precio tienen que ser mayores que cero, y el total se calcula automáticamente. La cotización se guarda de verdad con POST api/cotizacion (no es simulada) y la tarjeta pasa a "Cotizada ✓". Hay estado de carga y se muestra el mensaje de error del servidor.
- Fuera de alcance: filtros por categoría y por fecha; pantalla de detalle aparte; listado "mis cotizaciones" del proveedor.

### S2 Merge with master
"necesito adaptarlo al recorte de cotizaciones y juntarlo con master"

### S3 Marketplace endpoint (user decision, L3)
New GET api/nota-pedido/marketplace, only for Ventas and SuperAdmin: in one request, the Publicada, non-expired notas of other empresas, with items, product names and the buyer empresa. Existing permissions do not change.

### Assumptions
- A1 "recorte de cotizaciones" = the trimmed 5-point card pasted by the user (no other "recorte" exists in the repo or memory).
- A2 The endpoint also returns, per nota, whether the caller's empresa already quoted it, so the page no longer reads every cotización (GET api/cotizacion is unguarded and lists every empresa's quotes).
- A3 POST api/cotizacion requires X-Usuario-Id, Ventas or SuperAdmin, and a non-SuperAdmin can only quote as its own empresa (403 otherwise); without this the "own nota" rule is bypassed by sending another EmpresaProveedoraId.
- A4 The Marketplace menu entry is shown only to Ventas and SuperAdmin.

## Tasks
- T1 | S2 | inline | done | commit: 5fb783d (merge; conflicts in NavMenu, Program.cs, NotaPedidoController)
- T2 | S1,S3 | inline | done | commit: 0da0f17
- T3 | S1,S3 | delegated independent verifier (high-risk item 2) | done, no blockers; advisory 2 fixed

## Log
L1 (2026-11-06, user): "bien ahora estoy en la rama Front-Mariano lo ultimo que se hizo en esta rama es [card] necesito adaptarlo al recorte de cotizaciones y juntarlo con master"
L2: Merged origin/master (8b41cee) into Front-Mariano as 5fb783d. NavMenu took master's version, Marketplace moved to SeccionesNavegacion (Compras); Program.cs kept both registrations; NotaPedidoController GetById keeps the 403 check before resolving product names. Release build OK. Local .atl registry changes stashed (stash@{0}); they conflicted on pop, so master's registry files were kept and the stash remains.
L3 (user decision): "Endpoint propio (Recomendado)" — with master's permissions the Marketplace was empty (Ventas sees only its empresa's notas; GET {id} of a foreign nota is 403).
L4 (T2 evidence): Release build OK. Scratch DB IBUY_Verif_MP (all migrations), server on 5395. GET marketplace: no header 401, Encargado 403, Ventas B sees notas 1,2 of Empresa A (Publicada, not expired; Borrador 3 and expired 4 hidden, own nota 5 hidden) with item ids and product names; Ventas A sees only nota 5; SuperAdmin sees 1,5,2. POST cotizacion: no header 401, Encargado 403, Ventas B with EmpresaProveedoraId=A 403, own nota 400, Borrador 400, expired 400, price 0 and quantity 0 400; valid 200, PrecioTotal computed server-side (40 x 12.345 -> 493.80). After quoting, Cotizada=true only for Empresa B. GET {id} of a foreign nota still 403; GET api/nota-pedido for Ventas B still returns only its own. UI as Ventas B: menu shows Marketplace, 2 oportunidades, badges amber (2 days) and green (10 days), search cemento -> 1 oportunidad, modal total 15,000.00, empty plazo shows the validation message, send -> card Cotizada and disabled, no console errors. Scratch DB dropped, server stopped. Docs CLAUDE.md/AGENTS.md updated.
L5 (T3): independent verifier on HEAD 0da0f17 with scratch DB IBUY_Verif_V2 (dropped): S1, S3, A2, A3, A4 PASS, no blockers, rejected POSTs left cotizacion rows unchanged, nota-pedido reads unchanged. Advisories: (1) baseline: POST cotizacion accepts an ItemNotaId from another nota; (2) caused by this change: marketplace matches Publicada case-insensitively but POST compared exactly, fixed by comparing case-insensitively in CotizacionController; (3) baseline: GET/PUT/DELETE api/cotizacion unguarded.
