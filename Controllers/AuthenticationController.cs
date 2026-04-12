using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.Models;

namespace ResiduosBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AutenticacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Perfil>> Login([FromBody] string nombreUsuario)
        {
            if (string.IsNullOrEmpty(nombreUsuario))
                return BadRequest("El nombre de usuario es requerido.");

            // Buscar si el perfil ya existe
            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(p => p.NombreUsuario == nombreUsuario);

            // Si no existe, lo creamos (Persistencia inicial)
            if (perfil == null)
            {
                perfil = new Perfil 
                { 
                    NombreUsuario = nombreUsuario,
                    Monedas = 0,
                    Experiencia = 0
                };
                _context.Perfiles.Add(perfil);
                await _context.SaveChangesAsync();
            }

            return Ok(perfil);
        }
    }
}