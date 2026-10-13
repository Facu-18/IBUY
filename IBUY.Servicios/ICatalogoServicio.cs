using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    public interface ICatalogoServicio
    {
        /// <summary>Empresas de la plataforma, para el selector de empresa activa.</summary>
        Task<HttpResp<List<EmpresaDTO>?>> ObtenerEmpresasAsync();

        /// <summary>Usuarios activos de una empresa, para combos (solo Id y Nombre).</summary>
        Task<HttpResp<List<UsuarioOpcionDTO>?>> ObtenerUsuariosAsync(int empresaId);
    }
}
