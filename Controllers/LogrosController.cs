using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers;

/// <summary>
/// Consulta del estado de logros por perfil.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class LogrosController : ControllerBase
{
    private readonly ILogroService _logroService;

    /// <summary>
    /// Inicializa el controlador con el servicio de logros.
    /// </summary>
    public LogrosController(ILogroService logroService)
    {
        _logroService = logroService;
    }

    /// <summary>
    /// Lista todos los logros indicando bloqueo/desbloqueo y fecha de obtención.
    /// </summary>
    [HttpGet("{perfilId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LogroEstadoDTO>>> GetLogrosPorPerfil(int perfilId)
    {
        try
        {
            var logros = await _logroService.ObtenerLogrosPorPerfilAsync(perfilId);
            return Ok(logros);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}
