using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.Models;

namespace ResiduosBackend.Controllers;

// Esta línea define que la ruta base será: http://localhost:[puerto]/api/Perfil
[Route("api/[controller]")]
[ApiController]
public class PerfilController : ControllerBase
{
    private readonly AppDbContext _context;

    // Inyectamos nuestra base de datos (en memoria por ahora)
    public PerfilController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Endpoint GET: Para obtener todos los perfiles guardados
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Perfil>>> GetPerfiles()
    {
        return await _context.Perfiles.ToListAsync();
    }

    // 2. Endpoint POST: Para crear un nuevo perfil desde la "Pantalla de ingresar nombre"
    [HttpPost]
    public async Task<ActionResult<Perfil>> CrearPerfil(Perfil perfil)
    {
        _context.Perfiles.Add(perfil);
        await _context.SaveChangesAsync(); // Guarda en la memoria RAM

        return CreatedAtAction(nameof(GetPerfiles), new { id = perfil.Id }, perfil);
    }
}