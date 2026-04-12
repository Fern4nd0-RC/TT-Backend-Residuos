using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Models;

namespace ResiduosBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tus tablas en la base de datos
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        
        // (Agrega las demás tablas como PartidaMetrica o EnciclopediaDesbloqueo cuando llegues a ellas)
    }
}