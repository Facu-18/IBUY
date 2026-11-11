using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Notas de pedido reales: consume api/nota-pedido.</summary>
    public interface INotaPedidoServicio
    {
        Task<HttpResp<List<NotaPedidoResumenDTO>?>> ObtenerTodasAsync();

        /// <summary>Detalle de la nota con los ítems y el nombre de cada producto.</summary>
        Task<HttpResp<NotaPedidoDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Notas publicadas y vigentes de otras empresas, con sus ítems (Marketplace).</summary>
        Task<HttpResp<List<OportunidadMarketplaceDTO>?>> ObtenerMarketplaceAsync();

        Task<HttpResp<int>> CrearAsync(NotaPedidoDTO nota);

        Task<HttpResp<bool>> EditarAsync(int id, NotaPedidoDTO nota);

        Task<HttpResp<bool>> AprobarAsync(int id);

        Task<HttpResp<bool>> PublicarAsync(int id);

        Task<HttpResp<bool>> CancelarAsync(int id);
    }
}
