using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Gestión de usuarios contra la API real (api/usuario).</summary>
    public interface IUsuarioServicio
    {
        /// <summary>Usuarios de la plataforma. El listado nunca expone la contraseña.</summary>
        Task<HttpResp<List<UsuarioDTO>?>> ObtenerTodosAsync();

        Task<HttpResp<UsuarioDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Devuelve el id creado o el error del servidor.</summary>
        Task<HttpResp<int>> CrearAsync(CrearUsuarioDTO usuario);

        Task<HttpResp<bool>> EditarAsync(int id, CrearUsuarioDTO usuario);
    }
}
