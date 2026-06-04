using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Definición de un ítem vendible o obtenible: precios, tipo y recurso gráfico.
/// </summary>
public class Item
{
    /// <summary>Identificador único del ítem en catálogo.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>Nombre para tienda e inventario.</summary>
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción para la interfaz de tienda.</summary>
    [MaxLength(250)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Discriminante de comportamiento: <c>Cosmetico</c> (no consumible, RN-702) u <c>ObjetoDeJuego</c> (consumible en partida, RF-306).
    /// </summary>
    [MaxLength(50)]
    public string Tipo { get; set; } = "Cosmetico";

    /// <summary>Precio en fichas; cero excluye ese medio de pago (RN-702).</summary>
    public int CostoFichas { get; set; } = 0;

    /// <summary>Precio en estrellas para ítems premium; cero excluye ese medio de pago.</summary>
    public int CostoEstrellas { get; set; } = 0;

    /// <summary>Nivel mínimo del jugador para desbloquear la compra (RN-302).</summary>
    public int NivelDesbloqueo { get; set; } = 1;

    /// <summary>Nombre del sprite en el cliente para carga directa del recurso.</summary>
    [MaxLength(100)]
    public string NombreSprite { get; set; } = string.Empty;

    /// <summary>Registros de inventario que referencian este ítem.</summary>
    public ICollection<Inventario>? Inventarios { get; set; }
}
