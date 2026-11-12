using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Stock real: consume api/stock a través de <see cref="IHttpService"/>.</summary>
    public class StockServicioHttp : IStockServicio
    {
        private const string Base = "api/stock";
        private readonly IHttpService http;

        public StockServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<StockListadoDTO>?>> ObtenerTodosAsync(int? depositoId = null, int? productoId = null, bool soloCriticos = false)
        {
            var filtros = new List<string>();
            if (depositoId is > 0)
            {
                filtros.Add($"depositoId={depositoId}");
            }

            if (productoId is > 0)
            {
                filtros.Add($"productoId={productoId}");
            }

            if (soloCriticos)
            {
                filtros.Add("soloCriticos=true");
            }

            var url = filtros.Count == 0 ? Base : $"{Base}?{string.Join("&", filtros)}";
            return http.GetAsync<List<StockListadoDTO>>(url);
        }

        public Task<HttpResp<StockListadoDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<StockListadoDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<bool>> ActualizarMinimoAsync(int id, decimal cantidadMinima)
        {
            return http.PutAsync<CantidadMinimaDTO, bool>($"{Base}/{id}/minimo", new CantidadMinimaDTO { CantidadMinima = cantidadMinima });
        }
    }
}
