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
- T1 | S1,S2 | inline | pending | backend: column + migration, GET listing/detail DTOs with estado, PUT minimo with permissions
- T2 | S1,S2 | inline | pending | client: service, simulation state, 4 screens + per-depósito view, menu

## Log
L1 (2026-11-12, user): "en otra rama implementemos [card S1]"
L2 (user decision): "Simulación sin guardar (Recomendado)"
