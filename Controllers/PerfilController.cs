using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.Models;

namespace ResiduosBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PerfilController : ControllerBase
{
    private readonly AppDbContext _context;
    public PerfilController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // 1. GET: api/Perfil
    // Obtiene la lista de todos los perfiles registrados
    // =========================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Perfil>>> GetPerfiles()
    {
        return await _context.Perfiles.ToListAsync();
    }

    // =========================================================
    // 2. POST: api/Perfil
    // Crea un nuevo perfil de jugador
    // Se utiliza cuando el usuario ingresa su nombre por primera vez
    // =========================================================
    [HttpPost]
    public async Task<ActionResult<Perfil>> CrearPerfil(Perfil perfil)
    {
        // Agrega el perfil a la base de datos
        _context.Perfiles.Add(perfil);

        // Guarda los cambios
        await _context.SaveChangesAsync();

        // Devuelve el perfil creado junto con la ruta para consultarlo
        return CreatedAtAction(
            nameof(GetPerfiles),
            new { id = perfil.Id },
            perfil
        );
    }

    // =========================================================
    // 3. PUT: api/Perfil/{id}/progreso
    // Actualiza el progreso del jugador después de un minijuego
    // =========================================================
    [HttpPut("{id}/progreso")]
    public async Task<IActionResult> ActualizarProgreso(int id, PartidaResultado resultado)
    {
        // Validar que el ID recibido coincida con el perfil del resultado
        if (id != resultado.PerfilId)
            return BadRequest("El ID del perfil no coincide.");

        // Buscar el perfil en la base de datos
        var perfil = await _context.Perfiles.FindAsync(id);

        if (perfil == null)
            return NotFound("Perfil no encontrado.");

        // =====================================================
        // Actualización del progreso
        // =====================================================

        // Sumamos las fichas obtenidas en la partida
        perfil.FichasReciclaje += resultado.FichasGanadas;

        // Calculamos el nuevo nivel con base en las fichas acumuladas
        perfil.Nivel = CalcularNivelProgresivo(perfil.FichasReciclaje);

        // Marcamos la entidad como modificada
        _context.Entry(perfil).State = EntityState.Modified;

        // Guardamos cambios en la base de datos
        await _context.SaveChangesAsync();

        // Respuesta de confirmación
        return Ok(new
        {
            mensaje = "Progreso actualizado",
            nivelActual = perfil.Nivel,
            fichasTotales = perfil.FichasReciclaje
        });
    }

    // =========================================================
    // 4. Endpoint GET por ID: 
    // Para hacer el "Login" o cargar la partida seleccionada
    // =========================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<Perfil>> GetPerfil(int id)
    {
        // Buscamos en la base de datos el perfil que coincida con el ID que manda Unity
        var perfil = await _context.Perfiles.FindAsync(id);

        if (perfil == null)
        {
            return NotFound("No se encontró el perfil de este jugador.");
        }

        // Si lo encuentra, devuelve todos los datos (Nivel, Fichas, Nombre)
        return perfil;
    }

    // =========================================================
    // MÉTODO AUXILIAR
    // Calcula el nivel del jugador según sus fichas acumuladas
    // usando una curva progresiva de dificultad
    // =========================================================
    private int CalcularNivelProgresivo(int fichasTotales)
    {
        int nivelActual = 1;

        // Fichas necesarias para pasar del nivel 1 al 2
        int costoSiguienteNivel = 100;

        // Total acumulado requerido para subir de nivel
        int fichasAcumuladasRequeridas = costoSiguienteNivel;

        // Mientras el jugador tenga fichas suficientes
        // y no supere el nivel máximo (30)
        while (fichasTotales >= fichasAcumuladasRequeridas && nivelActual < 30)
        {
            nivelActual++;
            costoSiguienteNivel += 50;
            fichasAcumuladasRequeridas += costoSiguienteNivel;
        }

        return nivelActual;
    }
}