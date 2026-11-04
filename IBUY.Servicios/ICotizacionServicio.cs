using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Cotizaciones reales: consume api/cotizacion.</summary>
    public interface ICotizacionServicio
    {
        Task<HttpResp<List<CotizacionDTO>?>> ObtenerTodasAsync();

        /// <summary>Devuelve el id creado o el error del servidor.</summary>
        Task<HttpResp<int>> CrearAsync(CotizacionDTO cotizacion);
    }
}
