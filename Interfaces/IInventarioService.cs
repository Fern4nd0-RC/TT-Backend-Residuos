using ResiduosBackend.DTOs;

namespace ResiduosBackend.Interfaces
{
    public interface IInventarioService
    {
        // Trae toda la mochila de un jugador específico
        Task<IEnumerable<InventarioDTO>> ObtenerInventarioPorPerfilAsync(int perfilId);
        
        // Lógica para cuando el jugador recoge algo
        Task<InventarioDTO> AgregarItemAsync(int perfilId, int itemId, int cantidadAgregada);
        
        // Lógica para cuando el jugador gasta o tira algo
        Task<bool> ConsumirItemAsync(int perfilId, int itemId, int cantidadUsada);
    }
}