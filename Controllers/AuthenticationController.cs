using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Endpoints de autenticación o alta inicial por nombre de usuario.
    /// </summary>
    /// <remarks>
    /// Login de demo por nombre de usuario con emisión de JWT.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPerfilService _perfilService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Crea una instancia del controlador con el contexto de datos.
        /// </summary>
        public AutenticacionController(
            AppDbContext context,
            IPerfilService perfilService,
            IConfiguration configuration)
        {
            _context = context;
            _perfilService = perfilService;
            _configuration = configuration;
        }

        /// <summary>
        /// Busca un perfil por nombre de usuario o lo crea si no existe.
        /// </summary>
        /// <param name="request">Nombre de usuario para autenticar o crear perfil inicial.</param>
        /// <returns>Token JWT y datos de perfil para consumo del cliente.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nombreUsuario = request.NombreUsuario.Trim();
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return BadRequest(new { mensaje = "El nombre de usuario es requerido." });

            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(p => p.NombreUsuario == nombreUsuario);

            if (perfil == null)
            {
                try
                {
                    var perfilCreado = await _perfilService.CrearPerfilAsync(new CrearPerfilDTO
                    {
                        NombreUsuario = nombreUsuario
                    });

                    perfil = await _context.Perfiles.FindAsync(perfilCreado.Id);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { mensaje = ex.Message });
                }
            }

            if (perfil == null)
                return StatusCode(500, new { mensaje = "No se pudo completar el inicio de sesión." });

            var expiraEnUtc = DateTime.UtcNow.AddMinutes(
                int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var minutos) ? minutos : 120);

            var token = GenerarToken(perfil.Id, perfil.NombreUsuario, expiraEnUtc);
            var perfilDto = await _perfilService.ObtenerPerfilAsync(perfil.Id);
            if (perfilDto == null)
                return StatusCode(500, new { mensaje = "No se pudo recuperar el perfil autenticado." });

            return Ok(new LoginResponseDTO
            {
                Token = token,
                ExpiraEnUtc = expiraEnUtc,
                Perfil = perfilDto
            });
        }

        private string GenerarToken(int perfilId, string nombreUsuario, DateTime expiraEnUtc)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Falta configuración Jwt:Key.");
            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Falta configuración Jwt:Issuer.");
            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("Falta configuración Jwt:Audience.");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, perfilId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, nombreUsuario),
                new(ClaimTypes.NameIdentifier, perfilId.ToString()),
                new(ClaimTypes.Name, nombreUsuario),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiraEnUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
