using IBUY.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    public class NecesidadRepositorio : Repositorio<Necesidad>, INecesidadRepositorio
    {
        public NecesidadRepositorio(AppDbContext context) : base(context)
        {
        }

        public async Task<decimal> ObtenerStockDisponible(int depositoId, int productoId)
        {
            var stock = await context.Set<Stock>()
                .FirstOrDefaultAsync(s => s.DepositoId == depositoId && s.ProductoId == productoId);

            return stock?.CantidadActual ?? 0;
        }
    }
}
