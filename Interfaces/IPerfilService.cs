using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato del servicio de perfiles: consulta, alta con RN-201 por dispositivo,
    /// progreso RN-704 y baja RN-202.
    /// </summary>
    public interface IPerfilService
    {
        /// <summary>Obtiene todos los perfiles de un dispositivo.</summary>
        Task<IEnumerable<PerfilDTO>> ObtenerPerfilesPorDispositivoAsync(string deviceId);

        /// <summary>Obtiene un perfil por su identificador o <c>null</c> si no existe.</summary>
        Task<PerfilDTO?> ObtenerPerfilAsync(int id);

        /// <summary>
        /// Crea un perfil para el dispositivo indicado, validando el tope de cuatro
        /// perfiles por dispositivo (RN-201). Registra el dispositivo si no existe.
        /// </summary>
        Task<PerfilDTO> CrearPerfilAsync(string deviceId, CrearPerfilDTO dto);

        /// <summary>Suma experiencia, fichas y estrellas según el minijuego (RN-704).</summary>
        Task<PerfilDTO?> ActualizarProgresoAsync(int id, ActualizarProgresoDTO dto);

        /// <summary>Elimina el perfil indicado. Devuelve <c>false</c> si no existe.</summary>
        Task<bool> EliminarPerfilAsync(int id);
    }
}