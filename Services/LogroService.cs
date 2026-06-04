using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services;

/// <summary>
/// Gestión de logros: consulta de estado y desbloqueo automático por XP.
/// </summary>
public class LogroService : ILogroService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa el servicio de logros.
    /// </summary>
    public LogroService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task VerificarYDesbloquearLogrosAsync(int perfilId, bool guardarCambios = true)
    {
        var perfil = await _context.Perfiles.FindAsync(perfilId)
            ?? throw new KeyNotFoundException($"No se encontró el perfil con ID {perfilId}.");

        var logrosDisponibles = await _context.Logros
            .Where(l => l.RequisitoXP <= perfil.Experiencia)
            .Select(l => l.Id)
            .ToListAsync();

        if (logrosDisponibles.Count == 0)
            return;

        var logrosYaDesbloqueados = await _context.PerfilLogros
            .Where(pl => pl.PerfilId == perfilId)
            .Select(pl => pl.LogroId)
            .ToListAsync();

        var nuevosLogros = logrosDisponibles.Except(logrosYaDesbloqueados).ToList();
        if (nuevosLogros.Count == 0)
            return;

        var fechaActual = DateTime.UtcNow;
        foreach (var logroId in nuevosLogros)
        {
            _context.PerfilLogros.Add(new PerfilLogro
            {
                PerfilId = perfilId,
                LogroId = logroId,
                FechaObtencion = fechaActual
            });
        }

        if (guardarCambios)
            await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<LogroEstadoDTO>> ObtenerLogrosPorPerfilAsync(int perfilId)
    {
        var perfilExiste = await _context.Perfiles.AnyAsync(p => p.Id == perfilId);
        if (!perfilExiste)
            throw new KeyNotFoundException($"No se encontró el perfil con ID {perfilId}.");

        var desbloqueos = await _context.PerfilLogros
            .Where(pl => pl.PerfilId == perfilId)
            .ToDictionaryAsync(pl => pl.LogroId, pl => pl.FechaObtencion);

        var logros = await _context.Logros
            .OrderBy(l => l.RequisitoXP)
            .ThenBy(l => l.Id)
            .ToListAsync();

        return logros.Select(l =>
        {
            var desbloqueado = desbloqueos.TryGetValue(l.Id, out var fecha);
            return new LogroEstadoDTO
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                ImagenUrl = l.ImagenUrl,
                RequisitoXP = l.RequisitoXP,
                Desbloqueado = desbloqueado,
                FechaObtencion = desbloqueado ? fecha : null
            };
        }).ToList();
    }
}
