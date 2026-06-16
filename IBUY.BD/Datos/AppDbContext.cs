using IBUY.BD.Datos.Entity;
using IBUY.Shared;
using Microsoft.EntityFrameworkCore;


namespace Proyecto2026.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Deposito> Depositos { get; set; }
    
        public DbSet<Producto> Productos { get; set; }
        
        public DbSet<Necesidad> Necesidades { get; set; }
        
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        
        public DbSet<ItemCotizacion> ItemsCotizaciones { get; set; }
        
        public DbSet<NotaPedido> NotasPedidos { get; set; }

        public DbSet<Stock> Stocks { get; set; }
           
        public DbSet<ItemNota> ItemsNotas { get; set; }


        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
         
    }
}
