using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Contenido pedagógico de la enciclopedia: clasificación, textos y recurso visual.
/// </summary>
public class Residuo
{
    /// <summary>Identificador del residuo.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Nombre común presentado al desbloquear la ficha (RNF-404).</summary>
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Eje principal de clasificación; valores acordados con reglas RN-602 (orgánico, reciclable, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    /// <summary>Subcategoría para residuos complejos en niveles avanzados (RNF-408).</summary>
    [MaxLength(50)]
    public string Subcategoria { get; set; } = string.Empty;

    /// <summary>Explicación adaptada al rango de edad del juego (RNF-404).</summary>
    [MaxLength(300)]
    public string DescripcionParaNinos { get; set; } = string.Empty;

    /// <summary>Curiosidad mostrada en la ficha desbloqueada (RNF-404).</summary>
    [MaxLength(300)]
    public string DatoCurioso { get; set; } = string.Empty;

    /// <summary>Identificador del sprite en el cliente (RNF-404).</summary>
    [MaxLength(100)]
    public string NombreSprite { get; set; } = string.Empty;

    /// <summary>Desbloqueos por perfil que referencian este residuo.</summary>
    public ICollection<EnciclopediaDesbloqueo>? Desbloqueos { get; set; }
}
