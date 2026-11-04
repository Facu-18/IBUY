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
    }
}
