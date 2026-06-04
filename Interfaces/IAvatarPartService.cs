using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato del catálogo de partes de avatar.
    /// </summary>
    public interface IAvatarPartService
    {
        /// <summary>
        /// Devuelve el catálogo completo agrupado por slot, ordenado por <c>Orden</c>.
        /// </summary>
        Task<AvatarPartsCatalogoDTO> ObtenerCatalogoAsync();

        /// <summary>
        /// Devuelve los IDs por defecto para inicializar un avatar nuevo.
        /// Convenientemente, el primer item de cada slot por <c>Orden</c>.
        /// Si no existieran partes para algún slot, devuelve null en ese slot.
        /// </summary>
        Task<(int? bodyId, int? faceId, int? hatId, int? pfpId)> ObtenerDefaultsAsync();

        /// <summary>
        /// Valida que los tres IDs apunten a partes existentes y al slot correcto.
        /// Lanza <see cref="InvalidOperationException"/> con detalle si algo está mal.
        /// </summary>
        Task ValidarAsignacionAsync(int bodyId, int faceId, int hatId, int pfpId);
    }
}
