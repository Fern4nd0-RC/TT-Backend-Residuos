using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

public class Perfil
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    public int Monedas { get; set; } = 0; 

    public int Experiencia { get; set; } = 0; 

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones para navegación (Opcional pero recomendado)
    public ICollection<Inventario>? Inventarios { get; set; }
}