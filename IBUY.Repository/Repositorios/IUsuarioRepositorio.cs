using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        Task<Usuario> InsertarConDepositos(Usuario usuario, List<int> depositoIds);

        Task<bool> ActualizarConDepositos(Usuario usuario, List<int> depositoIds);

        Task<List<int>> ObtenerDepositoIds(int usuarioId);

        Task<Usuario?> SelectPorEmail(string email);

        Task<Usuario> RegistrarConEmpresa(Empresa empresa, Usuario usuario);

        Task<bool> ExisteConRol(string rol);
    }
}
