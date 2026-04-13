using Microsoft.AspNetCore.Mvc;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Consulta y modificación del inventario por perfil (CU-05).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        /// <summary>
        /// Inicializa el controlador con el servicio de inventario.
        /// </summary>
        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        /// <summary>
        /// Obtiene todas las filas de inventario de un jugador.
        /// </summary>
        [HttpGet("perfil/{perfilId}")]
        public async Task<ActionResult<IEnumerable<InventarioDTO>>> GetInventarioDelJugador(int perfilId)
        {
            var inventario = await _inventarioService.ObtenerInventarioPorPerfilAsync(perfilId);
            return Ok(inventario);
        }

        /// <summary>
        /// Agrega cantidad a un ítem o crea la fila de inventario (recompensas, tienda, etc.).
        /// </summary>
        [HttpPost("agregar")]
        public async Task<ActionResult<InventarioDTO>> AgregarItem([FromBody] AgregarItemDTO dto)
        {
            if (dto.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a cero." });

            try
            {
                var resultado = await _inventarioService.AgregarItemAsync(dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Reduce la cantidad de un ítem de tipo <c>ObjetoDeJuego</c>. Los cosméticos no son consumibles (RN-702).
        /// </summary>
        [HttpPost("consumir")]
        public async Task<IActionResult> ConsumirItem([FromBody] ConsumirItemDTO dto)
        {
            if (dto.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a cero." });

            var exito = await _inventarioService.ConsumirItemAsync(dto);

            if (!exito)
                return BadRequest(new
                {
                    mensaje = "No se pudo consumir el ítem. " +
                              "Verifica que el jugador tenga suficiente cantidad y que no sea un ítem cosmético (RN-702)."
                });

            return Ok(new { mensaje = "Ítem consumido correctamente." });
        }
    }
}
