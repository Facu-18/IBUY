namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Nota de pedido publicada por otra empresa, tal como la muestra el Marketplace:
    /// con sus ítems, la empresa compradora y si la empresa del usuario ya la cotizó.
    /// </summary>
    public class OportunidadMarketplaceDTO
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        public string EmpresaRazonSocial { get; set; } = string.Empty;

        public string Observaciones { get; set; } = string.Empty;

        public DateTime FechaVencimiento { get; set; }

        public List<ItemNotaDTO> Items { get; set; } = [];

        /// <summary>La empresa del usuario ya envió una cotización para esta nota.</summary>
        public bool Cotizada { get; set; }
    }
}
