using IBUY.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    public class CotizacionRepositorio : Repositorio<Cotizacion>, ICotizacionRepositorio
    {
        public CotizacionRepositorio(AppDbContext context) : base(context)
        {
        }

        public async Task<Cotizacion> InsertarConItems(Cotizacion cotizacion, List<ItemCotizacion> items)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            await context.Set<Cotizacion>().AddAsync(cotizacion);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.CotizacionId = cotizacion.Id;
            }

            await context.Set<ItemCotizacion>().AddRangeAsync(items);
            await context.SaveChangesAsync();

            await tx.CommitAsync();

            return cotizacion;
        }

        public async Task<(Cotizacion? Cotizacion, List<ItemCotizacion> Items)> ObtenerDetalle(int id)
        {
            var cotizacion = await SelectById(id);
            if (cotizacion is null)
            {
                return (null, []);
            }

            var items = await context.Set<ItemCotizacion>()
                .Where(i => i.CotizacionId == id)
                .ToListAsync();

            return (cotizacion, items);
        }

        public async Task<List<Cotizacion>> ObtenerPorNotaPedido(int notaPedidoId)
        {
            return await context.Set<Cotizacion>()
                .Where(c => c.NotaPedidoId == notaPedidoId)
                .ToListAsync();
        }
    }
}
