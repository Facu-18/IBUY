using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Necesidades de reposición reales: consume api/necesidad a través de <see cref="IHttpService"/>.</summary>
    public class NecesidadServicioHttp : INecesidadServicio
    {
        private const string Base = "api/necesidad";
        private readonly IHttpService http;

        public NecesidadServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<NecesidadDTO>?>> ObtenerTodosAsync(int empresaId, string? estado = null, int? depositoId = null)
        {
            var url = $"{Base}?empresaId={empresaId}";

            if (!string.IsNullOrWhiteSpace(estado))
            {
                url += $"&estado={Uri.EscapeDataString(estado)}";
            }

            if (depositoId.HasValue)
            {
                url += $"&depositoId={depositoId.Value}";
            }

            return http.GetAsync<List<NecesidadDTO>>(url);
        }

        public Task<HttpResp<NecesidadDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<NecesidadDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<int>> CrearAsync(NecesidadDTO necesidad)
        {
            return http.PostAsync<NecesidadDTO, int>(Base, necesidad);
        }

        public Task<HttpResp<bool>> EditarAsync(int id, NecesidadDTO necesidad)
        {
            return http.PutAsync<NecesidadDTO, bool>($"{Base}/{id}", necesidad);
        }

        public Task<HttpResp<RevisionNecesidadDTO?>> ObtenerRevisionAsync(int id)
        {
            return http.GetAsync<RevisionNecesidadDTO>($"{Base}/{id}/revision");
        }

        public Task<HttpResp<List<SugerenciaNecesidadDTO>?>> ObtenerSugerenciasAsync(int depositoId)
        {
            return http.GetAsync<List<SugerenciaNecesidadDTO>>($"{Base}/sugerencias?depositoId={depositoId}");
        }

        public Task<HttpResp<int>> GenerarNotaPedidoAsync(int necesidadId, CrearNotaDesdeNecesidadDTO datos)
        {
            return http.PostAsync<CrearNotaDesdeNecesidadDTO, int>($"{Base}/{necesidadId}/nota-pedido", datos);
        }
    }
}
