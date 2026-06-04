using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using System.Security.Claims;

namespace ResiduosBackend.Controllers;

/// <summary>
/// API de perfiles: listado por dispositivo, consulta, creación, actualización
/// de progreso y eliminación. Todos los endpoints requieren JWT del dispositivo.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;

    public PerfilController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    /// <summary>
    /// Lee el GUID del dispositivo desde el token JWT (claim "sub" / NameIdentifier).
    /// </summary>
    private string? ObtenerDeviceIdDelToken() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

    /// <summary>
    /// Obtiene los perfiles del dispositivo autenticado (pantalla de selección, RF-101).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PerfilDTO>>> GetPerfiles()
    {
        var deviceId = ObtenerDeviceIdDelToken();
        if (string.IsNullOrWhiteSpace(deviceId))
            return Unauthorized(new { mensaje = "Token de dispositivo inválido." });

        var perfiles = await _perfilService.ObtenerPerfilesPorDispositivoAsync(deviceId);
        return Ok(perfiles);
    }

    /// <summary>
    /// Obtiene un perfil por identificador (RF-103).
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PerfilDTO>> GetPerfil(int id)
    {
        var perfil = await _perfilService.ObtenerPerfilAsync(id);
        if (perfil == null)
            return NotFound("No se encontró el perfil de este jugador.");

        return Ok(perfil);
    }

    /// <summary>
    /// Crea un perfil para el dispositivo del token. Aplica RN-201 (máximo cuatro
    /// por dispositivo). El GUID se toma del JWT, no del cuerpo de la solicitud.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PerfilDTO>> CrearPerfil(CrearPerfilDTO dto)
    {
        var deviceId = ObtenerDeviceIdDelToken();
        if (string.IsNullOrWhiteSpace(deviceId))
            return Unauthorized(new { mensaje = "Token de dispositivo inválido." });

        try
        {
            var perfilCreado = await _perfilService.CrearPerfilAsync(deviceId, dto);
            return CreatedAtAction(nameof(GetPerfil), new { id = perfilCreado.Id }, perfilCreado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza monedas, experiencia y estrellas tras un minijuego (RF-103, RN-704).
    /// </summary>
    [HttpPut("{id}/progreso")]
    public async Task<IActionResult> ActualizarProgreso(int id, ActualizarProgresoDTO dto)
    {
        if (id != dto.PerfilId)
            return BadRequest(new { mensaje = "El ID del perfil no coincide con el cuerpo de la solicitud." });
        if (dto.XpGanado < 0 || dto.FichasGanadas < 0 || dto.EstrellasGanadas < 0)
            return BadRequest(new { mensaje = "Los incrementos de progreso no pueden ser negativos." });

        var perfilActualizado = await _perfilService.ActualizarProgresoAsync(id, dto);
        if (perfilActualizado == null)
            return NotFound(new { mensaje = "Perfil no encontrado." });

        return Ok(new
        {
            mensaje = "Progreso guardado correctamente.",
            nivelActual = perfilActualizado.Nivel,
            experienciaTotal = perfilActualizado.Experiencia,
            fichasTotales = perfilActualizado.Monedas,
            estrellasSostenibilidad = perfilActualizado.EstrellaSostenibilidad
        });
    }

    /// <summary>
    /// Elimina un perfil (RN-202). La confirmación en interfaz corresponde al cliente.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPerfil(int id)
    {
        var eliminado = await _perfilService.EliminarPerfilAsync(id);
        if (!eliminado)
            return NotFound(new { mensaje = "No se encontró el perfil a eliminar." });

        return Ok(new { mensaje = "Perfil eliminado correctamente." });
    }
}