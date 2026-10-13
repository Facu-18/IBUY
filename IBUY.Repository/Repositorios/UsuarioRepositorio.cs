using IBUY.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        public UsuarioRepositorio(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario> InsertarConDepositos(Usuario usuario, List<int> depositoIds)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            await context.Set<Usuario>().AddAsync(usuario);
            await context.SaveChangesAsync();

            var depositos = await context.Set<Deposito>()
                .Where(d => depositoIds.Contains(d.Id))
                .ToListAsync();

            foreach (var deposito in depositos)
            {
                deposito.UsuarioResponsableId = usuario.Id;
            }

            await context.SaveChangesAsync();

            await tx.CommitAsync();

            return usuario;
        }

        public async Task<bool> ActualizarConDepositos(Usuario usuario, List<int> depositoIds)
        {
            if (!await Existe(usuario.Id))
            {
                return false;
            }

            await using var tx = await context.Database.BeginTransactionAsync();

            context.Set<Usuario>().Update(usuario);
            await context.SaveChangesAsync();

            var depositosActuales = await context.Set<Deposito>()
                .Where(d => d.UsuarioResponsableId == usuario.Id)
                .ToListAsync();

            foreach (var deposito in depositosActuales.Where(d => !depositoIds.Contains(d.Id)))
            {
                deposito.UsuarioResponsableId = null;
            }

            var depositosNuevos = await context.Set<Deposito>()
                .Where(d => depositoIds.Contains(d.Id))
                .ToListAsync();

            foreach (var deposito in depositosNuevos)
            {
                deposito.UsuarioResponsableId = usuario.Id;
            }

            await context.SaveChangesAsync();

            await tx.CommitAsync();

            return true;
        }

        public async Task<List<int>> ObtenerDepositoIds(int usuarioId)
        {
            return await context.Set<Deposito>()
                .Where(d => d.UsuarioResponsableId == usuarioId)
                .Select(d => d.Id)
                .ToListAsync();
        }
    }
}
