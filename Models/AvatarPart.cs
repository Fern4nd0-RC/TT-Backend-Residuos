using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Pieza intercambiable del avatar 3D del jugador (cuerpo, cara o sombrero).
/// Cada fila representa UNA opción que el usuario puede elegir en el panel de registro.
///
/// Diseño:
/// - Tabla separada de <see cref="Item"/> a propósito: las partes del avatar no tienen
///   precio, costos en fichas/estrellas ni desbloqueo por nivel. Mezclarlas con
///   ítems de tienda obligaría a llenar columnas sin sentido y a filtrar por
///   <c>Tipo</c> en todas las consultas, ensuciando ambos dominios.
/// - <see cref="RecursoUnity"/> guarda el identificador con el que el cliente Unity
///   carga el material o prefab (ej. "Red 2 Base", "party hat", "face 1").
///   Es el contrato entre BD y los recursos del pack.
/// </summary>
public class AvatarPart
{
    /// <summary>Identificador único.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Slot al que pertenece la parte. Valores: <c>Body</c>, <c>Face</c>, <c>Hat</c>.
    /// Se valida en el servicio para impedir asignar, p. ej., un sombrero como cara.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Slot { get; set; } = string.Empty;

    /// <summary>Nombre legible para mostrar en la UI (ej. "Rojo Base", "Sombrero de fiesta").</summary>
    [Required]
    [MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del recurso que Unity carga vía <c>Resources.Load</c>. Para cuerpos y caras
    /// corresponde al nombre del material (ej. "Red 2 Base"); para sombreros, al nombre
    /// del prefab (ej. "party hat"). Cadena vacía significa "ninguno" (ej. "Sin sombrero").
    /// </summary>
    [Required]
    [MaxLength(120)]
    public string RecursoUnity { get; set; } = string.Empty;

    /// <summary>Orden de aparición en el carrusel de la UI (ascendente).</summary>
    public int Orden { get; set; } = 0;
}
