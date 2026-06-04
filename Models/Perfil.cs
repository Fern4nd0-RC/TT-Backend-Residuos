using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResiduosBackend.Models;

/// <summary>
/// Perfil de jugador: progreso, monedas y personalización del avatar.
/// </summary>
public class Perfil
{
    /// <summary>Identificador único en base de datos (autoincremental).</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>GUID del dispositivo dueño del perfil (FK a <see cref="Dispositivo"/>).</summary>
    [Required]
    [MaxLength(36)]
    public string DispositivoId { get; set; } = string.Empty;

    /// <summary>Dispositivo al que pertenece el perfil.</summary>
    [ForeignKey(nameof(DispositivoId))]
    public Dispositivo? Dispositivo { get; set; }

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

    // ============================================================
    // PERSONALIZACIÓN DEL AVATAR
    // ============================================================
    // Reemplaza al esquema previo (ColorHex + IndiceImagen). Cada slot del avatar
    // referencia una fila de AvatarPart. "Sin sombrero" se modela como una opción
    // más del catálogo (RecursoUnity = ""), no como NULL, para uniformizar el código
    // del cliente. Las tres FK son NOT NULL y se inicializan con los defaults al crear.

    /// <summary>FK a la parte de cuerpo (material) seleccionada.</summary>
    [Required]
    public int BodyPartId { get; set; }

    [ForeignKey(nameof(BodyPartId))]
    public AvatarPart? BodyPart { get; set; }

    /// <summary>FK a la parte de cara (expresión) seleccionada.</summary>
    [Required]
    public int FacePartId { get; set; }

    [ForeignKey(nameof(FacePartId))]
    public AvatarPart? FacePart { get; set; }

    /// <summary>FK al sombrero seleccionado. "Sin sombrero" es una opción del catálogo.</summary>
    [Required]
    public int HatPartId { get; set; }

    [ForeignKey(nameof(HatPartId))]
    public AvatarPart? HatPart { get; set; }

    /// <summary>FK a la foto de perfil seleccionada.</summary>
    [Required]
    public int ProfilePictureId { get; set; }

    [ForeignKey(nameof(ProfilePictureId))]
    public AvatarPart? ProfilePicturePart { get; set; }

    /// <summary>Marca temporal de creación en UTC.</summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>Filas de inventario asociadas a este perfil.</summary>
    public ICollection<Inventario>? Inventarios { get; set; }

    /// <summary>Insignias desbloqueadas por el perfil.</summary>
    public ICollection<PerfilLogro>? LogrosDesbloqueados { get; set; }
}
