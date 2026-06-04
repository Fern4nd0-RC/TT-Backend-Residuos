using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Registro histórico de una participación en partida: puntuación y aciertos de clasificación.
/// </summary>
public class PartidaMetrica
{
    /// <summary>Identificador de la métrica.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Perfil al que pertenecen los datos.</summary>
    public int PerfilId { get; set; }

    /// <summary>Puntuación obtenida en esa sesión (auditoría y analítica).</summary>
    public int PuntuacionObtenida { get; set; }

    /// <summary>Residuos clasificados correctamente en la sesión.</summary>
    public int ResiduosClasificadosCorrectamente { get; set; }

    /// <summary>Total de clasificaciones correctas de residuos orgánicos.</summary>
    public int ResiduosOrganicosClasificados { get; set; }

    /// <summary>Total de clasificaciones correctas de residuos inorgánicos.</summary>
    public int ResiduosInorganicosClasificados { get; set; }

    /// <summary>Marca temporal de la partida en UTC.</summary>
    public DateTime FechaPartida { get; set; } = DateTime.UtcNow;

    /// <summary>Navegación al perfil.</summary>
    public Perfil? Perfil { get; set; }
}
