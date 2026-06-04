using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResiduosBackend.Controllers
{
    /// <summary>
    /// Autenticación por dispositivo (sin datos personales): identifica un equipo
    /// por su GUID y devuelve los perfiles registrados en él.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPerfilService _perfilService;
        private readonly IConfiguration _configuration;

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
        /// Identifica el dispositivo por su GUID (lo registra si es nuevo) y devuelve
        /// el token del dispositivo junto con la lista de perfiles existentes.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deviceId = request.DeviceId?.Trim();
            if (string.IsNullOrWhiteSpace(deviceId))
                return BadRequest(new { mensaje = "El identificador del dispositivo es requerido." });

            // Get or create: registrar el dispositivo si es la primera conexión.
            var dispositivo = await _context.Dispositivos.FindAsync(deviceId);
            if (dispositivo == null)
            {
                dispositivo = new Dispositivo
                {
                    Id = deviceId,
                    FechaRegistro = DateTime.UtcNow
                };
                _context.Dispositivos.Add(dispositivo);
                await _context.SaveChangesAsync();
            }

            var expiraEnUtc = DateTime.UtcNow.AddMinutes(
                int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var minutos) ? minutos : 120);

            var token = GenerarToken(deviceId, expiraEnUtc);

            var perfiles = await _perfilService.ObtenerPerfilesPorDispositivoAsync(deviceId);

            return Ok(new LoginResponseDTO
            {
                Token = token,
                ExpiraEnUtc = expiraEnUtc,
                Perfiles = perfiles.ToList()
            });
        }

        private string GenerarToken(string deviceId, DateTime expiraEnUtc)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Falta configuración Jwt:Key.");
            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Falta configuración Jwt:Issuer.");
            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("Falta configuración Jwt:Audience.");

            var claims = new List<Claim>
            {
                // El "subject" del token es el dispositivo, no una persona.
                new(JwtRegisteredClaimNames.Sub, deviceId),
                new(ClaimTypes.NameIdentifier, deviceId),
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