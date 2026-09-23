using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface IRemitoRepositorio : IRepositorio<Remito>
    {
        /// <summary>
        /// Registra el remito, sus items y el impacto en stock segun el Tipo, todo en una
        /// transaccion. Lanza StockInsuficienteException si una salida/transferencia no
        /// tiene stock suficiente en el origen.
        /// </summary>
        Task<Remito> RegistrarMovimiento(Remito remito, List<ItemRemito> items);

        Task<(Remito? Remito, List<ItemRemito> Items)> ObtenerDetalle(int id);
    }
}
