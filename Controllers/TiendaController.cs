using Microsoft.AspNetCore.Mvc;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Catálogo de tienda y compra de ítems (RF-505, CU-05).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TiendaController : ControllerBase
    {
        private readonly ITiendaService _tiendaService;

        /// <summary>
        /// Inicializa el controlador con el servicio de tienda.
        /// </summary>
        public TiendaController(ITiendaService tiendaService)
        {
            _tiendaService = tiendaService;
        }

        /// <summary>
        /// Devuelve el catálogo con precios y banderas <c>YaPoseido</c> / <c>NivelSuficiente</c> para habilitar la compra en cliente.
        /// </summary>
        [HttpGet("catalogo/{perfilId}")]
        public async Task<ActionResult<IEnumerable<TiendaItemDTO>>> GetCatalogo(int perfilId)
        {
            try
            {
                var catalogo = await _tiendaService.ObtenerCatalogoAsync(perfilId);
                return Ok(catalogo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Ejecuta la compra con validación de nivel, posesión previa y saldo (fichas o estrellas).
        /// </summary>
        [HttpPost("comprar")]
        public async Task<ActionResult<CompraResultadoDTO>> ComprarItem([FromBody] ComprarItemDTO dto)
        {
            try
            {
                var resultado = await _tiendaService.ComprarItemAsync(dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
