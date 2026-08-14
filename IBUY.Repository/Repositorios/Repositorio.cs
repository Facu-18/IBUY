using Proyecto2026.BD.Datos;
using Microsoft.EntityFrameworkCore;


namespace IBUY.Repository.Repositorios
{
    public class Repositorio<E> : IRepositorio<E> where E : class, IEntityBase
    {

        private readonly AppDbContext context;
        public Repositorio(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<E>> Select()
        {
            return await context.Set<E>().ToListAsync();
        }

        public async Task<E?> SelectById(int id)
        {
            return await context.Set<E>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<E> Insert(E entity)
        {
            await context.Set<E>().AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;

        }

        public async Task<bool> Existe(int id)
        {
            bool existe = await context.Set<E>().AnyAsync(x => x.Id == id);
            return existe;
        }

        public async Task<bool> Update(E entity)
        {
            if (!await Existe(entity.Id))
            {
                return false;
            }

            context.Set<E>().Update(entity);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            if (!await Existe(id))
            {
                return false;
            }

            var entidad = await context.Set<E>().FirstAsync(x => x.Id == id);
            context.Set<E>().Remove(entidad);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
