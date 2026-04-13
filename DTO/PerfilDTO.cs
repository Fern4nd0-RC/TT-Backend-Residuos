using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.DTO
{
    /// <summary>
    /// Representación de un perfil expuesta por la API (lectura).
    /// </summary>
    public class PerfilDTO
    {
        /// <summary>Identificador único del perfil.</summary>
        public int Id { get; set; }

        /// <summary>Nombre visible del jugador.</summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>Nivel derivado de la experiencia acumulada (RN-703).</summary>
        public int Nivel { get; set; }

        /// <summary>Experiencia total usada para calcular nivel y barra de progreso (RF-502).</summary>
        public int Experiencia { get; set; }

        /// <summary>Fichas de reciclaje (economía principal, RN-701).</summary>
        public int Monedas { get; set; }

        /// <summary>Estrellas de sostenibilidad (objetivo de victoria, RN-404).</summary>
        public int EstrellaSostenibilidad { get; set; }

        /// <summary>Fecha de creación en UTC.</summary>
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// Datos permitidos al crear un perfil; evita que el cliente envíe identificadores o fechas de servidor.
    /// </summary>
    public class CrearPerfilDTO
    {
        /// <summary>Nombre de usuario único en la pantalla de selección (RF-101).</summary>
        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;
    }

    /// <summary>
    /// Incrementos de progreso tras un minijuego (RF-103, RN-704). En JSON use camelCase: <c>perfilId</c>, <c>xpGanado</c>, <c>fichasGanadas</c>, <c>estrellasGanadas</c>.
    /// </summary>
    public class ActualizarProgresoDTO
    {
        /// <summary>Debe coincidir con el identificador de perfil en la ruta del endpoint.</summary>
        public int PerfilId { get; set; }

        /// <summary>Experiencia a sumar según desempeño (RNF-603). No usar nombres alternativos como <c>experienciaAdicional</c>: no se enlazan al modelo.</summary>
        public int XpGanado { get; set; }

        /// <summary>Fichas a sumar (RF-401, RN-707).</summary>
        public int FichasGanadas { get; set; }

        /// <summary>Estrellas a sumar en escenarios especiales de tablero (RN-404).</summary>
        public int EstrellasGanadas { get; set; } = 0;
    }
}
