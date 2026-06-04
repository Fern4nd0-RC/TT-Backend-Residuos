using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Models;
using ResiduosBackend.Services; // para AvatarPartService.SlotBody/Face/Hat

namespace ResiduosBackend.Data
{
    /// <summary>
    /// Contexto de Entity Framework Core para la base de datos del juego.
    /// </summary>
    public class AppDbContext : DbContext
    {
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

        /// <summary>Dispositivos autenticados.</summary>
        public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();

        /// <summary>Catálogo de partes del avatar (cuerpo, cara, sombrero).</summary>
        public DbSet<AvatarPart> AvatarParts => Set<AvatarPart>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Perfil>()
                .HasOne(p => p.Dispositivo)
                .WithMany(d => d.Perfiles)
                .HasForeignKey(p => p.DispositivoId)
                .OnDelete(DeleteBehavior.Cascade); // borrar dispositivo borra sus perfiles

            // ----------- FKs del avatar -----------
            // DeleteBehavior.Restrict: impide eliminar una parte del catálogo si algún
            // perfil la está usando. Lo correcto en este dominio: el catálogo es estable
            // y borrar una parte "en uso" rompería avatares en producción.
            modelBuilder.Entity<Perfil>()
                .HasOne(p => p.BodyPart)
                .WithMany()
                .HasForeignKey(p => p.BodyPartId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Perfil>()
                .HasOne(p => p.FacePart)
                .WithMany()
                .HasForeignKey(p => p.FacePartId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Perfil>()
                .HasOne(p => p.HatPart)
                .WithMany()
                .HasForeignKey(p => p.HatPartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice por slot acelera ObtenerCatalogoAsync y los filtros de defaults.
            modelBuilder.Entity<AvatarPart>()
                .HasIndex(a => new { a.Slot, a.Orden });

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

            // ============================================================
            // SEMBRADO DEL CATÁLOGO DE AVATAR
            // ============================================================
            // Los IDs son fijos por convención:
            //   1xx -> Body, 2xx -> Face, 3xx -> Hat
            // Así es trivial leer dumps de BD y debugar a ojo qué fila es qué slot.
            //
            // El campo RecursoUnity DEBE coincidir EXACTAMENTE con el nombre del
            // material o prefab dentro de Resources/ en el proyecto Unity.
            // Si renombras un material en Unity, actualiza la fila correspondiente.
            //
            // TODO: confirmar nombres exactos contra el pack. Los que pongo abajo se
            // basan en lo que leí de las capturas (patrón "Color N Variante"). Si en
            // tu proyecto un material se llama distinto, edita el RecursoUnity antes
            // de generar la migración o agrega/quita filas a tu gusto.
            SembrarAvatarParts(modelBuilder);
        }

        private static void SembrarAvatarParts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvatarPart>().HasData(
                // ---------------- BODIES (100..) ----------------
                // Por convención el primero (menor Orden) es el default al crear perfil.
                new AvatarPart { Id = 101, Slot = AvatarPartService.SlotBody, Nombre = "Azul Base",     RecursoUnity = "Blue 2 Base",   Orden = 0 },
                new AvatarPart { Id = 102, Slot = AvatarPartService.SlotBody, Nombre = "Azul Oscuro",   RecursoUnity = "Blue 1 Dark",   Orden = 1 },
                new AvatarPart { Id = 103, Slot = AvatarPartService.SlotBody, Nombre = "Azul Claro",    RecursoUnity = "Blue 3 Light",  Orden = 2 },
                new AvatarPart { Id = 104, Slot = AvatarPartService.SlotBody, Nombre = "Rojo Base",     RecursoUnity = "Red 2 Base",    Orden = 3 },
                new AvatarPart { Id = 105, Slot = AvatarPartService.SlotBody, Nombre = "Rojo Oscuro",   RecursoUnity = "Red 1 Dark",    Orden = 4 },
                new AvatarPart { Id = 106, Slot = AvatarPartService.SlotBody, Nombre = "Rojo Claro",    RecursoUnity = "Red 3 Light",   Orden = 5 },
                new AvatarPart { Id = 107, Slot = AvatarPartService.SlotBody, Nombre = "Verde Base",    RecursoUnity = "Green 2 Base",  Orden = 6 },
                new AvatarPart { Id = 108, Slot = AvatarPartService.SlotBody, Nombre = "Amarillo Base", RecursoUnity = "Yellow 2 Base", Orden = 7 },
                new AvatarPart { Id = 109, Slot = AvatarPartService.SlotBody, Nombre = "Morado Base",   RecursoUnity = "Purple 2 Base", Orden = 8 },
                new AvatarPart { Id = 110, Slot = AvatarPartService.SlotBody, Nombre = "Rosa Base",     RecursoUnity = "Pink 2 Base",   Orden = 9 },
                new AvatarPart { Id = 111, Slot = AvatarPartService.SlotBody, Nombre = "Naranja Base",  RecursoUnity = "Orange 2 Base", Orden = 10 },
                new AvatarPart { Id = 112, Slot = AvatarPartService.SlotBody, Nombre = "Cian Base",     RecursoUnity = "Cyan 2 Base",   Orden = 11 },
                new AvatarPart { Id = 113, Slot = AvatarPartService.SlotBody, Nombre = "Turquesa Base", RecursoUnity = "Turquoise 2 Base", Orden = 12 },
                new AvatarPart { Id = 114, Slot = AvatarPartService.SlotBody, Nombre = "Café Base",     RecursoUnity = "Brown 2 Base",  Orden = 13 },
                new AvatarPart { Id = 115, Slot = AvatarPartService.SlotBody, Nombre = "Crema Base",    RecursoUnity = "Cream 2 Base",  Orden = 14 },
                new AvatarPart { Id = 116, Slot = AvatarPartService.SlotBody, Nombre = "Gris Base",     RecursoUnity = "Grey 2 Base",   Orden = 15 },

                // ---------------- FACES (200..) ----------------
                new AvatarPart { Id = 201, Slot = AvatarPartService.SlotFace, Nombre = "Feliz",   RecursoUnity = "face 1", Orden = 0 },
                new AvatarPart { Id = 202, Slot = AvatarPartService.SlotFace, Nombre = "Enojado", RecursoUnity = "face 2", Orden = 1 },
                new AvatarPart { Id = 203, Slot = AvatarPartService.SlotFace, Nombre = "Triste",  RecursoUnity = "face 3", Orden = 2 },

                // ---------------- HATS (300..) ----------------
                // 301 (Orden=0, RecursoUnity vacío) es la opción "Sin sombrero". Default.
                new AvatarPart { Id = 301, Slot = AvatarPartService.SlotHat, Nombre = "Sin sombrero",      RecursoUnity = "",            Orden = 0 },
                new AvatarPart { Id = 302, Slot = AvatarPartService.SlotHat, Nombre = "Sombrero de chef",  RecursoUnity = "chef hat",    Orden = 1 },
                new AvatarPart { Id = 303, Slot = AvatarPartService.SlotHat, Nombre = "Sombrero de fiesta", RecursoUnity = "party hat",  Orden = 2 },
                new AvatarPart { Id = 304, Slot = AvatarPartService.SlotHat, Nombre = "Sombrero naranja",   RecursoUnity = "orange fedora", Orden = 3 },
                

                // ---------------- PROFILE PICTURES (400..) ----------------
                // El RecursoUnity aquí correspondería al nombre del Sprite dentro de la carpeta Resources en Unity (si decides cargarlos dinámicamente) 
                // o simplemente un identificador de texto si usas un ScriptableObject fijo.
                new AvatarPart { Id = 401, Slot = AvatarPartService.SlotPfp, Nombre = "PFP Oso", RecursoUnity = "pfp_bear", Orden = 0 },
                new AvatarPart { Id = 402, Slot = AvatarPartService.SlotPfp, Nombre = "PFP Pollo",  RecursoUnity = "pfp_chicken", Orden = 1 },
                new AvatarPart { Id = 403, Slot = AvatarPartService.SlotPfp, Nombre = "PFP Koala", RecursoUnity = "pfp_koala", Orden = 2 },
                new AvatarPart { Id = 404, Slot = AvatarPartService.SlotPfp, Nombre = "PFP Suricata", RecursoUnity = "pfp_meerkat", Orden = 3 },
                new AvatarPart { Id = 405, Slot = AvatarPartService.SlotPfp, Nombre = "PFP Panda", RecursoUnity = "pfp_panda", Orden = 4 }
            );
        }
    }
}
