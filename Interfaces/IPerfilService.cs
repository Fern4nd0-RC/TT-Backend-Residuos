using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato del servicio de perfiles: consulta, alta con RN-201, progreso RN-704 y baja RN-202.
    /// </summary>
    public interface IPerfilService
    {
        /// <summary>
        /// Obtiene todos los perfiles persistidos.
        /// </summary>
        Task<IEnumerable<PerfilDTO>> ObtenerPerfilesAsync();

        /// <summary>
        /// Obtiene un perfil por su identificador o <c>null</c> si no existe.
        /// </summary>
        Task<PerfilDTO?> ObtenerPerfilAsync(int id);

        /// <summary>
        /// Crea un perfil validando el tope de cuatro perfiles por dispositivo (RN-201).
        /// </summary>
        Task<PerfilDTO> CrearPerfilAsync(CrearPerfilDTO dto);

        /// <summary>
        /// Suma experiencia, fichas y estrellas según el resultado de un minijuego (RN-704).
        /// </summary>
        Task<PerfilDTO?> ActualizarProgresoAsync(int id, ActualizarProgresoDTO dto);

        /// <summary>
        /// Elimina el perfil indicado. Devuelve <c>false</c> si el identificador no existe.
        /// </summary>
        Task<bool> EliminarPerfilAsync(int id);
    }
}
