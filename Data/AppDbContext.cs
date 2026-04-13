using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Models;

namespace ResiduosBackend.Data
{
    /// <summary>
    /// Contexto de Entity Framework Core para la base de datos del juego.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Inicializa el contexto con las opciones registradas al configurar la aplicación.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>Conjunto de perfiles de jugador.</summary>
        public DbSet<Perfil> Perfiles { get; set; }

        /// <summary>Catálogo de ítems (tienda e inventario).</summary>
        public DbSet<Item> Items { get; set; }

        /// <summary>Registros de inventario por perfil (ítem apilado).</summary>
        public DbSet<Inventario> Inventarios { get; set; }

        /// <summary>Residuos para la enciclopedia y la mecánica de clasificación.</summary>
        public DbSet<Residuo> Residuos { get; set; }

        /// <summary>Relación perfil–residuo desbloqueado en enciclopedia (RF-602).</summary>
        public DbSet<EnciclopediaDesbloqueo> EnciclopediaDesbloqueos { get; set; }

        /// <summary>Métricas históricas por partida finalizada.</summary>
        public DbSet<PartidaMetrica> PartidaMetricas { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Clave compuesta obligatoria: sin ella EF Core no modela correctamente la tabla de unión y fallan las migraciones.
            modelBuilder.Entity<EnciclopediaDesbloqueo>()
                .HasKey(e => new { e.PerfilId, e.ResiduoId });

            modelBuilder.Entity<EnciclopediaDesbloqueo>()
                .HasOne(e => e.Perfil)
                .WithMany()
                .HasForeignKey(e => e.PerfilId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EnciclopediaDesbloqueo>()
                .HasOne(e => e.Residuo)
                .WithMany(r => r.Desbloqueos)
                .HasForeignKey(e => e.ResiduoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un único registro (PerfilId, ItemId) fuerza el apilado en Cantidad y evita filas duplicadas del mismo ítem.
            modelBuilder.Entity<Inventario>()
                .HasIndex(i => new { i.PerfilId, i.ItemId })
                .IsUnique();
        }
    }
}
