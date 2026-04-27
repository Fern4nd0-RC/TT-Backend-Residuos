using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Finalización de partida multijugador y persistencia de resultados.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PartidaController : ControllerBase
    {
        private readonly IPartidaService _partidaService;

        /// <summary>
        /// Inicializa el controlador con el servicio de partida.
        /// </summary>
        public PartidaController(IPartidaService partidaService)
        {
            _partidaService = partidaService;
        }

        /// <summary>
        /// Procesa los resultados de uno o varios jugadores: acumula fichas y guarda métricas por partida.
        /// </summary>
        [HttpPost("finalizar")]
        [Authorize]
        public async Task<IActionResult> FinalizarPartida([FromBody] FinalizarPartidaRequestDTO request)
        {
            if (request == null || !request.ResultadosJugadores.Any())
            {
                return BadRequest("No se enviaron resultados válidos.");
            }
            if (request.ResultadosJugadores.Any(r =>
                    r.XpGanado < 0 ||
                    r.FichasGanadas < 0 ||
                    r.PuntuacionObtenida < 0 ||
                    r.ResiduosClasificadosCorrectamente < 0 ||
                    r.ResiduosOrganicosClasificados < 0 ||
                    r.ResiduosInorganicosClasificados < 0))
            {
                return BadRequest(new { mensaje = "No se aceptan métricas negativas en resultados de partida." });
            }

            var exito = await _partidaService.ProcesarResultadosPartidaAsync(request);

            if (exito)
            {
                return Ok(new { mensaje = "Resultados de la partida procesados correctamente." });
            }

            return StatusCode(500, "Ocurrió un error al procesar los resultados en el servidor.");
        }
    }
}
