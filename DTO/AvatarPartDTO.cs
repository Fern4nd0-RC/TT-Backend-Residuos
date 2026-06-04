namespace ResiduosBackend.DTO
{
    /// <summary>
    /// Representación de una parte del avatar para el cliente.
    /// </summary>
    public class AvatarPartDTO
    {
        /// <summary>Identificador único.</summary>
        public int Id { get; set; }

        /// <summary>Slot: <c>Body</c>, <c>Face</c> o <c>Hat</c>.</summary>
        public string Slot { get; set; } = string.Empty;

        /// <summary>Nombre legible para la UI.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Identificador del recurso para Unity (<c>Resources.Load</c>).</summary>
        public string RecursoUnity { get; set; } = string.Empty;

        /// <summary>Posición en el carrusel.</summary>
        public int Orden { get; set; }
    }

    /// <summary>
    /// Catálogo completo agrupado por slot. Pensado para la pantalla de registro,
    /// que muestra tres carruseles (cuerpo, cara, sombrero) y necesita las listas
    /// ya separadas.
    /// </summary>
    public class AvatarPartsCatalogoDTO
    {
        /// <summary>Opciones de cuerpo (materiales).</summary>
        public List<AvatarPartDTO> Bodies { get; set; } = new();

        /// <summary>Opciones de cara (expresiones).</summary>
        public List<AvatarPartDTO> Faces { get; set; } = new();

        /// <summary>Opciones de sombrero (incluye la opción "Sin sombrero").</summary>
        public List<AvatarPartDTO> Hats { get; set; } = new();

        /// <summary>Opciones de foto de perfil del jugador.</summary>
        public List<AvatarPartDTO> ProfilePictures { get; set; } = new();
    }
}
