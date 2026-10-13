using IBUY.BD.Datos.Entity;

namespace IBUY.Repository.Repositorios
{
    public interface INecesidadRepositorio : IRepositorio<Necesidad>
    {
        /// <summary>Cantidad actual de un producto en un depósito (0 si no hay fila de stock).</summary>
        Task<decimal> ObtenerStockDisponible(int depositoId, int productoId);
    }
}
