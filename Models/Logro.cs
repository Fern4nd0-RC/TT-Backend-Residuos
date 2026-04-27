using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Insignia desbloqueable según progreso acumulado.
/// </summary>
public class Logro
{
    /// <summary>Identificador único del logro.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Nombre mostrado de la insignia.</summary>
    [Required]
    [MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción corta del criterio del logro.</summary>
    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Ruta o URL de imagen consumida por cliente.</summary>
    [MaxLength(300)]
    public string ImagenUrl { get; set; } = string.Empty;

    /// <summary>XP mínima acumulada necesaria para desbloquear.</summary>
    public int RequisitoXP { get; set; }

    /// <summary>Perfiles que han desbloqueado este logro.</summary>
    public ICollection<PerfilLogro>? PerfilesLogro { get; set; }
}
