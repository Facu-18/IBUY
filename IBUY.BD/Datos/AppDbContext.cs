using IBUY.Shared;
using Microsoft.EntityFrameworkCore;


namespace Proyecto2026.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Empresa> Empresa { get; set; }
       
        public DbSet<Usuario> Usuario { get; set; }
    }
}