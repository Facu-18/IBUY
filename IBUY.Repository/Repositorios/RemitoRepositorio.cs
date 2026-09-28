using IBUY.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    public class RemitoRepositorio : Repositorio<Remito>, IRemitoRepositorio
    {
        public RemitoRepositorio(AppDbContext context) : base(context)
        {
        }

        public async Task<Remito> RegistrarMovimiento(Remito remito, List<ItemRemito> items)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            await context.Set<Remito>().AddAsync(remito);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.RemitoId = remito.Id;
            }

            await context.Set<ItemRemito>().AddRangeAsync(items);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                switch (remito.Tipo)
                {
                    case "Entrada":
                        await IncrementarStock(remito.DepositoDestinoId!.Value, item.ProductoId, item.Cantidad);
                        break;
                    case "Salida":
                        await DecrementarStock(remito.DepositoOrigenId!.Value, item.ProductoId, item.Cantidad);
                        break;
                    case "Transferencia":
                        // La transferencia solo descuenta el origen aca; la mercaderia
                        // queda en transito hasta que el destino la recepciona.
                        await DecrementarStock(remito.DepositoOrigenId!.Value, item.ProductoId, item.Cantidad);
                        break;
                }
            }

            await context.SaveChangesAsync();
            await tx.CommitAsync();

            return remito;
        }

        public async Task<Remito?> RegistrarRecepcion(int id, DateTime fechaRecepcion)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            var remito = await context.Set<Remito>().FirstOrDefaultAsync(r => r.Id == id);
            if (remito is null)
            {
                return null;
            }

            // La recepcion es total: no hay recepcion parcial de cantidades. Si llega
            // mercaderia dañada o de menos, el destino abre una Necesidad de vuelta al
            // remitente en vez de ajustar este remito.
            var items = await context.Set<ItemRemito>()
                .Where(i => i.RemitoId == id)
                .ToListAsync();

            foreach (var item in items)
            {
                await IncrementarStock(remito.DepositoDestinoId!.Value, item.ProductoId, item.Cantidad);
            }

            remito.Estado = "Recibido";
            remito.FechaRecepcion = fechaRecepcion;

            await context.SaveChangesAsync();
            await tx.CommitAsync();

            return remito;
        }

        public async Task<(Remito? Remito, List<ItemRemito> Items)> ObtenerDetalle(int id)
        {
            var remito = await SelectById(id);
            if (remito is null)
            {
                return (null, []);
            }

            var items = await context.Set<ItemRemito>()
                .Where(i => i.RemitoId == id)
                .ToListAsync();

            return (remito, items);
        }

        private async Task IncrementarStock(int depositoId, int productoId, decimal cantidad)
        {
            var stock = await context.Set<Stock>()
                .FirstOrDefaultAsync(s => s.DepositoId == depositoId && s.ProductoId == productoId);

            if (stock is null)
            {
                stock = new Stock { DepositoId = depositoId, ProductoId = productoId, CantidadActual = 0 };
                await context.Set<Stock>().AddAsync(stock);
            }

            stock.CantidadActual += cantidad;
        }

        private async Task DecrementarStock(int depositoId, int productoId, decimal cantidad)
        {
            var stock = await context.Set<Stock>()
                .FirstOrDefaultAsync(s => s.DepositoId == depositoId && s.ProductoId == productoId);

            if (stock is null || stock.CantidadActual < cantidad)
            {
                throw new StockInsuficienteException(depositoId, productoId);
            }

            stock.CantidadActual -= cantidad;
        }
    }
}
