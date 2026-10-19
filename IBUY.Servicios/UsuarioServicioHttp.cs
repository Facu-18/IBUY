using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Usuarios reales: consume api/usuario a través de <see cref="IHttpService"/>.</summary>
    public class UsuarioServicioHttp : IUsuarioServicio
    {
        private const string Base = "api/usuario";
        private readonly IHttpService http;

        public UsuarioServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<UsuarioDTO>?>> ObtenerTodosAsync()
        {
            return http.GetAsync<List<UsuarioDTO>>(Base);
        }

        public Task<HttpResp<UsuarioDTO?>> ObtenerPorIdAsync(int id)
        {
            return http.GetAsync<UsuarioDTO>($"{Base}/{id}");
        }

        public Task<HttpResp<int>> CrearAsync(CrearUsuarioDTO usuario)
        {
            return http.PostAsync<CrearUsuarioDTO, int>(Base, usuario);
        }

        public Task<HttpResp<bool>> EditarAsync(int id, CrearUsuarioDTO usuario)
        {
            return http.PutAsync<CrearUsuarioDTO, bool>($"{Base}/{id}", usuario);
        }
    }
}
