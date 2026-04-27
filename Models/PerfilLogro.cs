namespace ResiduosBackend.Models;

/// <summary>
/// Tabla de unión entre perfil y logro desbloqueado.
/// </summary>
public class PerfilLogro
{
    /// <summary>Perfil que obtuvo la insignia.</summary>
    public int PerfilId { get; set; }

    /// <summary>Logro desbloqueado por el perfil.</summary>
    public int LogroId { get; set; }

    /// <summary>Fecha en UTC de obtención del logro.</summary>
    public DateTime FechaObtencion { get; set; } = DateTime.UtcNow;

    /// <summary>Navegación al perfil.</summary>
    public Perfil? Perfil { get; set; }

    /// <summary>Navegación al logro.</summary>
    public Logro? Logro { get; set; }
}
