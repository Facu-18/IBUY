namespace IBUY.Shared.DTO
{
    /// <summary>Existencia de un producto en un depósito, con nombres, mínimo y estado.</summary>
    public class StockListadoDTO
    {
        public int Id { get; set; }

        public int DepositoId { get; set; }

        public string DepositoNombre { get; set; } = string.Empty;

        public int ProductoId { get; set; }

        public string ProductoNombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public decimal CantidadActual { get; set; }

        public decimal CantidadMinima { get; set; }

        /// <summary>Normal, Bajo o Agotado (ver <see cref="EstadosStock"/>).</summary>
        public string Estado { get; set; } = string.Empty;
    }
}
