using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Gestión de remitos contra la API real (api/remito).</summary>
    public interface IRemitoServicio
    {
        /// <summary>Historial de remitos que el usuario puede ver según su rol (lo filtra el servidor).</summary>
        Task<HttpResp<List<RemitoListadoDTO>?>> ObtenerTodosAsync();

        /// <summary>Cabecera del remito con sus items, depósitos y productos con nombre.</summary>
        Task<HttpResp<RemitoDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Registra un remito Externo o Interno; devuelve el id creado.</summary>
        Task<HttpResp<int>> CrearAsync(RemitoDTO remito);

        /// <summary>Marca un remito emitido como recibido: acredita el stock del depósito destino.</summary>
        Task<HttpResp<bool>> RegistrarRecepcionAsync(int id, RecepcionRemitoDTO recepcion);
    }
}
