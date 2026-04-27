namespace ResiduosBackend.DTO;

/// <summary>
/// Resumen agregado de estadísticas por perfil.
/// </summary>
public class EstadisticasPerfilDTO
{
    /// <summary>Perfil consultado.</summary>
    public int PerfilId { get; set; }

    /// <summary>Total de partidas jugadas por el perfil.</summary>
    public int TotalPartidasJugadas { get; set; }

    /// <summary>Puntaje máximo alcanzado por el perfil.</summary>
    public int PuntajeMaximoAlcanzado { get; set; }

    /// <summary>Total de residuos orgánicos clasificados correctamente.</summary>
    public int TotalResiduosOrganicosClasificados { get; set; }

    /// <summary>Total de residuos inorgánicos clasificados correctamente.</summary>
    public int TotalResiduosInorganicosClasificados { get; set; }
}
