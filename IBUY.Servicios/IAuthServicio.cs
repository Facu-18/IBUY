using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Inicio de sesión contra la API real (api/auth).</summary>
    public interface IAuthServicio
    {
        /// <summary>Valida email y contraseña; devuelve el usuario, su empresa y sus depósitos a cargo.</summary>
        Task<HttpResp<LoginRespuestaDTO?>> IniciarSesionAsync(LoginDTO credenciales);
    }
}
