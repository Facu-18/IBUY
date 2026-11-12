# Feature: stock-visualizacion
Objective: view and configure stock per product and depósito (listing, per-depósito view, detail, minimum quantity, simulated manual adjustment).
Branch: stock-visualizacion (from local master f18c731). Delivery: exception-ok, one commit per unit; push and PR are the user's. Test runner: none in the repo (no-runner exception; build plus E2E when the environment allows it).

## Specs
### S1 Card (verbatim)
Objetivo: permitir visualizar y configurar el stock por producto y depósito.
Pantallas: Listado general de stock. Vista de stock por depósito. Detalle de stock. Formulario de cantidad mínima. Formulario de ajuste manual simulado.
Funciones reales con el backend: Filtrar por depósito. Filtrar por producto. Mostrar solamente productos críticos. Modificar la cantidad mínima. Realizar ajustes simulados de stock. Identificar stock normal, bajo y agotado.
Criterios de aceptación: No permite ingresar cantidades negativas. Los productos críticos se identifican visualmente. La vista muestra cantidad actual y cantidad mínima. Los indicadores del dashboard se actualizan después de un ajuste. Se solicita una confirmación antes de aplicar un ajuste.

### S2 Ajuste (user decision)
"Simulación sin guardar (Recomendado)": the form computes how stock would end up (normal/bajo/agotado) and, after confirmation, updates the on-screen indicators. It does not touch the database (stock only moves through remitos). The minimum quantity is saved for real.

### Assumptions
- A1 Estado: Agotado if CantidadActual <= 0; Bajo if CantidadActual <= CantidadMinima (CantidadMinima > 0); Normal otherwise. Crítico = Bajo or Agotado.
- A2 "Indicadores del dashboard" = Normal/Bajo/Agotado counters on the stock screens (the Home dashboard stays as is, per the user's earlier instruction not to add data to it).
- A3 Simulated adjustments live in client memory for the session (singleton), are labeled "Simulado", and can be discarded; they never reach the API.
- A4 Minimum quantity: new Stock.CantidadMinima column (decimal, default 0, >= 0), edited with PUT api/stock/{id}/minimo by Ventas/SuperAdmin or the Encargado of that depósito. Quantities are whole numbers in the UI (as in remitos and necesidades).
- A5 GET api/stock returns StockListadoDTO (with names, mínimo and estado) and accepts depositoId, productoId and soloCriticos; no client used its previous entity-shaped response.

## Tasks
- T1 | S1,S2 | inline | done | commit: 6b93c3e (backend: column + migration, listing/detail with estado, PUT minimo with permissions)
- T2 | S1,S2 | inline | done | commit: see L4 (client: service, simulation state, listing, per-depósito view, detail, minimo and ajuste forms)

## Log
L1 (2026-11-12, user): "en otra rama implementemos [card S1]"
L2 (user decision): "Simulación sin guardar (Recomendado)"
L3 (T1 evidence): Debug build OK (Release DLLs are blocked by Windows Application Control on this machine; Debug loads). Scratch DB IBUY_Verif_Stock, server on 5396. GET api/stock: estados Agotado 0/0, Normal 50/0, after mins Bajo 50/60 and Bajo 5/10; productoId, depositoId and soloCriticos filters OK; Encargado sees only its depósito and gets 403 for another; Ventas of another empresa gets []. PUT minimo: no header 401, negative 400, other empresa 403, foreign Encargado 403, own Encargado 200, unknown id 404.
L4 (T2 evidence): UI as Ventas: indicators 1/1/1, 6 columns with actual and minimo, Agotado row table-danger and Bajo row table-warning; product and depósito filters; minimo -1 shows the validation message, 40 saves (Normal); ajuste egreso 100 blocked (negative) with button disabled; egreso 20 previews 30 Bajo, confirmation modal, detail shows simulated 30 Bajo while the real value stays 50; listing indicators 0/2/1 with the Simulado badge; solo criticos 3; per-depósito view; discarding restores 1/1/1. Real stock unchanged in the API (50, minimo 40). No console errors. Scratch DB dropped, server stopped. Simulations are cleared by a full page reload (session memory, by design).
