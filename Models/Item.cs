using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

public class Item
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Descripcion { get; set; } = string.Empty;

    // Categoría útil para filtrar en Unity (ej. "Consumible", "Material", "Clave")
    [MaxLength(50)]
    public string Tipo { get; set; } = "General"; 

    // Opcional: Útil si quieres mandar a Unity el nombre exacto del sprite a cargar
    [MaxLength(100)]
    public string NombreSprite { get; set; } = string.Empty;

    // Relación de navegación inversa (Opcional pero buena práctica en EF Core)
    public ICollection<Inventario>? Inventarios { get; set; }
}