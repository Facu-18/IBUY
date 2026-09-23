using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        Task<Usuario> InsertarConDepositos(Usuario usuario, List<int> depositoIds);

        Task<bool> ActualizarConDepositos(Usuario usuario, List<int> depositoIds);

        Task<List<int>> ObtenerDepositoIds(int usuarioId);
    }
}
