using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.DTO;

/// <summary>
/// Credenciales mínimas para login de demo.
/// </summary>
public class LoginRequestDTO
{
    /// <summary>Nombre de usuario del perfil.</summary>
    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta de autenticación para el cliente Unity.
/// </summary>
public class LoginResponseDTO
{
    /// <summary>JWT para autenticación Bearer.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Fecha de expiración del token en UTC.</summary>
    public DateTime ExpiraEnUtc { get; set; }

    /// <summary>Perfil autenticado.</summary>
    public PerfilDTO Perfil { get; set; } = null!;
}
