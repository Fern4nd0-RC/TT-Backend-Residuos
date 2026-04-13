using ResiduosBackend.Models;

namespace ResiduosBackend.Services;

/// <summary>
/// Cálculo de nivel y aplicación de experiencia compartida entre <see cref="PerfilService"/> y <see cref="PartidaService"/> (RN-703).
/// </summary>
internal static class PerfilProgresoMath
{
    internal const int NivelMaximo = 30;

    /// <summary>
    /// Progresión: coste acumulado sube 50 XP por cada nivel (100 para 1→2, luego 150, 200, …).
    /// </summary>
    internal static int CalcularNivel(int xpTotal)
    {
        int nivel = 1;
        int xpRequerida = 100;
        int incremento = 50;
        int xpAcumuladaRequerida = xpRequerida;

        while (xpTotal >= xpAcumuladaRequerida && nivel < NivelMaximo)
        {
            nivel++;
            xpRequerida += incremento;
            xpAcumuladaRequerida += xpRequerida;
        }

        return nivel;
    }

    /// <summary>
    /// Suma experiencia y recalcula nivel. En nivel máximo no se acumula más XP (RN-703).
    /// </summary>
    internal static void AplicarExperiencia(Perfil perfil, int xpGanado)
    {
        if (xpGanado <= 0 || perfil.Nivel >= NivelMaximo)
            return;

        perfil.Experiencia += xpGanado;
        perfil.Nivel = CalcularNivel(perfil.Experiencia);
        if (perfil.Nivel >= NivelMaximo)
            perfil.Nivel = NivelMaximo;
    }
}
