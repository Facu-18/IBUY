using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Depósitos reales: consume api/deposito a través de <see cref="IHttpService"/>.</summary>
    public class DepositoServicioHttp : IDepositoServicio
    {
        private const string Base = "api/deposito";
        private readonly IHttpService http;

        public DepositoServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<DepositoDTO>?>> ObtenerTodosAsync(int? empresaId, int? depositoId = null)
        {
            var url = empresaId.HasValue ? $"{Base}?empresaId={empresaId.Value}" : Base;

            if (depositoId.HasValue)
            {
                url += $"{(empresaId.HasValue ? "&" : "?")}depositoId={depositoId.Value}";
            }

            return http.GetAsync<List<DepositoDTO>>(url);
        }

        public Task<HttpResp<List<DepositoOpcionDTO>?>> ObtenerOpcionesAsync(int? empresaId = null)
        {
            var url = empresaId.HasValue ? $"{Base}/opciones?empresaId={empresaId.Value}" : $"{Base}/opciones";
            return http.GetAsync<List<DepositoOpcionDTO>>(url);
        }

        public Task<HttpResp<DepositoDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<DepositoDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<int>> CrearAsync(DepositoDTO deposito)
        {
            return http.PostAsync<DepositoDTO, int>(Base, deposito);
        }

        public Task<HttpResp<bool>> EditarAsync(int id, DepositoDTO deposito)
        {
            return http.PutAsync<DepositoDTO, bool>($"{Base}/{id}", deposito);
        }

        public Task<HttpResp<List<StockDepositoDTO>?>> ObtenerStockAsync(int id)
        {
            return http.GetAsync<List<StockDepositoDTO>>($"{Base}/{id}/stock");
        }
    }
}
