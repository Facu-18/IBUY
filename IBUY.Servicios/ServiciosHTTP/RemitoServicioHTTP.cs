using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios.ServiciosHTTP
{
    /// <summary>Remitos reales: consume api/remito a través de <see cref="IHttpService"/>.</summary>
    public class RemitoServicioHttp : IRemitoServicio
    {
        private const string Base = "api/remito";
        private readonly IHttpService http;

        public RemitoServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<RemitoListadoDTO>?>> ObtenerTodosAsync()
        {
            return http.GetAsync<List<RemitoListadoDTO>>(Base);
        }

        public Task<HttpResp<RemitoDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<RemitoDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<int>> CrearAsync(RemitoDTO remito)
        {
            return http.PostAsync<RemitoDTO, int>(Base, remito);
        }

        public Task<HttpResp<bool>> RegistrarRecepcionAsync(int id, RecepcionRemitoDTO recepcion)
        {
            return http.PostAsync<RecepcionRemitoDTO, bool>($"{Base}/{id}/recepcion", recepcion);
        }
    }
}
