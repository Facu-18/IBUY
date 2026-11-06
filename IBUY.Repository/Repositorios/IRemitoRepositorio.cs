using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface IRemitoRepositorio : IRepositorio<Remito>
    {
        /// <summary>
        /// Registra el remito, sus items y el impacto en stock segun el Tipo, todo en una
        /// transaccion. Todo remito queda Emitido y el destino no suma stock aca: Interno
        /// descuenta el origen, Externo no mueve stock. Genera el Numero (R-0001) a partir
        /// del Id insertado. Lanza StockInsuficienteException si un Interno no tiene stock
        /// suficiente en el origen (no se guarda nada).
        /// </summary>
        Task<Remito> RegistrarMovimiento(Remito remito, List<ItemRemito> items);

        /// <summary>
        /// Recepciona un remito Emitido (Interno o Externo): acredita el stock del deposito
        /// destino por cada item (crea la fila si no existia) y pasa el remito a Recibido,
        /// todo en una transaccion. Devuelve null si no existe el remito. Lanza
        /// RemitoYaRecibidoException si ya estaba Recibido.
        /// </summary>
        Task<Remito?> RegistrarRecepcion(int id, DateTime fechaRecepcion);

        /// <summary>Devuelve el remito (con los nombres de sus depositos) y sus items (con el producto).</summary>
        Task<(Remito? Remito, List<ItemRemito> Items)> ObtenerDetalle(int id);

        /// <summary>Todos los remitos con los depositos origen y destino cargados, del mas nuevo al mas viejo.</summary>
        Task<List<Remito>> ObtenerListado();
    }
}
