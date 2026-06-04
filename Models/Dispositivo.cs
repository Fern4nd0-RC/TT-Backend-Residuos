using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

/// <summary>
/// Dispositivo (equipo) identificado por un GUID generado en el cliente Unity.
/// Sustituye a la autenticación por datos personales: agrupa hasta cuatro perfiles
/// de niños sin almacenar correo ni credenciales (RN-201 por dispositivo).
/// </summary>
public class Dispositivo
{
    /// <summary>
    /// GUID generado por Unity (PlayerPrefs "GuestID"). Clave primaria del dispositivo.
    /// Se almacena como string para conservar el formato exacto enviado por el cliente.
    /// </summary>
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = string.Empty;

    /// <summary>Marca temporal de primer registro del dispositivo en UTC.</summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>Perfiles (hasta cuatro) asociados a este dispositivo.</summary>
    public ICollection<Perfil>? Perfiles { get; set; }
}