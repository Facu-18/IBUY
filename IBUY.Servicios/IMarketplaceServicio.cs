using IBUY.Shared.DTO;

namespace IBUY.Servicios
{
    public interface IMarketplaceServicio
    {
        /// <summary>Notas de pedido publicadas por otras empresas, disponibles para cotizar.</summary>
        Task<List<NotaPedidoDTO>> ObtenerPublicadasAsync();
    }
}
