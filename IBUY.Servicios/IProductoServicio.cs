using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>Productos reales contra la API (api/producto), para combos.</summary>
    public interface IProductoServicio
    {
        /// <summary>Productos de la empresa indicada; si no se informa, devuelve todos.</summary>
        Task<HttpResp<List<ProductoOpcionDTO>?>> ObtenerTodosAsync(int? empresaId);
    }
}
