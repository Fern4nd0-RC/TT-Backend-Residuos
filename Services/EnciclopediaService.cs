using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Enciclopedia: catálogo con enmascaramiento RN-601, detalle autorizado y altas idempotentes de desbloqueo.
    /// </summary>
    public class EnciclopediaService : IEnciclopediaService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Crea el servicio con el contexto de datos inyectado.
        /// </summary>
        public EnciclopediaService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EnciclopediaEntradaDTO>> ObtenerCatalogoAsync(int perfilId)
        {
            var perfilExiste = await _context.Perfiles.AnyAsync(p => p.Id == perfilId);
            if (!perfilExiste)
                throw new KeyNotFoundException($"No se encontró el perfil con ID {perfilId}.");

            var desbloqueos = await _context.EnciclopediaDesbloqueos
                .Where(d => d.PerfilId == perfilId)
                .ToDictionaryAsync(d => d.ResiduoId, d => d.FechaDesbloqueo);

            var residuos = await _context.Residuos.ToListAsync();

            return residuos.Select(r =>
            {
                var desbloqueado = desbloqueos.TryGetValue(r.Id, out var fecha);
                return new EnciclopediaEntradaDTO
                {
                    Id = r.Id,
                    Nombre = desbloqueado ? r.Nombre : string.Empty,
                    Categoria = r.Categoria,
                    NombreSprite = desbloqueado ? r.NombreSprite : string.Empty,
                    Desbloqueado = desbloqueado,
                    FechaDesbloqueo = desbloqueado ? fecha : null
                };
            });
        }

        /// <inheritdoc />
        public async Task<EnciclopediaDetalleDTO> ObtenerDetalleAsync(int perfilId, int residuoId)
        {
            var desbloqueo = await _context.EnciclopediaDesbloqueos
                .Include(d => d.Residuo)
                .FirstOrDefaultAsync(d => d.PerfilId == perfilId && d.ResiduoId == residuoId);

            if (desbloqueo == null)
                throw new UnauthorizedAccessException(
                    "Este residuo aún no ha sido descubierto. ¡Sigue jugando para desbloquearlo!");

            var r = desbloqueo.Residuo!;
            return new EnciclopediaDetalleDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Categoria = r.Categoria,
                Subcategoria = r.Subcategoria,
                DescripcionParaNinos = r.DescripcionParaNinos,
                DatoCurioso = r.DatoCurioso,
                NombreSprite = r.NombreSprite,
                FechaDesbloqueo = desbloqueo.FechaDesbloqueo
            };
        }

        /// <inheritdoc />
        public async Task<DesbloqueoResultadoDTO> DesbloquearResiduoAsync(DesbloquearResiduoDTO dto)
        {
            var perfilExiste = await _context.Perfiles.AnyAsync(p => p.Id == dto.PerfilId);
            if (!perfilExiste)
                throw new KeyNotFoundException($"No se encontró el perfil con ID {dto.PerfilId}.");

            var residuo = await _context.Residuos.FindAsync(dto.Id)
                ?? throw new KeyNotFoundException($"No se encontró el residuo con ID {dto.Id}.");

            var yaExiste = await _context.EnciclopediaDesbloqueos
                .AnyAsync(d => d.PerfilId == dto.PerfilId && d.ResiduoId == dto.Id);

            if (yaExiste)
            {
                return new DesbloqueoResultadoDTO
                {
                    EsNuevoDesbloqueo = false,
                    Mensaje = $"{residuo.Nombre} ya estaba en tu enciclopedia.",
                    Residuo = null
                };
            }

            var nuevoDesbloqueo = new EnciclopediaDesbloqueo
            {
                PerfilId = dto.PerfilId,
                ResiduoId = dto.Id,
                FechaDesbloqueo = DateTime.UtcNow
            };

            _context.EnciclopediaDesbloqueos.Add(nuevoDesbloqueo);
            await _context.SaveChangesAsync();

            return new DesbloqueoResultadoDTO
            {
                EsNuevoDesbloqueo = true,
                Mensaje = $"¡Nuevo residuo descubierto! {residuo.Nombre} fue agregado a tu enciclopedia.",
                Residuo = new EnciclopediaDetalleDTO
                {
                    Id = residuo.Id,
                    Nombre = residuo.Nombre,
                    Categoria = residuo.Categoria,
                    Subcategoria = residuo.Subcategoria,
                    DescripcionParaNinos = residuo.DescripcionParaNinos,
                    DatoCurioso = residuo.DatoCurioso,
                    NombreSprite = residuo.NombreSprite,
                    FechaDesbloqueo = nuevoDesbloqueo.FechaDesbloqueo
                }
            };
        }
    }
}
