namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Nota de pedido publicada. Por ahora solo la usa el marketplace para mostrar
    /// las oportunidades disponibles.
    /// </summary>
    public class NotaPedidoDTO
    {
        public int Id { get; set; }

        public string EmpresaNombre { get; set; } = string.Empty;

        public string Observaciones { get; set; } = string.Empty;

        public DateTime FechaVencimiento { get; set; }

        public List<ItemNotaDTO> Items { get; set; } = [];

        public int DiasRestantes => (FechaVencimiento.Date - DateTime.Today).Days;
    }
}
