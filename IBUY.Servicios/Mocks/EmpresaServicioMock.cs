using IBUY.Servicios.Interfaces;
using IBUY.Shared.DTO;

namespace IBUY.Servicios.Mocks
{
    public class EmpresaServicioMock : ServicioMockBase<EmpresaDTO>, IEmpresaServicio
    {
        public EmpresaServicioMock(BaseDatosMock bd) : base(bd)
        {
        }

        protected override List<EmpresaDTO> Coleccion => bd.Empresas;

        public async Task<List<EmpresaDTO>> ObtenerProveedorasAsync()
        {
            await SimularDemora();
            return [.. bd.Empresas.Where(EsProveedora)];
        }

        public async Task<List<EmpresaDTO>> ObtenerCompradorasAsync()
        {
            await SimularDemora();
            return [.. bd.Empresas.Where(EsCompradora)];
        }

        private static bool EsProveedora(EmpresaDTO empresa) =>
            string.Equals(empresa.Tipo, BaseDatosMock.TipoProveedora, StringComparison.OrdinalIgnoreCase);

        private static bool EsCompradora(EmpresaDTO empresa) =>
            string.Equals(empresa.Tipo, BaseDatosMock.TipoCompradora, StringComparison.OrdinalIgnoreCase);
    }
}
