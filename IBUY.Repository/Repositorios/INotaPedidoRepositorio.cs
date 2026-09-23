using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface INotaPedidoRepositorio : IRepositorio<NotaPedido>
    {
        Task<NotaPedido> InsertarConItems(NotaPedido notaPedido, List<ItemNota> items);

        Task<(NotaPedido? NotaPedido, List<ItemNota> Items)> ObtenerDetalle(int id);
    }
}
