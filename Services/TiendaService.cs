using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Catálogo de tienda por perfil y compra con descuento atómico de saldo e inserción en inventario.
    /// </summary>
    public class TiendaService : ITiendaService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Crea el servicio con el contexto de datos inyectado.
        /// </summary>
        public TiendaService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TiendaItemDTO>> ObtenerCatalogoAsync(int perfilId)
        {
            var perfil = await _context.Perfiles.FindAsync(perfilId)
                ?? throw new KeyNotFoundException($"No se encontró el perfil con ID {perfilId}.");

            var idsEnInventario = await _context.Inventarios
                .Where(i => i.PerfilId == perfilId)
                .Select(i => i.ItemId)
                .ToHashSetAsync();

            var items = await _context.Items.ToListAsync();

            return items.Select(item => new TiendaItemDTO
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion,
                Tipo = item.Tipo,
                NombreSprite = item.NombreSprite,
                CostoFichas = item.CostoFichas,
                CostoEstrellas = item.CostoEstrellas,
                NivelDesbloqueo = item.NivelDesbloqueo,
                YaPoseido = idsEnInventario.Contains(item.Id),
                NivelSuficiente = perfil.Nivel >= item.NivelDesbloqueo
            });
        }

        /// <inheritdoc />
        public async Task<CompraResultadoDTO> ComprarItemAsync(ComprarItemDTO dto)
        {
            var perfil = await _context.Perfiles.FindAsync(dto.PerfilId)
                ?? throw new KeyNotFoundException($"No se encontró el perfil con ID {dto.PerfilId}.");

            var item = await _context.Items.FindAsync(dto.ItemId)
                ?? throw new KeyNotFoundException($"No se encontró el ítem con ID {dto.ItemId}.");

            if (perfil.Nivel < item.NivelDesbloqueo)
                throw new InvalidOperationException(
                    $"Debes alcanzar el nivel {item.NivelDesbloqueo} para desbloquear este ítem.");

            var yaLoTiene = await _context.Inventarios
                .AnyAsync(i => i.PerfilId == dto.PerfilId && i.ItemId == dto.ItemId);
            if (yaLoTiene)
                throw new InvalidOperationException("Ya posees este ítem.");

            if (item.CostoFichas > 0)
            {
                if (perfil.Monedas < item.CostoFichas)
                    throw new InvalidOperationException(
                        $"Fichas insuficientes. Necesitas {item.CostoFichas}, tienes {perfil.Monedas}.");

                perfil.Monedas -= item.CostoFichas;
            }
            else if (item.CostoEstrellas > 0)
            {
                if (perfil.EstrellaSostenibilidad < item.CostoEstrellas)
                    throw new InvalidOperationException(
                        $"Estrellas insuficientes. Necesitas {item.CostoEstrellas}, tienes {perfil.EstrellaSostenibilidad}.");

                perfil.EstrellaSostenibilidad -= item.CostoEstrellas;
            }

            var nuevoRegistro = new Inventario
            {
                PerfilId = dto.PerfilId,
                ItemId = dto.ItemId,
                Cantidad = 1
            };

            _context.Entry(perfil).State = EntityState.Modified;
            _context.Inventarios.Add(nuevoRegistro);
            await _context.SaveChangesAsync();

            return new CompraResultadoDTO
            {
                Mensaje = $"¡Compra exitosa! Adquiriste: {item.Nombre}.",
                FichasRestantes = perfil.Monedas,
                EstrellasRestantes = perfil.EstrellaSostenibilidad,
                ItemAgregado = new InventarioDTO
                {
                    Id = nuevoRegistro.Id,
                    ItemId = item.Id,
                    Nombre = item.Nombre,
                    Tipo = item.Tipo,
                    Cantidad = 1
                }
            };
        }
    }
}
