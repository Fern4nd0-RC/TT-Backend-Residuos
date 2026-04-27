using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Services;

/// <summary>
/// Agregación de métricas históricas por perfil para panel estadístico.
/// </summary>
public class EstadisticasService : IEstadisticasService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa el servicio con acceso al contexto de datos.
    /// </summary>
    public EstadisticasService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<EstadisticasPerfilDTO> ObtenerEstadisticasPerfilAsync(int perfilId)
    {
        var perfilExiste = await _context.Perfiles.AnyAsync(p => p.Id == perfilId);
        if (!perfilExiste)
            throw new KeyNotFoundException($"No se encontró el perfil con ID {perfilId}.");

        var metricas = _context.PartidaMetricas.Where(m => m.PerfilId == perfilId);

        return new EstadisticasPerfilDTO
        {
            PerfilId = perfilId,
            TotalPartidasJugadas = await metricas.CountAsync(),
            PuntajeMaximoAlcanzado = await metricas.Select(m => (int?)m.PuntuacionObtenida).MaxAsync() ?? 0,
            TotalResiduosOrganicosClasificados = await metricas.SumAsync(m => m.ResiduosOrganicosClasificados),
            TotalResiduosInorganicosClasificados = await metricas.SumAsync(m => m.ResiduosInorganicosClasificados)
        };
    }
}
