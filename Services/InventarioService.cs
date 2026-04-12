using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTOs;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models; // Ajusta según tu namespace de modelos

namespace ResiduosBackend.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly AppDbContext _context;

        public InventarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventarioDTO>> ObtenerInventarioPorPerfilAsync(int perfilId)
        {
            // Buscamos los registros y usamos Include para traer los datos del Item real
            var inventario = await _context.Inventarios
                .Include(i => i.Item) // Asumiendo que tienes una propiedad de navegación llamada Item
                .Where(i => i.PerfilId == perfilId)
                .ToListAsync();

            return inventario.Select(i => new InventarioDTO
            {
                IdRegistro = i.Id,
                ItemId = i.ItemId,
                NombreItem = i.Item.Nombre, // Ajusta a la propiedad real de tu modelo Item
                Cantidad = i.Cantidad
            });
        }

        public async Task<InventarioDTO> AgregarItemAsync(int perfilId, int itemId, int cantidadAgregada)
        {
            // 1. Verificamos si el jugador ya tiene este ítem en su inventario
            var registroExistente = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.PerfilId == perfilId && i.ItemId == itemId);

            if (registroExistente != null)
            {
                // Si ya lo tiene, solo apilamos la cantidad
                registroExistente.Cantidad += cantidadAgregada;
                _context.Entry(registroExistente).State = EntityState.Modified;
            }
            else
            {
                // Si es un ítem nuevo, creamos el registro
                registroExistente = new Inventario // Ajusta a tu clase real de Inventario
                {
                    PerfilId = perfilId,
                    ItemId = itemId,
                    Cantidad = cantidadAgregada
                };
                _context.Inventarios.Add(registroExistente);
            }

            await _context.SaveChangesAsync();

            // Para devolver el DTO correctamente, necesitamos el nombre del ítem
            var itemAsociado = await _context.Items.FindAsync(itemId);

            return new InventarioDTO
            {
                IdRegistro = registroExistente.Id,
                ItemId = registroExistente.ItemId,
                NombreItem = itemAsociado?.Nombre ?? "Ítem Desconocido",
                Cantidad = registroExistente.Cantidad
            };
        }

        public async Task<bool> ConsumirItemAsync(int perfilId, int itemId, int cantidadUsada)
        {
            var registro = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.PerfilId == perfilId && i.ItemId == itemId);

            // Si no lo tiene, o la cantidad que quiere usar es mayor a la que posee, falla
            if (registro == null || registro.Cantidad < cantidadUsada) return false;

            registro.Cantidad -= cantidadUsada;

            // Si la cantidad llega a 0, limpiamos ese espacio del inventario para ahorrar espacio en BD
            if (registro.Cantidad <= 0)
            {
                _context.Inventarios.Remove(registro);
            }
            else
            {
                _context.Entry(registro).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}