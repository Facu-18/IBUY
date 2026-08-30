using IBUY.Shared.DTO;

namespace IBUY.Servicios
{
    public interface IProductoServicio
    {
        Task<List<ProductoDTO>> ObtenerTodosAsync();
    }
}
