using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTOs;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly AppDbContext _context;

        public PerfilService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PerfilDTO>> ObtenerPerfilesAsync()
        {
            var perfiles = await _context.Perfiles.ToListAsync();
            // Mapeamos la lista de modelos a una lista de DTOs
            return perfiles.Select(p => new PerfilDTO
            {
                Id = p.Id,
                NombreUsuario = p.NombreUsuario,
                Nivel = p.Experiencia,
                Monedas = p.Monedas
            }).ToList();
        }

        public async Task<PerfilDTO> ObtenerPerfilAsync(int id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            if (perfil == null) return null;

            return new PerfilDTO
            {
                Id = perfil.Id,
                NombreUsuario = perfil.NombreUsuario,
                Nivel = perfil.Experiencia,
                Monedas = perfil.Monedas
            };
        }

        public async Task<PerfilDTO> CrearPerfilAsync(Perfil perfilNuevo)
        {
            _context.Perfiles.Add(perfilNuevo);
            await _context.SaveChangesAsync();

            return new PerfilDTO
            {
                Id = perfilNuevo.Id,
                NombreUsuario = perfilNuevo.NombreUsuario,
                Nivel = perfilNuevo.Experiencia,
                Monedas = perfilNuevo.Monedas
            };
        }

        public async Task<PerfilDTO> ActualizarProgresoAsync(int id, PartidaResultado resultado)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            if (perfil == null) return null;

            // Actualización del progreso
            perfil.Monedas += resultado.FichasGanadas;
            perfil.Experiencia = CalcularNivelProgresivo(perfil.Monedas);

            _context.Entry(perfil).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new PerfilDTO
            {
                Id = perfil.Id,
                NombreUsuario = perfil.NombreUsuario,
                Nivel = perfil.Experiencia,
                Monedas = perfil.Monedas
            };
        }

        // El método auxiliar ahora vive en el servicio
        private int CalcularNivelProgresivo(int fichasTotales)
        {
            int nivelActual = 1;
            int costoSiguienteNivel = 100;
            int fichasAcumuladasRequeridas = costoSiguienteNivel;

            while (fichasTotales >= fichasAcumuladasRequeridas && nivelActual < 30)
            {
                nivelActual++;
                costoSiguienteNivel += 50;
                fichasAcumuladasRequeridas += costoSiguienteNivel;
            }

            return nivelActual;
        }
    }
}