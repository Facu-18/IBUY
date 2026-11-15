using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Stock contra la API real (api/stock).</summary>
    public interface IStockServicio
    {
        /// <summary>Stock visible para el usuario, con estado; los filtros son opcionales.</summary>
        Task<HttpResp<List<StockListadoDTO>?>> ObtenerTodosAsync(int? depositoId = null, int? productoId = null, bool soloCriticos = false);

        Task<HttpResp<StockListadoDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Carga inicial del stock de un producto en un depósito; devuelve el id creado.</summary>
        Task<HttpResp<int>> CrearAsync(StockDTO stock);

        /// <summary>Cambia solo la cantidad mínima; la actual se mueve con remitos.</summary>
        Task<HttpResp<bool>> ActualizarMinimoAsync(int id, decimal cantidadMinima);
    }
}
