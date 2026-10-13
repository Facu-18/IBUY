using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Catálogos básicos (empresas y usuarios) consumiendo la API real.</summary>
    public class CatalogoServicioHttp : ICatalogoServicio
    {
        private readonly IHttpService http;

        public CatalogoServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<EmpresaDTO>?>> ObtenerEmpresasAsync()
        {
            return http.GetAsync<List<EmpresaDTO>>("api/empresa");
        }

        public Task<HttpResp<List<UsuarioOpcionDTO>?>> ObtenerUsuariosAsync(int empresaId)
        {
            return http.GetAsync<List<UsuarioOpcionDTO>>($"api/usuario/empresa/{empresaId}");
        }
    }
}
