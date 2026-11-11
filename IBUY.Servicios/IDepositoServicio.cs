using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Gestión de depósitos contra la API real (api/deposito).</summary>
    public interface IDepositoServicio
    {
        /// <summary>Depósitos visibles; si se informan empresaId o depositoId, se usan como filtros.</summary>
        Task<HttpResp<List<DepositoDTO>?>> ObtenerTodosAsync(int? empresaId, int? depositoId = null);

        /// <summary>Id y nombre de los depósitos de la empresa, para selectores de destino.</summary>
        Task<HttpResp<List<DepositoOpcionDTO>?>> ObtenerOpcionesAsync(int? empresaId = null);

        Task<HttpResp<DepositoDTO?>> ObtenerPorIdAsync(int id);

        /// <summary>Devuelve el id creado o el error del servidor.</summary>
        Task<HttpResp<int>> CrearAsync(DepositoDTO deposito);

        Task<HttpResp<bool>> EditarAsync(int id, DepositoDTO deposito);

        /// <summary>Existencias (stock) del depósito, con el producto ya resuelto.</summary>
        Task<HttpResp<List<StockDepositoDTO>?>> ObtenerStockAsync(int id);
    }
}
