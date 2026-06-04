using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers;

/// <summary>
/// Consulta de estadísticas agregadas por perfil.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EstadisticasController : ControllerBase
{
    private readonly IEstadisticasService _estadisticasService;

    /// <summary>
    /// Inicializa el controlador de estadísticas.
    /// </summary>
    public EstadisticasController(IEstadisticasService estadisticasService)
    {
        _estadisticasService = estadisticasService;
    }

    /// <summary>
    /// Devuelve total de partidas, puntaje máximo y residuos clasificados por tipo.
    /// </summary>
    [HttpGet("{perfilId}")]
    [Authorize]
    public async Task<ActionResult<EstadisticasPerfilDTO>> GetEstadisticas(int perfilId)
    {
        try
        {
            var estadisticas = await _estadisticasService.ObtenerEstadisticasPerfilAsync(perfilId);
            return Ok(estadisticas);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}
