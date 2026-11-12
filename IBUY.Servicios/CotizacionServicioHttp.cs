using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Cotizaciones reales: consume api/cotizacion a través de <see cref="IHttpService"/>.</summary>
    public class CotizacionServicioHttp : ICotizacionServicio
    {
        private const string Base = "api/cotizacion";
        private readonly IHttpService http;

        public CotizacionServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<CotizacionDTO>?>> ObtenerTodasAsync()
        {
            return http.GetAsync<List<CotizacionDTO>>(Base);
        }

        public Task<HttpResp<int>> CrearAsync(CotizacionDTO cotizacion)
        {
            return http.PostAsync<CotizacionDTO, int>(Base, cotizacion);
        }
    }
}
