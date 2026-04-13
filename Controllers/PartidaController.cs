using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> FinalizarPartida([FromBody] FinalizarPartidaRequestDTO request)
        {
            if (request == null || !request.ResultadosJugadores.Any())
            {
                return BadRequest("No se enviaron resultados válidos.");
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
