using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Autenticación contra la API real (api/auth).</summary>
    public interface IAuthServicio
    {
        /// <summary>Valida las credenciales; ante error devuelve el mensaje del servidor (por ejemplo, 401).</summary>
        Task<HttpResp<LoginRespuestaDTO?>> IniciarSesionAsync(LoginDTO login);

        /// <summary>Registra una empresa nueva con su primer usuario y devuelve la sesión de ese usuario.</summary>
        Task<HttpResp<LoginRespuestaDTO?>> RegistrarAsync(RegistroDTO registro);
    }
}
