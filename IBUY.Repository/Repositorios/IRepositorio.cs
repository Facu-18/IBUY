using Proyecto2026.BD.Datos;

namespace IBUY.Repository.Repositorios
{
    public interface IRepositorio<E> where E : class, IEntityBase
    {
            Task<bool> Delete(int id);
            Task<bool> Existe(int id);
            Task<E> Insert(E entity);
            Task<List<E>> Select();
            Task<E?> SelectById(int id);
            Task<bool> Update(E entity);
    }
}