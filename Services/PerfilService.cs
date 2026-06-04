using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Lógica de negocio para perfiles: creación con tope RN-201 por dispositivo,
    /// progreso RN-704, cálculo de nivel RN-703 e inicialización del avatar con el
    /// catálogo de <see cref="AvatarPart"/>.
    /// </summary>
    public class PerfilService : IPerfilService
    {
        private readonly AppDbContext _context;
        private readonly ILogroService _logroService;
        private readonly IAvatarPartService _avatarPartService;

        private const int MaxPerfiles = 4;

        public PerfilService(AppDbContext context, ILogroService logroService, IAvatarPartService avatarPartService)
        {
            _context = context;
            _logroService = logroService;
            _avatarPartService = avatarPartService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PerfilDTO>> ObtenerPerfilesPorDispositivoAsync(string deviceId)
        {
            var perfiles = await _context.Perfiles
                .Where(p => p.DispositivoId == deviceId)
                .ToListAsync();
            return perfiles.Select(MapearADTO).ToList();
        }

        /// <inheritdoc />
        public async Task<PerfilDTO?> ObtenerPerfilAsync(int id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            return perfil == null ? null : MapearADTO(perfil);
        }

        /// <inheritdoc />
        public async Task<PerfilDTO> CrearPerfilAsync(string deviceId, CrearPerfilDTO dto)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new InvalidOperationException("Falta el identificador del dispositivo.");

            // Registrar el dispositivo si es la primera vez que se ve este GUID.
            var dispositivo = await _context.Dispositivos.FindAsync(deviceId);
            if (dispositivo == null)
            {
                dispositivo = new Dispositivo
                {
                    Id = deviceId,
                    FechaRegistro = DateTime.UtcNow
                };
                _context.Dispositivos.Add(dispositivo);
            }

            // RN-201: máximo cuatro perfiles POR DISPOSITIVO.
            var totalPerfiles = await _context.Perfiles
                .CountAsync(p => p.DispositivoId == deviceId);
            if (totalPerfiles >= MaxPerfiles)
                throw new InvalidOperationException(
                    $"No se pueden crear más de {MaxPerfiles} perfiles en este dispositivo. Elimina uno antes de continuar.");

            // ---- Resolución de las partes del avatar ----
            // Si el cliente no envió IDs (0), usamos los defaults del catálogo.
            // Si los envió, validamos que existan y sean del slot correcto.
            int bodyId = dto.BodyPartId;
            int faceId = dto.FacePartId;
            int hatId  = dto.HatPartId;
            int pfpId  = dto.ProfilePictureId;

            if (bodyId == 0 || faceId == 0 || hatId == 0 || pfpId == 0)
            {
                var (defBody, defFace, defHat, defPfp) = await _avatarPartService.ObtenerDefaultsAsync();
                if (defBody == null || defFace == null || defHat == null || defPfp == null)
                    throw new InvalidOperationException(
                        "El catálogo de partes de avatar no está inicializado. Aplica la migración con HasData.");

                if (bodyId == 0) bodyId = defBody.Value;
                if (faceId == 0) faceId = defFace.Value;
                if (hatId  == 0) hatId  = defHat.Value;
            }

            // Validación final (siempre, aunque vengan de defaults): no se cuela un
            // ID inválido a la FK. Lanza InvalidOperationException si algo no cuadra.
            await _avatarPartService.ValidarAsignacionAsync(bodyId, faceId, hatId, pfpId);

            var perfilNuevo = new Perfil
            {
                DispositivoId = deviceId,
                NombreUsuario = dto.NombreUsuario,
                Monedas = 0,
                Experiencia = 0,
                Nivel = 1,
                EstrellaSostenibilidad = 0,
                BodyPartId = bodyId,
                FacePartId = faceId,
                HatPartId = hatId,
                ProfilePictureId = pfpId,
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
            await _logroService.VerificarYDesbloquearLogrosAsync(id);

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
            BodyPartId = p.BodyPartId,
            FacePartId = p.FacePartId,
            HatPartId = p.HatPartId,
            ProfilePictureId = p.ProfilePictureId,
            FechaCreacion = p.FechaCreacion
        };
    }
}
