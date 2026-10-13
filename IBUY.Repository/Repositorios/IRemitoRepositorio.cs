using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface IRemitoRepositorio : IRepositorio<Remito>
    {
        /// <summary>
        /// Registra el remito, sus items y el impacto en stock segun el Tipo, todo en una
        /// transaccion. Entrada y Salida quedan completos en este paso; Transferencia solo
        /// descuenta el origen (el destino se acredita en RegistrarRecepcion). Lanza
        /// StockInsuficienteException si una salida/transferencia no tiene stock suficiente
        /// en el origen.
        /// </summary>
        Task<Remito> RegistrarMovimiento(Remito remito, List<ItemRemito> items);

        /// <summary>
        /// Recepciona una transferencia emitida: acredita el stock del deposito destino por
        /// cada item y pasa el remito a Estado Recibido, todo en una transaccion. Devuelve
        /// null si no existe el remito. El Tipo/Estado ya se validan en el controller.
        /// </summary>
        Task<Remito?> RegistrarRecepcion(int id, DateTime fechaRecepcion);

        Task<(Remito? Remito, List<ItemRemito> Items)> ObtenerDetalle(int id);
    }
}
