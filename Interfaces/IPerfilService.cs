using ResiduosBackend.DTOs;
using ResiduosBackend.Models; // Necesario para PartidaResultado

namespace ResiduosBackend.Interfaces
{
    public interface IPerfilService
    {
        Task<IEnumerable<PerfilDTO>> ObtenerPerfilesAsync();
        Task<PerfilDTO> ObtenerPerfilAsync(int id);
        Task<PerfilDTO> CrearPerfilAsync(Perfil perfilNuevo); 
        Task<PerfilDTO> ActualizarProgresoAsync(int id, PartidaResultado resultado);
    }
}