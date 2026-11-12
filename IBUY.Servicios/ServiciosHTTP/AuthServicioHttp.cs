using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios.ServiciosHTTP
{
    /// <summary>Login real: consume api/auth/login a través de <see cref="IHttpService"/>.</summary>
    public class AuthServicioHttp : IAuthServicio
    {
        private const string Base = "api/auth";
        private readonly IHttpService http;

        public AuthServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<LoginRespuestaDTO?>> IniciarSesionAsync(LoginDTO credenciales)
        {
            return http.PostAsync<LoginDTO, LoginRespuestaDTO>($"{Base}/login", credenciales);
        }
    }
}
