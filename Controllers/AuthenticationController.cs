using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.Models;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Endpoints de autenticación o alta inicial por nombre de usuario.
    /// </summary>
    /// <remarks>
    /// Nota: la creación de perfil aquí no pasa por <c>PerfilService</c>; conviene alinearlo con las reglas RN-201 si este flujo sigue en uso.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Crea una instancia del controlador con el contexto de datos.
        /// </summary>
        public AutenticacionController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca un perfil por nombre de usuario o lo crea si no existe.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario enviado en el cuerpo de la petición.</param>
        /// <returns>La entidad <see cref="Perfil"/> persistida o existente.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Perfil>> Login([FromBody] string nombreUsuario)
        {
            if (string.IsNullOrEmpty(nombreUsuario))
                return BadRequest("El nombre de usuario es requerido.");

            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(p => p.NombreUsuario == nombreUsuario);

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
