using IBUY.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    public class NotaPedidoRepositorio : Repositorio<NotaPedido>, INotaPedidoRepositorio
    {
        public NotaPedidoRepositorio(AppDbContext context) : base(context)
        {
        }

        public async Task<NotaPedido> InsertarConItems(NotaPedido notaPedido, List<ItemNota> items)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            await context.Set<NotaPedido>().AddAsync(notaPedido);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.NotaPedidoId = notaPedido.Id;
            }

            await context.Set<ItemNota>().AddRangeAsync(items);
            await context.SaveChangesAsync();

            await tx.CommitAsync();

            return notaPedido;
        }

        public async Task<NotaPedido> ActualizarConItems(NotaPedido notaPedido, List<ItemNota> items)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            context.Set<NotaPedido>().Update(notaPedido);

            var existentes = await context.Set<ItemNota>()
                .Where(i => i.NotaPedidoId == notaPedido.Id)
                .ToListAsync();

            context.Set<ItemNota>().RemoveRange(existentes);

            foreach (var item in items)
            {
                item.NotaPedidoId = notaPedido.Id;
            }

            await context.Set<ItemNota>().AddRangeAsync(items);
            await context.SaveChangesAsync();

            await tx.CommitAsync();

            return notaPedido;
        }

        public async Task<(NotaPedido? NotaPedido, List<ItemNota> Items)> ObtenerDetalle(int id)
        {
            var notaPedido = await SelectById(id);
            if (notaPedido is null)
            {
                return (null, []);
            }

            var items = await context.Set<ItemNota>()
                .Where(i => i.NotaPedidoId == id)
                .ToListAsync();

            return (notaPedido, items);
        }

        public async Task<List<NotaPedido>> ObtenerPorNecesidad(int necesidadId)
        {
            return await context.Set<NotaPedido>()
                .Where(n => n.NecesidadId == necesidadId)
                .ToListAsync();
        }

        public Task<bool> TieneCotizaciones(int notaPedidoId)
        {
            return context.Set<Cotizacion>().AnyAsync(c => c.NotaPedidoId == notaPedidoId);
        }

        public async Task<bool> EliminarConItems(int notaPedidoId)
        {
            var notaPedido = await SelectById(notaPedidoId);
            if (notaPedido is null)
            {
                return false;
            }

            await using var tx = await context.Database.BeginTransactionAsync();

            var items = await context.Set<ItemNota>()
                .Where(i => i.NotaPedidoId == notaPedidoId)
                .ToListAsync();

            context.Set<ItemNota>().RemoveRange(items);
            context.Set<NotaPedido>().Remove(notaPedido);

            await context.SaveChangesAsync();
            await tx.CommitAsync();

            return true;
        }
    }
}
