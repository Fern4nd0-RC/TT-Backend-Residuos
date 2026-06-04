using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces;

/// <summary>
/// Contrato de estadísticas agregadas por perfil.
/// </summary>
public interface IEstadisticasService
{
    /// <summary>
    /// Obtiene resumen acumulado de partidas del perfil indicado.
    /// </summary>
    Task<EstadisticasPerfilDTO> ObtenerEstadisticasPerfilAsync(int perfilId);
}
