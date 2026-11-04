namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Resumen de una nota de pedido para listados. No incluye ítems: el detalle se
    /// consulta con GET api/nota-pedido/{id}.
    /// </summary>
    public class NotaPedidoResumenDTO
    {
        public int Id { get; set; }

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaEmision { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public int EmpresaId { get; set; }
    }
}
