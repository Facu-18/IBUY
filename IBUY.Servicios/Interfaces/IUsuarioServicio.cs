using IBUY.Shared.DTO;

namespace IBUY.Servicios.Interfaces
{
    public interface IUsuarioServicio : IServicioMock<UsuarioDTO>
    {
        Task<List<UsuarioDTO>> ObtenerPorEmpresaAsync(int empresaId);
    }
}
