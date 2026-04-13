using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Lógica de negocio para perfiles: creación con tope RN-201, progreso RN-704 y cálculo de nivel RN-703.
    /// </summary>
    public class PerfilService : IPerfilService
    {
        private readonly AppDbContext _context;

        private const int MaxPerfiles = 4;

        /// <summary>
        /// Crea el servicio con el contexto de datos inyectado.
        /// </summary>
        public PerfilService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PerfilDTO>> ObtenerPerfilesAsync()
        {
            var perfiles = await _context.Perfiles.ToListAsync();
            return perfiles.Select(MapearADTO).ToList();
        }

        /// <inheritdoc />
        public async Task<PerfilDTO?> ObtenerPerfilAsync(int id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            return perfil == null ? null : MapearADTO(perfil);
        }

        /// <inheritdoc />
        public async Task<PerfilDTO> CrearPerfilAsync(CrearPerfilDTO dto)
        {
            var totalPerfiles = await _context.Perfiles.CountAsync();
            if (totalPerfiles >= MaxPerfiles)
                throw new InvalidOperationException(
                    $"No se pueden crear más de {MaxPerfiles} perfiles. Elimina uno antes de continuar.");

            var perfilNuevo = new Perfil
            {
                NombreUsuario = dto.NombreUsuario,
                Monedas = 0,
                Experiencia = 0,
                Nivel = 1,
                EstrellaSostenibilidad = 0,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Perfiles.Add(perfilNuevo);
            await _context.SaveChangesAsync();

            return MapearADTO(perfilNuevo);
        }

        /// <inheritdoc />
        public async Task<PerfilDTO?> ActualizarProgresoAsync(int id, ActualizarProgresoDTO dto)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            if (perfil == null) return null;

            perfil.Monedas += dto.FichasGanadas;
            PerfilProgresoMath.AplicarExperiencia(perfil, dto.XpGanado);

            if (dto.EstrellasGanadas > 0)
                perfil.EstrellaSostenibilidad += dto.EstrellasGanadas;

            _context.Entry(perfil).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return MapearADTO(perfil);
        }

        /// <inheritdoc />
        public async Task<bool> EliminarPerfilAsync(int id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            if (perfil == null) return false;

            _context.Perfiles.Remove(perfil);
            await _context.SaveChangesAsync();
            return true;
        }

        private static PerfilDTO MapearADTO(Perfil p) => new PerfilDTO
        {
            Id = p.Id,
            NombreUsuario = p.NombreUsuario,
            Nivel = p.Nivel,
            Experiencia = p.Experiencia,
            Monedas = p.Monedas,
            EstrellaSostenibilidad = p.EstrellaSostenibilidad,
            FechaCreacion = p.FechaCreacion
        };
    }
}
