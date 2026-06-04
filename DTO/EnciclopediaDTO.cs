namespace ResiduosBackend.DTO
{
    /// <summary>
    /// Entrada de la lista de enciclopedia: combina datos del residuo y estado de desbloqueo (CU-06).
    /// </summary>
    public class EnciclopediaEntradaDTO
    {
        /// <summary>Identificador del residuo.</summary>
        public int Id { get; set; }

        /// <summary>Nombre legible solo si está desbloqueado; vacío en caso contrario para no filtrar contenido (RN-601).</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Categoría para agrupación en interfaz (RN-602).</summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>Sprite del residuo o cadena vacía si está bloqueado.</summary>
        public string NombreSprite { get; set; } = string.Empty;

        /// <summary>Indica si el perfil ya desbloqueó esta ficha.</summary>
        public bool Desbloqueado { get; set; }

        /// <summary>Fecha del primer desbloqueo; nulo mientras siga bloqueado.</summary>
        public DateTime? FechaDesbloqueo { get; set; }

        /// <summary>Costo en fichas para desbloquear la entrada; visible aunque la ficha esté bloqueada para que la UI lo muestre.</summary>
        public int CostoFichas { get; set; }
    }

    /// <summary>
    /// Detalle completo de un residuo ya desbloqueado (RNF-404).
    /// </summary>
    public class EnciclopediaDetalleDTO
    {
        /// <summary>Identificador del residuo.</summary>
        public int Id { get; set; }

        /// <summary>Nombre común del residuo.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Categoría principal.</summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>Subcategoría para residuos avanzados.</summary>
        public string Subcategoria { get; set; } = string.Empty;

        /// <summary>Texto explicativo para el público infantil.</summary>
        public string DescripcionParaNinos { get; set; } = string.Empty;

        /// <summary>Dato curioso mostrado en la ficha.</summary>
        public string DatoCurioso { get; set; } = string.Empty;

        /// <summary>Recurso gráfico en el cliente.</summary>
        public string NombreSprite { get; set; } = string.Empty;

        /// <summary>Momento en que el perfil desbloqueó esta entrada.</summary>
        public DateTime FechaDesbloqueo { get; set; }
    }

    /// <summary>
    /// Solicitud para registrar un desbloqueo pagado con fichas (RF-602).
    /// </summary>
    public class DesbloquearResiduoDTO
    {
        /// <summary>Perfil que obtiene el desbloqueo y paga las fichas.</summary>
        public int PerfilId { get; set; }

        /// <summary>Residuo que se desea desbloquear.</summary>
        public int ResiduoId { get; set; }
    }

    /// <summary>
    /// Resultado de la operación de desbloqueo: indica si fue nueva, si faltaron fichas y el saldo final del perfil.
    /// </summary>
    public class DesbloqueoResultadoDTO
    {
        /// <summary><c>true</c> si se insertó un nuevo registro de desbloqueo y se cobró el costo.</summary>
        public bool EsNuevoDesbloqueo { get; set; }

        /// <summary>
        /// <c>true</c> si la operación se rechazó porque el perfil no tenía fichas suficientes.
        /// Cuando esto sucede el HTTP es 400 Bad Request y no se modifica nada en BD.
        /// </summary>
        public bool FondosInsuficientes { get; set; }

        /// <summary>Texto informativo para el cliente.</summary>
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// Detalle del residuo cuando aplica mostrarlo: desbloqueo nuevo, o ya estaba desbloqueado (no nulo en ambos casos).
        /// Nulo solo cuando el cobro no procedió por fondos insuficientes.
        /// </summary>
        public EnciclopediaDetalleDTO? Residuo { get; set; }

        /// <summary>Saldo de fichas del perfil tras la operación. Permite al cliente refrescar la UI sin otro fetch.</summary>
        public int FichasRestantes { get; set; }
    }
}
