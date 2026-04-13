using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato de la enciclopedia: catálogo, detalle protegido y registro de desbloqueos (RF-602).
    /// </summary>
    public interface IEnciclopediaService
    {
        /// <summary>
        /// Lista residuos con datos sensibles omitidos mientras el perfil no los haya desbloqueado (RN-601).
        /// </summary>
        Task<IEnumerable<EnciclopediaEntradaDTO>> ObtenerCatalogoAsync(int perfilId);

        /// <summary>
        /// Devuelve el detalle solo si existe relación de desbloqueo; en caso contrario lanza <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        Task<EnciclopediaDetalleDTO> ObtenerDetalleAsync(int perfilId, int residuoId);

        /// <summary>
        /// Inserta el desbloqueo si es la primera vez; si ya existía, indica <c>EsNuevoDesbloqueo = false</c>.
        /// </summary>
        Task<DesbloqueoResultadoDTO> DesbloquearResiduoAsync(DesbloquearResiduoDTO dto);
    }
}
