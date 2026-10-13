using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface ICotizacionRepositorio : IRepositorio<Cotizacion>
    {
        Task<Cotizacion> InsertarConItems(Cotizacion cotizacion, List<ItemCotizacion> items);

        Task<(Cotizacion? Cotizacion, List<ItemCotizacion> Items)> ObtenerDetalle(int id);

        Task<List<Cotizacion>> ObtenerPorNotaPedido(int notaPedidoId);
    }
}
