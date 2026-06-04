using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResiduosBackend.Models;

/// <summary>
/// Tabla de unión perfil–residuo: registra el primer acierto de clasificación que desbloquea la ficha (RF-602).
/// La clave primaria compuesta se configura en <see cref="ResiduosBackend.Data.AppDbContext"/>.
/// </summary>
public class EnciclopediaDesbloqueo
{
    /// <summary>Perfil que desbloqueó el residuo.</summary>
    [Required]
    public int PerfilId { get; set; }

    /// <summary>Residuo desbloqueado.</summary>
    [Required]
    public int ResiduoId { get; set; }

    /// <summary>Fecha y hora UTC en que se registró el desbloqueo.</summary>
    public DateTime FechaDesbloqueo { get; set; } = DateTime.UtcNow;

    /// <summary>Navegación al perfil.</summary>
    [ForeignKey("PerfilId")]
    public Perfil? Perfil { get; set; }

    /// <summary>Navegación al residuo.</summary>
    [ForeignKey("ResiduoId")]
    public Residuo? Residuo { get; set; }
}
