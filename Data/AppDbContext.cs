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

        /// <summary>Catálogo de insignias desbloqueables.</summary>
        public DbSet<Logro> Logros { get; set; }

        /// <summary>Relación perfil–logro desbloqueado.</summary>
        public DbSet<PerfilLogro> PerfilLogros { get; set; }

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

            modelBuilder.Entity<PerfilLogro>()
                .HasKey(pl => new { pl.PerfilId, pl.LogroId });

            modelBuilder.Entity<PerfilLogro>()
                .HasOne(pl => pl.Perfil)
                .WithMany(p => p.LogrosDesbloqueados)
                .HasForeignKey(pl => pl.PerfilId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PerfilLogro>()
                .HasOne(pl => pl.Logro)
                .WithMany(l => l.PerfilesLogro)
                .HasForeignKey(pl => pl.LogroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Logro>().HasData(
                new Logro
                {
                    Id = 1,
                    Nombre = "Reciclador Novato",
                    Descripcion = "Alcanza 100 XP acumulada.",
                    ImagenUrl = "logros/reciclador-novato.png",
                    RequisitoXP = 100
                },
                new Logro
                {
                    Id = 2,
                    Nombre = "Guardian del Planeta",
                    Descripcion = "Alcanza 500 XP acumulada.",
                    ImagenUrl = "logros/guardian-planeta.png",
                    RequisitoXP = 500
                },
                new Logro
                {
                    Id = 3,
                    Nombre = "Maestro del Reciclaje",
                    Descripcion = "Alcanza 1000 XP acumulada.",
                    ImagenUrl = "logros/maestro-reciclaje.png",
                    RequisitoXP = 1000
                }
            );
        }
    }
}
