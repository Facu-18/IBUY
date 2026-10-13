namespace IBUY.Shared.DTO
{
    /// <summary>Existencia de un producto dentro de un depósito, con el producto ya resuelto.</summary>
    public class StockDepositoDTO
    {
        public int StockId { get; set; }

        public int ProductoId { get; set; }

        public string ProductoNombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public decimal CantidadActual { get; set; }
    }
}
