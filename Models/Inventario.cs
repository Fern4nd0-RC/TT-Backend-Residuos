using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResiduosBackend.Models;

public class Inventario
{
    [Key]
    public int Id { get; set; } // El ID único de este registro o "espacio en la mochila"

    [Required]
    public int PerfilId { get; set; } // ¿De quién es la mochila?

    [Required]
    public int ItemId { get; set; } // ¿Qué objeto hay en este espacio?

    [Required]
    public int Cantidad { get; set; } = 1; // ¿Cuántos hay apilados?

    // --- PROPIEDADES DE NAVEGACIÓN ---
    // Esto es lo que permite que el `_context.Inventarios.Include(i => i.Item)` funcione en tu Servicio.
    // Le dicen a Entity Framework: "Oye, cuando traigas un registro de inventario, tráete también todos los datos del Perfil y del Item asociado".

    [ForeignKey("PerfilId")]
    public Perfil? Perfil { get; set; }

    [ForeignKey("ItemId")]
    public Item? Item { get; set; }
}