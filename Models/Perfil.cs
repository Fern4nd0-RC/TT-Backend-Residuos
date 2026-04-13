using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Perfil de jugador: progreso, monedas y relación con inventario.
/// </summary>
public class Perfil
{
    /// <summary>Identificador único en base de datos.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Nombre mostrado en selección de perfil y tablero.</summary>
    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>Fichas de reciclaje; se incrementan en partidas y minijuegos para la economía de tienda (RN-701).</summary>
    public int Monedas { get; set; } = 0;

    /// <summary>Experiencia acumulada; el nivel se deriva de este valor (RF-502).</summary>
    public int Experiencia { get; set; } = 0;

    /// <summary>Nivel actual acotado superiormente por la regla de juego (RN-703).</summary>
    public int Nivel { get; set; } = 1;

    /// <summary>Estrellas de sostenibilidad asociadas a condiciones de victoria (RN-404).</summary>
    public int EstrellaSostenibilidad { get; set; } = 0;

    /// <summary>Marca temporal de creación en UTC.</summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>Filas de inventario asociadas a este perfil.</summary>
    public ICollection<Inventario>? Inventarios { get; set; }
}
