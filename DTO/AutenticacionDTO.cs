using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.DTO;

/// <summary>
/// Identificación del dispositivo para "login" sin datos personales.
/// El cliente Unity envía el GUID generado localmente (PlayerPrefs "GuestID").
/// </summary>
public class LoginRequestDTO
{
    /// <summary>GUID del dispositivo generado por el cliente.</summary>
    [Required]
    [MaxLength(36)]
    public string DeviceId { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta de autenticación para el cliente Unity.
/// </summary>
public class LoginResponseDTO
{
    /// <summary>JWT que representa al dispositivo para autenticación Bearer.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Fecha de expiración del token en UTC.</summary>
    public DateTime ExpiraEnUtc { get; set; }

    /// <summary>Perfiles registrados en este dispositivo (0 a 4).</summary>
    public List<PerfilDTO> Perfiles { get; set; } = new();
}