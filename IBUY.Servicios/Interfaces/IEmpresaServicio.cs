using IBUY.Shared.DTO;

namespace IBUY.Servicios.Interfaces
{
    public interface IEmpresaServicio : IServicioMock<EmpresaDTO>
    {
        Task<List<EmpresaDTO>> ObtenerProveedorasAsync();

        Task<List<EmpresaDTO>> ObtenerCompradorasAsync();
    }
}
