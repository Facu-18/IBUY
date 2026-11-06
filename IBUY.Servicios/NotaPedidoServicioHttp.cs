using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Notas de pedido reales: consume api/nota-pedido a través de <see cref="IHttpService"/>.</summary>
    public class NotaPedidoServicioHttp : INotaPedidoServicio
    {
        private const string Base = "api/nota-pedido";
        private readonly IHttpService http;

        public NotaPedidoServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<NotaPedidoResumenDTO>?>> ObtenerTodasAsync()
        {
            return http.GetAsync<List<NotaPedidoResumenDTO>>(Base);
        }

        public Task<HttpResp<NotaPedidoDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<NotaPedidoDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<List<OportunidadMarketplaceDTO>?>> ObtenerMarketplaceAsync()
        {
            return http.GetAsync<List<OportunidadMarketplaceDTO>>($"{Base}/marketplace");
        }
    }
}
