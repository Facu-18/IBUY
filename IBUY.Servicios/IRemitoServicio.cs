using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Gestión de remitos contra la API real (api/remito).</summary>
    public interface IRemitoServicio
    {
        /// <summary>Historial de remitos; el endpoint devuelve todos, el filtrado lo hace el cliente.</summary>
        Task<HttpResp<List<RemitoDTO>?>> ObtenerTodosAsync();

        /// <summary>Cabecera del remito con sus items.</summary>
        Task<HttpResp<RemitoDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Registra el movimiento (entrada, salida o transferencia); devuelve el id creado.</summary>
        Task<HttpResp<int>> CrearAsync(RemitoDTO remito);

        /// <summary>Recepciona una transferencia emitida: acredita el stock del depósito destino.</summary>
        Task<HttpResp<bool>> RegistrarRecepcionAsync(int id, RecepcionRemitoDTO recepcion);
    }
}
