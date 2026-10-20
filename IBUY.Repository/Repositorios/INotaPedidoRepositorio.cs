using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface INotaPedidoRepositorio : IRepositorio<NotaPedido>
    {
        Task<NotaPedido> InsertarConItems(NotaPedido notaPedido, List<ItemNota> items);

        Task<(NotaPedido? NotaPedido, List<ItemNota> Items)> ObtenerDetalle(int id);

        /// <summary>Notas de pedido generadas a partir de una necesidad.</summary>
        Task<List<NotaPedido>> ObtenerPorNecesidad(int necesidadId);
    }
}
