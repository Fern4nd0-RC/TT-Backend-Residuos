using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResiduosBackend.Models;

/// <summary>
/// Una fila de inventario por par (perfil, ítem); la cantidad apila unidades sin duplicar la clave compuesta lógica.
/// </summary>
public class Inventario
{
    /// <summary>Identificador de la fila de inventario.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Perfil propietario.</summary>
    [Required]
    public int PerfilId { get; set; }

    /// <summary>Ítem almacenado.</summary>
    [Required]
    public int ItemId { get; set; }

    /// <summary>Unidades disponibles en esta fila.</summary>
    [Required]
    public int Cantidad { get; set; } = 1;

    /// <summary>Navegación al perfil; habilita consultas con <c>Include</c> en EF Core.</summary>
    [ForeignKey("PerfilId")]
    public Perfil? Perfil { get; set; }

    /// <summary>Navegación al ítem; necesaria para validar tipo y texto al mapear a DTO.</summary>
    [ForeignKey("ItemId")]
    public Item? Item { get; set; }
}
