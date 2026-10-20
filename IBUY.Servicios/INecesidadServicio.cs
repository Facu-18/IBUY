using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Necesidades de reposición reales: consume api/necesidad.</summary>
    public interface INecesidadServicio
    {
        /// <summary>Necesidades de la empresa, con filtros opcionales de estado y depósito.</summary>
        Task<HttpResp<List<NecesidadDTO>?>> ObtenerTodosAsync(int empresaId, string? estado = null, int? depositoId = null);

        Task<HttpResp<NecesidadDTO?>> ObtenerPorIdAsync(int id);

        Task<HttpResp<int>> CrearAsync(NecesidadDTO necesidad);

        Task<HttpResp<bool>> EditarAsync(int id, NecesidadDTO necesidad);

        /// <summary>Stock disponible en el depósito destino, solo informativo.</summary>
        Task<HttpResp<RevisionNecesidadDTO?>> ObtenerRevisionAsync(int id);

        /// <summary>Productos con stock crítico en un depósito, como propuesta de carga.</summary>
        Task<HttpResp<List<SugerenciaNecesidadDTO>?>> ObtenerSugerenciasAsync(int depositoId);

        /// <summary>Genera la nota de pedido de la necesidad y devuelve su id.</summary>
        Task<HttpResp<int>> GenerarNotaPedidoAsync(int necesidadId, CrearNotaDesdeNecesidadDTO datos);
    }
}
