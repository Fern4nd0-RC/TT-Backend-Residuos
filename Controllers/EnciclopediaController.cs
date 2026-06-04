using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Enciclopedia de residuos: catálogo por perfil, detalle y desbloqueos pagados con fichas (CU-06, RF-602).
    /// Todos los endpoints requieren token JWT del dispositivo.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        /// Lista todos los residuos con estado de desbloqueo y costo en fichas para el perfil (RN-601).
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
        /// Obtiene el detalle de un residuo solo si el perfil lo tiene desbloqueado; en caso contrario responde 403 (RN-601).
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
            catch (UnauthorizedAccessException ex)
            {
                // 403 con cuerpo en lugar de Forbid() para garantizar el JSON independientemente del esquema configurado.
                return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Cobra el costo en fichas y registra el desbloqueo. Devuelve 400 si no hay fichas suficientes,
        /// 200 con <c>EsNuevoDesbloqueo = false</c> si ya estaba desbloqueado (RF-602).
        /// </summary>
        [HttpPost("desbloquear")]
        public async Task<ActionResult<DesbloqueoResultadoDTO>> DesbloquearResiduo(
            [FromBody] DesbloquearResiduoDTO dto)
        {
            try
            {
                var resultado = await _enciclopediaService.DesbloquearResiduoAsync(dto);

                if (resultado.FondosInsuficientes)
                {
                    // Devolvemos el DTO completo en el cuerpo para que el cliente pueda mostrar el saldo y el costo.
                    return BadRequest(resultado);
                }

                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
