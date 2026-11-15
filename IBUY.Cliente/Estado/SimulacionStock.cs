using IBUY.Shared.DTO;

namespace IBUY.Cliente.Estado
{
    /// <summary>
    /// Ajustes manuales de stock simulados. Viven solo en memoria durante la sesión del
    /// navegador y nunca se envían a la API: el stock real solo se mueve con remitos. Las
    /// pantallas de stock los aplican sobre los datos del servidor para mostrar cómo
    /// quedarían la cantidad, el estado y los indicadores.
    /// </summary>
    public class SimulacionStock
    {
        private readonly Dictionary<int, decimal> cantidades = [];

        public event Action? OnCambio;

        public int Cantidad => cantidades.Count;

        public bool EsSimulado(int stockId) => cantidades.ContainsKey(stockId);

        /// <summary>Registra la cantidad simulada de un stock (nunca negativa).</summary>
        public void Aplicar(int stockId, decimal cantidadSimulada)
        {
            if (cantidadSimulada < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cantidadSimulada), "La cantidad simulada no puede ser negativa.");
            }

            cantidades[stockId] = cantidadSimulada;
            OnCambio?.Invoke();
        }

        public void Descartar(int stockId)
        {
            if (cantidades.Remove(stockId))
            {
                OnCambio?.Invoke();
            }
        }

        public void DescartarTodo()
        {
            if (cantidades.Count > 0)
            {
                cantidades.Clear();
                OnCambio?.Invoke();
            }
        }

        /// <summary>Copia del stock con la cantidad simulada (si la hay) y su estado recalculado.</summary>
        public StockListadoDTO Simular(StockListadoDTO stock)
        {
            if (!cantidades.TryGetValue(stock.Id, out var cantidad))
            {
                return stock;
            }

            return new StockListadoDTO
            {
                Id = stock.Id,
                DepositoId = stock.DepositoId,
                DepositoNombre = stock.DepositoNombre,
                ProductoId = stock.ProductoId,
                ProductoNombre = stock.ProductoNombre,
                Categoria = stock.Categoria,
                CantidadActual = cantidad,
                CantidadMinima = stock.CantidadMinima,
                Estado = EstadosStock.Calcular(cantidad, stock.CantidadMinima)
            };
        }

        public List<StockListadoDTO> Simular(IEnumerable<StockListadoDTO> stocks) =>
            [.. stocks.Select(Simular)];
    }
}
