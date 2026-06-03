using Microsoft.EntityFrameworkCore;


namespace Proyecto2026.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}