namespace ResiduosBackend.Models;

/// <summary>
/// Agregado legado no mapeado en <see cref="ResiduosBackend.Data.AppDbContext"/>; el flujo actual usa <see cref="ResiduosBackend.DTO.JugadorResultadoDTO"/> y <see cref="PartidaMetrica"/>.
/// </summary>
/// <remarks>Se mantiene solo si algún módulo externo aún lo referencia; en caso contrario puede eliminarse.</remarks>
public class PartidaResultado
{
    /// <summary>Perfil al que aplican las fichas.</summary>
    public int PerfilId { get; set; }

    /// <summary>Fichas ganadas en la partida.</summary>
    public int FichasGanadas { get; set; }
}
