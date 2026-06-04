using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Enciclopedia: catálogo con enmascaramiento RN-601, detalle autorizado y desbloqueos pagados con fichas.
    /// El cobro y la inserción del registro de desbloqueo ocurren en una única transacción.
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
            var perfilExiste = await _context.Perfiles
                .AsNoTracking()
                .AnyAsync(p => p.Id == perfilId);

            if (!perfilExiste)
                throw new KeyNotFoundException($"No se encontró el perfil con ID {perfilId}.");

            var desbloqueos = await _context.EnciclopediaDesbloqueos
                .AsNoTracking()
                .Where(d => d.PerfilId == perfilId)
                .ToDictionaryAsync(d => d.ResiduoId, d => d.FechaDesbloqueo);

            // Orden estable: agrupa por categoría y dentro de cada una por Id; la UI puede confiar en este orden.
            var residuos = await _context.Residuos
                .AsNoTracking()
                .OrderBy(r => r.Categoria)
                .ThenBy(r => r.Id)
                .ToListAsync();

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
                    FechaDesbloqueo = desbloqueado ? fecha : null,
                    CostoFichas = r.CostoFichas
                };
            });
        }

        /// <inheritdoc />
        public async Task<EnciclopediaDetalleDTO> ObtenerDetalleAsync(int perfilId, int residuoId)
        {
            var desbloqueo = await _context.EnciclopediaDesbloqueos
                .AsNoTracking()
                .Include(d => d.Residuo)
                .FirstOrDefaultAsync(d => d.PerfilId == perfilId && d.ResiduoId == residuoId);

            if (desbloqueo == null || desbloqueo.Residuo == null)
                throw new UnauthorizedAccessException(
                    "Este residuo aún no ha sido descubierto. ¡Sigue jugando para desbloquearlo!");

            return ConstruirDetalle(desbloqueo.Residuo, desbloqueo.FechaDesbloqueo);
        }

        /// <inheritdoc />
        public async Task<DesbloqueoResultadoDTO> DesbloquearResiduoAsync(DesbloquearResiduoDTO dto)
        {
            // Cargamos perfil con seguimiento para poder descontar Monedas más abajo.
            var perfil = await _context.Perfiles.FirstOrDefaultAsync(p => p.Id == dto.PerfilId)
                ?? throw new KeyNotFoundException($"No se encontró el perfil con ID {dto.PerfilId}.");

            var residuo = await _context.Residuos.FindAsync(dto.ResiduoId)
                ?? throw new KeyNotFoundException($"No se encontró el residuo con ID {dto.ResiduoId}.");

            // Caso 1: ya estaba desbloqueado -> respuesta idempotente, sin cobro.
            var desbloqueoExistente = await _context.EnciclopediaDesbloqueos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.PerfilId == dto.PerfilId && d.ResiduoId == dto.ResiduoId);

            if (desbloqueoExistente != null)
            {
                return new DesbloqueoResultadoDTO
                {
                    EsNuevoDesbloqueo = false,
                    Mensaje = $"{residuo.Nombre} ya estaba en tu enciclopedia.",
                    FichasRestantes = perfil.Monedas,
                    Residuo = ConstruirDetalle(residuo, desbloqueoExistente.FechaDesbloqueo)
                };
            }

            // Caso 2: fondos insuficientes -> no cobra, no inserta. El controller traduce a HTTP 400.
            if (perfil.Monedas < residuo.CostoFichas)
            {
                return new DesbloqueoResultadoDTO
                {
                    EsNuevoDesbloqueo = false,
                    FondosInsuficientes = true,
                    Mensaje = $"Necesitas {residuo.CostoFichas} fichas y tienes {perfil.Monedas}.",
                    FichasRestantes = perfil.Monedas,
                    Residuo = null
                };
            }

            // Caso 3: cobro + alta en una sola transacción.
            // La transacción protege la atomicidad: si algo falla, no queda perfil descontado sin desbloqueo
            // ni desbloqueo sin descuento.
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                perfil.Monedas -= residuo.CostoFichas;

                var nuevoDesbloqueo = new EnciclopediaDesbloqueo
                {
                    PerfilId = dto.PerfilId,
                    ResiduoId = dto.ResiduoId,
                    FechaDesbloqueo = DateTime.UtcNow
                };
                _context.EnciclopediaDesbloqueos.Add(nuevoDesbloqueo);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new DesbloqueoResultadoDTO
                {
                    EsNuevoDesbloqueo = true,
                    Mensaje = $"¡Nuevo residuo descubierto! {residuo.Nombre} se agregó a tu enciclopedia.",
                    FichasRestantes = perfil.Monedas,
                    Residuo = ConstruirDetalle(residuo, nuevoDesbloqueo.FechaDesbloqueo)
                };
            }
            catch (DbUpdateException)
            {
                // Race condition: otra request insertó la misma PK compuesta entre el check y el SaveChanges.
                // Rollback, releer el saldo real y devolver como "ya existía".
                await tx.RollbackAsync();
                await _context.Entry(perfil).ReloadAsync();

                return new DesbloqueoResultadoDTO
                {
                    EsNuevoDesbloqueo = false,
                    Mensaje = $"{residuo.Nombre} ya estaba en tu enciclopedia.",
                    FichasRestantes = perfil.Monedas,
                    Residuo = null
                };
            }
        }

        /// <summary>Helper interno: arma el DTO de detalle a partir del modelo y la fecha de desbloqueo.</summary>
        private static EnciclopediaDetalleDTO ConstruirDetalle(Residuo r, DateTime fecha) => new()
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Categoria = r.Categoria,
            Subcategoria = r.Subcategoria,
            DescripcionParaNinos = r.DescripcionParaNinos,
            DatoCurioso = r.DatoCurioso,
            NombreSprite = r.NombreSprite,
            FechaDesbloqueo = fecha
        };
    }
}
