using IBUY.Servicios.Interfaces;
using IBUY.Shared.DTO;

namespace IBUY.Servicios.Mocks
{
    public class UsuarioServicioMock : ServicioMockBase<UsuarioDTO>, IUsuarioServicio
    {
        public UsuarioServicioMock(BaseDatosMock bd) : base(bd)
        {
        }

        protected override List<UsuarioDTO> Coleccion => bd.Usuarios;

        public async Task<List<UsuarioDTO>> ObtenerPorEmpresaAsync(int empresaId)
        {
            await SimularDemora();
            return [.. bd.Usuarios.Where(usuario => usuario.EmpresaId == empresaId)];
        }
    }
}
