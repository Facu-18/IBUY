using IBUY.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    /// <summary>Se intenta recibir un remito que ya estaba Recibido.</summary>
    public class RemitoYaRecibidoException : Exception
    {
        public RemitoYaRecibidoException(int remitoId)
            : base($"El remito de id: {remitoId} ya fue recibido.")
        {
        }
    }

    public class RemitoRepositorio : Repositorio<Remito>, IRemitoRepositorio
    {
        public RemitoRepositorio(AppDbContext context) : base(context)
        {
        }

        public async Task<Remito> RegistrarMovimiento(Remito remito, List<ItemRemito> items)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            // El Numero depende del Id, que recien existe tras insertar: se inserta con un
            // valor provisorio unico y se reemplaza dentro de la misma transaccion.
            remito.Numero = "T" + Guid.NewGuid().ToString("N")[..12];
            await context.Set<Remito>().AddAsync(remito);
            await context.SaveChangesAsync();

            remito.Numero = $"R-{remito.Id:D4}";

            foreach (var item in items)
            {
                item.RemitoId = remito.Id;
            }

            await context.Set<ItemRemito>().AddRangeAsync(items);
            await context.SaveChangesAsync();

            // Solo el Interno descuenta el origen; la mercaderia queda en transito hasta
            // que se recibe. El Externo no mueve stock al emitirse.
            if (remito.Tipo == "Interno")
            {
                foreach (var item in items)
                {
                    await DecrementarStock(remito.DepositoOrigenId!.Value, item.ProductoId, item.Cantidad);
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

            // El cambio de estado es condicional (Estado = 'Emitido') y atomico: si dos
            // recepciones compiten, solo una afecta la fila y la otra falla sin sumar stock.
            int filas = await context.Set<Remito>()
                .Where(r => r.Id == id && r.Estado == "Emitido")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Estado, "Recibido")
                    .SetProperty(r => r.FechaRecepcion, fechaRecepcion));
            if (filas == 0)
            {
                throw new RemitoYaRecibidoException(id);
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
            var remito = await context.Set<Remito>()
                .Include(r => r.DepositoOrigen)
                .Include(r => r.DepositoDestino)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (remito is null)
            {
                return (null, []);
            }

            var items = await context.Set<ItemRemito>()
                .Include(i => i.Producto)
                .Where(i => i.RemitoId == id)
                .ToListAsync();

            return (remito, items);
        }

        public async Task<List<Remito>> ObtenerListado()
        {
            return await context.Set<Remito>()
                .Include(r => r.DepositoOrigen)
                .Include(r => r.DepositoDestino)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        private async Task IncrementarStock(int depositoId, int productoId, decimal cantidad)
        {
            // Se mira primero lo ya agregado sin guardar: un remito puede repetir el producto
            // y no debe crear dos filas de stock para el mismo (deposito, producto).
            var stock = context.Set<Stock>().Local
                .FirstOrDefault(s => s.DepositoId == depositoId && s.ProductoId == productoId)
                ?? await context.Set<Stock>()
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
