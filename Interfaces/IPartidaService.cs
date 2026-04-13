using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato para persistir el resultado de una partida multijugador.
    /// </summary>
    public interface IPartidaService
    {
        /// <summary>
        /// Aplica fichas a cada perfil encontrado y guarda una métrica por jugador en una transacción.
        /// </summary>
        /// <returns><c>true</c> si la transacción confirmó; <c>false</c> si se revirtió por error.</returns>
        Task<bool> ProcesarResultadosPartidaAsync(FinalizarPartidaRequestDTO request);
    }
}
