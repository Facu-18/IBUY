using IBUY.BD.Datos.Entity;
using IBUY.Shared;
using Microsoft.EntityFrameworkCore;


namespace Proyecto2026.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        
    }
}