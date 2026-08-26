using IBUY.Shared.DTO;

namespace IBUY.Servicios.Interfaces
{
    /// <summary>
    /// Operaciones comunes a todos los servicios simulados: consultar, crear y actualizar
    /// en memoria. Todas devuelven Task para que las pantallas usen el mismo codigo
    /// que van a usar cuando detras haya HTTP real.
    /// </summary>
    public interface IServicioMock<T> where T : DtoBase
    {
        Task<List<T>> ObtenerTodosAsync();

        Task<T?> ObtenerPorIdAsync(int id);

        Task<T> CrearAsync(T dto);

        Task<bool> ActualizarAsync(int id, T dto);
    }
}
