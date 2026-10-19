using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Gestión de empresas contra la API real (api/empresa).</summary>
    public interface IEmpresaServicio
    {
        /// <summary>Empresas de la plataforma, ordenadas por razón social.</summary>
        Task<HttpResp<List<EmpresaDTO>?>> ObtenerTodasAsync();

        Task<HttpResp<EmpresaDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Devuelve el id creado o el error del servidor.</summary>
        Task<HttpResp<int>> CrearAsync(EmpresaDTO empresa);

        Task<HttpResp<bool>> EditarAsync(int id, EmpresaDTO empresa);
    }
}
