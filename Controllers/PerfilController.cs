using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers;

/// <summary>
/// API de perfiles: listado, consulta, creación, actualización de progreso y eliminación.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;

    /// <summary>
    /// Inicializa el controlador con el servicio de perfiles.
    /// </summary>
    public PerfilController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    /// <summary>
    /// Obtiene todos los perfiles (pantalla de selección, RF-101).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PerfilDTO>>> GetPerfiles()
    {
        var perfiles = await _perfilService.ObtenerPerfilesAsync();
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
    /// Crea un perfil nuevo. Aplica RN-201 (máximo cuatro perfiles). Usa <see cref="CrearPerfilDTO"/> para no aceptar campos de servidor como <c>Id</c> o <c>FechaCreacion</c>.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PerfilDTO>> CrearPerfil(CrearPerfilDTO dto)
    {
        try
        {
            var perfilCreado = await _perfilService.CrearPerfilAsync(dto);
            return CreatedAtAction(nameof(GetPerfil), new { id = perfilCreado.Id }, perfilCreado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza monedas, experiencia y estrellas tras un minijuego (RF-103, RN-704). El <paramref name="id"/> de ruta debe coincidir con <see cref="ActualizarProgresoDTO.PerfilId"/>.
    /// </summary>
    [HttpPut("{id}/progreso")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> EliminarPerfil(int id)
    {
        var eliminado = await _perfilService.EliminarPerfilAsync(id);
        if (!eliminado)
            return NotFound(new { mensaje = "No se encontró el perfil a eliminar." });

        return Ok(new { mensaje = "Perfil eliminado correctamente." });
    }
}
