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

        // ===== Personalización del avatar =====
        // Solo los IDs. Unity descarga el catálogo (GET /api/AvatarParts) al iniciar
        // sesión y resuelve qué material/prefab cargar para cada ID.

        /// <summary>ID de la parte de cuerpo seleccionada.</summary>
        public int BodyPartId { get; set; }

        /// <summary>ID de la parte de cara seleccionada.</summary>
        public int FacePartId { get; set; }

        /// <summary>ID del sombrero seleccionado ("Sin sombrero" es una opción del catálogo).</summary>
        public int HatPartId { get; set; }

        /// <summary>ID de la foto de perfil estática.</summary>
        public int ProfilePictureId { get; set; } // NUEVO

        /// <summary>Fecha de creación en UTC.</summary>
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// Datos permitidos al crear un perfil. Los tres IDs de avatar son OPCIONALES en
    /// el cuerpo: si vienen en 0, el servicio usa los defaults del catálogo. Esto
    /// permite que un cliente "simple" cree perfiles sin elegir avatar (compatibilidad)
    /// y que el cliente actual envíe los tres IDs cuando el usuario sí personaliza.
    /// </summary>
    public class CrearPerfilDTO
    {
        /// <summary>Nombre de usuario en la pantalla de selección (RF-101).</summary>
        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>ID de la parte de cuerpo. 0 = usar default.</summary>
        public int BodyPartId { get; set; } = 0;

        /// <summary>ID de la parte de cara. 0 = usar default.</summary>
        public int FacePartId { get; set; } = 0;

        /// <summary>ID del sombrero. 0 = usar default ("Sin sombrero" si existe).</summary>
        public int HatPartId { get; set; } = 0;

        /// <summary>ID de la foto de perfil. 0 = usar default.</summary>
        public int ProfilePictureId { get; set; } = 0; // NUEVO
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
