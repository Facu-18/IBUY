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
    }
}
