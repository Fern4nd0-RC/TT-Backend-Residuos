using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces;

/// <summary>
/// Contrato de consulta y desbloqueo automático de logros.
/// </summary>
public interface ILogroService
{
    /// <summary>
    /// Verifica y registra logros desbloqueados por XP actual del perfil.
    /// </summary>
    Task VerificarYDesbloquearLogrosAsync(int perfilId, bool guardarCambios = true);

    /// <summary>
    /// Lista todos los logros con estado bloqueado/desbloqueado para un perfil.
    /// </summary>
    Task<IEnumerable<LogroEstadoDTO>> ObtenerLogrosPorPerfilAsync(int perfilId);
}
