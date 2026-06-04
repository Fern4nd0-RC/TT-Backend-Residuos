using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Lógica del catálogo de partes de avatar (cuerpo, cara, sombrero).
    /// El catálogo es estático en BD (sembrado vía <c>HasData</c>), así que el servicio
    /// es prácticamente de lectura. Las únicas operaciones de negocio son resolver
    /// "qué IDs uso por defecto" y validar una asignación al crear/editar perfil.
    /// </summary>
    public class AvatarPartService : IAvatarPartService
    {
        private readonly AppDbContext _context;

        // Constantes de los nombres de slot. Centralizadas para no esparcir literales.
        public const string SlotBody = "Body";
        public const string SlotFace = "Face";
        public const string SlotHat  = "Hat";
        public const string SlotPfp  = "PFP";

        public AvatarPartService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<AvatarPartsCatalogoDTO> ObtenerCatalogoAsync()
        {
            var partes = await _context.AvatarParts
                .AsNoTracking()
                .OrderBy(p => p.Slot).ThenBy(p => p.Orden).ThenBy(p => p.Id)
                .ToListAsync();

            var catalogo = new AvatarPartsCatalogoDTO();
            foreach (var p in partes)
            {
                var dto = MapearADTO(p);
                switch (p.Slot)
                {
                    case SlotBody: catalogo.Bodies.Add(dto); break;
                    case SlotFace: catalogo.Faces.Add(dto); break;
                    case SlotHat:  catalogo.Hats.Add(dto); break;
                    case SlotPfp:  catalogo.ProfilePictures.Add(dto); break;
                    // Si aparece un slot inesperado se ignora silenciosamente: protege la API
                    // ante datos sucios en BD sin tumbar la respuesta.
                }
            }
            return catalogo;
        }

        /// <inheritdoc />
        public async Task<(int? bodyId, int? faceId, int? hatId, int? pfpId)> ObtenerDefaultsAsync()
        {
            // "Primero por Orden" da control al sembrado: lo que pongas con Orden=0
            // es el default. Útil para que el avatar nuevo arranque con un look concreto.
            var bodyId = await _context.AvatarParts
                .Where(p => p.Slot == SlotBody)
                .OrderBy(p => p.Orden).ThenBy(p => p.Id)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync();

            var faceId = await _context.AvatarParts
                .Where(p => p.Slot == SlotFace)
                .OrderBy(p => p.Orden).ThenBy(p => p.Id)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync();

            // Para sombrero, intentamos que el default sea "Sin sombrero" si existe
            // (lo identificamos por RecursoUnity vacío); si no, el de menor Orden.
            var hatId = await _context.AvatarParts
                .Where(p => p.Slot == SlotHat && p.RecursoUnity == string.Empty)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync();

            if (hatId == null)
            {
                hatId = await _context.AvatarParts
                    .Where(p => p.Slot == SlotHat)
                    .OrderBy(p => p.Orden).ThenBy(p => p.Id)
                    .Select(p => (int?)p.Id)
                    .FirstOrDefaultAsync();
            }

            var pfpId = await _context.AvatarParts
                    .Where(p => p.Slot == SlotPfp)
                    .OrderBy(p => p.Orden).ThenBy(p => p.Id)
                    .Select(p => (int?)p.Id)
                    .FirstOrDefaultAsync();

            return (bodyId, faceId, hatId, pfpId);
        }

        /// <inheritdoc />
        public async Task ValidarAsignacionAsync(int bodyId, int faceId, int hatId, int pfpId)
        {
            // Buscamos cada parte por clave primaria con FindAsync. EF traduce esto
            // siempre a un SELECT ... WHERE Id = @p, sin armar un WHERE IN sobre un
            // array local (que Pomelo/MySQL no logra traducir y lanzaba el error de
            // "LINQ query parameter expression").
            var body = await _context.AvatarParts.FindAsync(bodyId);
            var face = await _context.AvatarParts.FindAsync(faceId);
            var hat  = await _context.AvatarParts.FindAsync(hatId);
            var pfp  = await _context.AvatarParts.FindAsync(pfpId);

            ValidarSlot(body, bodyId, SlotBody, "cuerpo");
            ValidarSlot(face, faceId, SlotFace, "cara");
            ValidarSlot(hat,  hatId,  SlotHat,  "sombrero");
            ValidarSlot(pfp,  pfpId,  SlotPfp,  "imagen de perfil");
        }

        private static void ValidarSlot(AvatarPart? parte, int id, string slotEsperado, string nombreHumano)
        {
            if (parte == null)
                throw new InvalidOperationException(
                    $"La parte de avatar con Id {id} (para {nombreHumano}) no existe en el catálogo.");

            if (!string.Equals(parte.Slot, slotEsperado, StringComparison.Ordinal))
                throw new InvalidOperationException(
                    $"La parte con Id {id} es de tipo '{parte.Slot}', pero se esperaba '{slotEsperado}' ({nombreHumano}).");
        }

        private static AvatarPartDTO MapearADTO(AvatarPart p) => new()
        {
            Id = p.Id,
            Slot = p.Slot,
            Nombre = p.Nombre,
            RecursoUnity = p.RecursoUnity,
            Orden = p.Orden
        };
    }
}