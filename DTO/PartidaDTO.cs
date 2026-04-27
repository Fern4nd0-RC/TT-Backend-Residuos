namespace ResiduosBackend.DTO
{
    /// <summary>
    /// Cuerpo del cierre de partida con resultados por jugador (hasta cuatro perfiles).
    /// </summary>
    public class FinalizarPartidaRequestDTO
    {
        /// <summary>Lista de resultados parciales enviados por el cliente al finalizar la sesión.</summary>
        public List<JugadorResultadoDTO> ResultadosJugadores { get; set; } = new();
    }

    /// <summary>
    /// Métricas y recompensas de un jugador al cerrar la partida.
    /// </summary>
    public class JugadorResultadoDTO
    {
        /// <summary>Perfil al que se aplican fichas y métricas.</summary>
        public int PerfilId { get; set; }

        /// <summary>Fichas a acreditar al perfil existente.</summary>
        public int FichasGanadas { get; set; }

        /// <summary>Experiencia a sumar al perfil al cerrar la partida (misma regla que <c>PUT …/progreso</c>).</summary>
        public int XpGanado { get; set; }

        /// <summary>Puntuación registrada en el histórico de partida.</summary>
        public int PuntuacionObtenida { get; set; }

        /// <summary>Conteo de aciertos de clasificación persistido en métricas.</summary>
        public int ResiduosClasificadosCorrectamente { get; set; }

        /// <summary>Conteo de aciertos de residuos orgánicos (opcional para retrocompatibilidad).</summary>
        public int ResiduosOrganicosClasificados { get; set; } = 0;

        /// <summary>Conteo de aciertos de residuos inorgánicos (opcional para retrocompatibilidad).</summary>
        public int ResiduosInorganicosClasificados { get; set; } = 0;
    }
}
