using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Enciclopedia de residuos: catálogo por perfil, detalle y registro de desbloqueos (CU-06, RF-602).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EnciclopediaController : ControllerBase
    {
        private readonly IEnciclopediaService _enciclopediaService;

        /// <summary>
        /// Inicializa el controlador con el servicio de enciclopedia.
        /// </summary>
        public EnciclopediaController(IEnciclopediaService enciclopediaService)
        {
            _enciclopediaService = enciclopediaService;
        }

        /// <summary>
        /// Lista todos los residuos con estado de desbloqueo para el perfil (RN-601).
        /// </summary>
        [HttpGet("catalogo/{perfilId}")]
        public async Task<ActionResult<IEnumerable<EnciclopediaEntradaDTO>>> GetCatalogo(int perfilId)
        {
            try
            {
                var catalogo = await _enciclopediaService.ObtenerCatalogoAsync(perfilId);
                return Ok(catalogo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el detalle de un residuo solo si el perfil lo tiene desbloqueado; en caso contrario se responde 403 (RN-601).
        /// </summary>
        [HttpGet("detalle/{perfilId}/{residuoId}")]
        public async Task<ActionResult<EnciclopediaDetalleDTO>> GetDetalle(int perfilId, int residuoId)
        {
            try
            {
                var detalle = await _enciclopediaService.ObtenerDetalleAsync(perfilId, residuoId);
                return Ok(detalle);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Registra el desbloqueo al clasificar correctamente; si ya existía, el servicio indica <c>EsNuevoDesbloqueo = false</c> (CU-04 A3).
        /// </summary>
        [HttpPost("desbloquear")]
        [Authorize]
        public async Task<ActionResult<DesbloqueoResultadoDTO>> DesbloquearResiduo(
            [FromBody] DesbloquearResiduoDTO dto)
        {
            try
            {
                var resultado = await _enciclopediaService.DesbloquearResiduoAsync(dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
