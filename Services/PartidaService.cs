using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Models;

namespace ResiduosBackend.Services
{
    /// <summary>
    /// Persistencia transaccional del cierre de partida: fichas al perfil y fila de métricas por jugador.
    /// </summary>
    public class PartidaService : IPartidaService
    {
        private readonly AppDbContext _context;
        private readonly ILogroService _logroService;

        /// <summary>
        /// Crea el servicio con el contexto de datos inyectado.
        /// </summary>
        public PartidaService(AppDbContext context, ILogroService logroService)
        {
            _context = context;
            _logroService = logroService;
        }

        /// <inheritdoc />
        public async Task<bool> ProcesarResultadosPartidaAsync(FinalizarPartidaRequestDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var resultado in request.ResultadosJugadores)
                {
                    var perfil = await _context.Perfiles.FindAsync(resultado.PerfilId);

                    if (perfil != null)
                    {
                        perfil.Monedas += resultado.FichasGanadas;
                        PerfilProgresoMath.AplicarExperiencia(perfil, resultado.XpGanado);

                        var metrica = new PartidaMetrica
                        {
                            PerfilId = resultado.PerfilId,
                            PuntuacionObtenida = resultado.PuntuacionObtenida,
                            ResiduosClasificadosCorrectamente = resultado.ResiduosClasificadosCorrectamente,
                            ResiduosOrganicosClasificados = resultado.ResiduosOrganicosClasificados,
                            ResiduosInorganicosClasificados = resultado.ResiduosInorganicosClasificados,
                            FechaPartida = DateTime.UtcNow
                        };

                        _context.PartidaMetricas.Add(metrica);
                        await _logroService.VerificarYDesbloquearLogrosAsync(resultado.PerfilId, guardarCambios: false);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
