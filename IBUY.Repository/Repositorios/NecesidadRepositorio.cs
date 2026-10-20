using IBUY.BD.Datos.Entity;
using IBUY.Shared.DTO;
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

        public async Task<bool> ExistePendienteDuplicada(int depositoSolicitanteId, int depositoDestinoId, int productoId, int? excluirId = null)
        {
            return await context.Set<Necesidad>().AnyAsync(n =>
                n.Estado == EstadosNecesidad.Pendiente
                && n.DepositoSolicitanteId == depositoSolicitanteId
                && n.DepositoDestinoId == depositoDestinoId
                && n.ProductoId == productoId
                && (!excluirId.HasValue || n.Id != excluirId.Value));
        }

        public async Task<List<Necesidad>> ObtenerDeEmpresa(int empresaId, string? estado, int? depositoId)
        {
            var depositosDeEmpresa = await ObtenerDepositosDeEmpresa(empresaId);
            var ids = depositosDeEmpresa.Select(d => d.Id).ToHashSet();

            var necesidades = await context.Set<Necesidad>()
                .Where(n => ids.Contains(n.DepositoDestinoId) || ids.Contains(n.DepositoSolicitanteId))
                .ToListAsync();

            if (depositoId.HasValue)
            {
                necesidades = necesidades
                    .Where(n => n.DepositoDestinoId == depositoId.Value || n.DepositoSolicitanteId == depositoId.Value)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                necesidades = necesidades.Where(n => n.Estado == estado).ToList();
            }

            return necesidades;
        }

        public async Task<List<Deposito>> ObtenerDepositosDeEmpresa(int empresaId)
        {
            return await context.Set<Deposito>()
                .Where(d => d.EmpresaId == empresaId)
                .ToListAsync();
        }

        public async Task<List<Stock>> ObtenerStockDeDeposito(int depositoId)
        {
            return await context.Set<Stock>()
                .Where(s => s.DepositoId == depositoId)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> ObtenerNotasPorNecesidad()
        {
            return await context.Set<NotaPedido>()
                .Where(n => n.NecesidadId.HasValue)
                .GroupBy(n => n.NecesidadId!.Value)
                .Select(g => new { NecesidadId = g.Key, NotaPedidoId = g.Min(n => n.Id) })
                .ToDictionaryAsync(x => x.NecesidadId, x => x.NotaPedidoId);
        }

        public async Task<NotaPedido> GenerarNotaPedido(Necesidad necesidad, NotaPedido notaPedido)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            notaPedido.NecesidadId = necesidad.Id;
            await context.Set<NotaPedido>().AddAsync(notaPedido);
            await context.SaveChangesAsync();

            // El producto y la cantidad salen de la necesidad: el usuario no los carga.
            await context.Set<ItemNota>().AddAsync(new ItemNota
            {
                NotaPedidoId = notaPedido.Id,
                ProductoId = necesidad.ProductoId,
                CantidadSolicitada = necesidad.CantidadRequerida
            });

            necesidad.Estado = EstadosNecesidad.EnNota;
            context.Set<Necesidad>().Update(necesidad);

            await context.SaveChangesAsync();
            await tx.CommitAsync();

            return notaPedido;
        }
    }
}
