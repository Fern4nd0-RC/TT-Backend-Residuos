using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Operaciones de inventario: listado, apilado al agregar y consumo solo de ítems de juego (RN-702).
    /// </summary>
    public class InventarioService : IInventarioService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Crea el servicio con el contexto de datos inyectado.
        /// </summary>
        public InventarioService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InventarioDTO>> ObtenerInventarioPorPerfilAsync(int perfilId)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Item)
                .Where(i => i.PerfilId == perfilId)
                .ToListAsync();

            return inventario.Select(MapearADTO);
        }

        /// <inheritdoc />
        public async Task<InventarioDTO> AgregarItemAsync(AgregarItemDTO dto)
        {
            var perfilExiste = await _context.Perfiles.AnyAsync(p => p.Id == dto.PerfilId);
            if (!perfilExiste)
                throw new KeyNotFoundException($"No se encontró el perfil con ID {dto.PerfilId}.");

            var itemExiste = await _context.Items.AnyAsync(i => i.Id == dto.ItemId);
            if (!itemExiste)
                throw new KeyNotFoundException($"No se encontró el ítem con ID {dto.ItemId}.");

            var registroExistente = await _context.Inventarios
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.PerfilId == dto.PerfilId && i.ItemId == dto.ItemId);

            if (registroExistente != null)
            {
                registroExistente.Cantidad += dto.Cantidad;
                _context.Entry(registroExistente).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return MapearADTO(registroExistente);
            }

            var nuevoRegistro = new Inventario
            {
                PerfilId = dto.PerfilId,
                ItemId = dto.ItemId,
                Cantidad = dto.Cantidad
            };
            _context.Inventarios.Add(nuevoRegistro);
            await _context.SaveChangesAsync();

            await _context.Entry(nuevoRegistro).Reference(i => i.Item).LoadAsync();

            return MapearADTO(nuevoRegistro);
        }

        /// <inheritdoc />
        public async Task<bool> ConsumirItemAsync(ConsumirItemDTO dto)
        {
            var registro = await _context.Inventarios
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.PerfilId == dto.PerfilId && i.ItemId == dto.ItemId);

            if (registro == null) return false;

            if (registro.Item?.Tipo == "Cosmetico")
                return false;

            if (registro.Cantidad < dto.Cantidad) return false;

            registro.Cantidad -= dto.Cantidad;

            if (registro.Cantidad <= 0)
                _context.Inventarios.Remove(registro);
            else
                _context.Entry(registro).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;
        }

        private static InventarioDTO MapearADTO(Inventario i) => new InventarioDTO
        {
            Id = i.Id,
            ItemId = i.ItemId,
            Nombre = i.Item?.Nombre ?? "Ítem Desconocido",
            Tipo = i.Item?.Tipo ?? "Desconocido",
            Cantidad = i.Cantidad
        };
    }
}
