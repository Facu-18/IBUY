using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    public interface IProductoServicio
    {
        Task<HttpResp<List<ProductoDTO>?>> ObtenerTodosAsync(int empresaId);

        Task<HttpResp<ProductoDTO?>> ObtenerPorIdAsync(int id, int empresaId);

        Task<HttpResp<int>> CrearAsync(ProductoDTO producto);

        Task<HttpResp<bool>> EditarAsync(int id, ProductoDTO producto);
    }
}
