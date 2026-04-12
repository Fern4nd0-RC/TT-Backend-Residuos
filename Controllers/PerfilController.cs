using Microsoft.AspNetCore.Mvc;
using ResiduosBackend.DTOs;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;

    // ¡Adiós AppDbContext! Inyectamos solo el servicio
    public PerfilController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PerfilDTO>>> GetPerfiles()
    {
        var perfiles = await _perfilService.ObtenerPerfilesAsync();
        return Ok(perfiles);
    }

    [HttpPost]
    public async Task<ActionResult<PerfilDTO>> CrearPerfil(Perfil perfil)
    {
        var perfilCreado = await _perfilService.CrearPerfilAsync(perfil);
        
        return CreatedAtAction(
            nameof(GetPerfil),
            new { id = perfilCreado.Id },
            perfilCreado
        );
    }

    [HttpPut("{id}/progreso")]
    public async Task<IActionResult> ActualizarProgreso(int id, PartidaResultado resultado)
    {
        if (id != resultado.PerfilId)
            return BadRequest("El ID del perfil no coincide.");

        var perfilActualizado = await _perfilService.ActualizarProgresoAsync(id, resultado);

        if (perfilActualizado == null)
            return NotFound("Perfil no encontrado.");

        // Mantenemos la estructura JSON que tenías originalmente para Unity
        return Ok(new
        {
            mensaje = "Progreso actualizado",
            nivelActual = perfilActualizado.Nivel,
            fichasTotales = perfilActualizado.Monedas
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PerfilDTO>> GetPerfil(int id)
    {
        var perfil = await _perfilService.ObtenerPerfilAsync(id);

        if (perfil == null)
            return NotFound("No se encontró el perfil de este jugador.");

        return Ok(perfil);
    }
}