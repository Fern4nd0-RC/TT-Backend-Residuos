namespace ResiduosBackend.DTO;

/// <summary>
/// Estado de un logro para un perfil específico.
/// </summary>
public class LogroEstadoDTO
{
    /// <summary>Identificador del logro.</summary>
    public int Id { get; set; }

    /// <summary>Nombre mostrado del logro.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del logro.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>URL o ruta de imagen de insignia.</summary>
    public string ImagenUrl { get; set; } = string.Empty;

    /// <summary>XP mínima requerida para desbloqueo.</summary>
    public int RequisitoXP { get; set; }

    /// <summary>Indica si el perfil ya desbloqueó el logro.</summary>
    public bool Desbloqueado { get; set; }

    /// <summary>Fecha de obtención en UTC; null si sigue bloqueado.</summary>
    public DateTime? FechaObtencion { get; set; }
}
