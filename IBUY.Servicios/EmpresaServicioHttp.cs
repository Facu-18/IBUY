using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Empresas reales: consume api/empresa a través de <see cref="IHttpService"/>.</summary>
    public class EmpresaServicioHttp : IEmpresaServicio
    {
        private const string Base = "api/empresa";
        private readonly IHttpService http;

        public EmpresaServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<EmpresaDTO>?>> ObtenerTodasAsync()
        {
            return http.GetAsync<List<EmpresaDTO>>(Base);
        }

        public Task<HttpResp<EmpresaDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<EmpresaDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<int>> CrearAsync(EmpresaDTO empresa)
        {
            return http.PostAsync<EmpresaDTO, int>(Base, empresa);
        }

        public Task<HttpResp<bool>> EditarAsync(int id, EmpresaDTO empresa)
        {
            return http.PutAsync<EmpresaDTO, bool>($"{Base}/{id}", empresa);
        }
    }
}
