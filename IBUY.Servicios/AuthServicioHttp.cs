using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Autenticación real: consume api/auth a través de <see cref="IHttpService"/>.</summary>
    public class AuthServicioHttp : IAuthServicio
    {
        private const string Base = "api/auth";
        private readonly IHttpService http;

        public AuthServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<LoginRespuestaDTO?>> IniciarSesionAsync(LoginDTO login)
        {
            return http.PostAsync<LoginDTO, LoginRespuestaDTO>($"{Base}/login", login);
        }

        public Task<HttpResp<LoginRespuestaDTO?>> RegistrarAsync(RegistroDTO registro)
        {
            return http.PostAsync<RegistroDTO, LoginRespuestaDTO>($"{Base}/registro", registro);
        }
    }
}
