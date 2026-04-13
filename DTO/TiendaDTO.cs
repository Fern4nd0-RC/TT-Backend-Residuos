namespace ResiduosBackend.DTO
{
    /// <summary>
    /// Ítem del catálogo de tienda con información de precio y elegibilidad (RF-505).
    /// </summary>
    public class TiendaItemDTO
    {
        /// <summary>Identificador del ítem (<see cref="ResiduosBackend.Models.Item.Id"/>).</summary>
        public int Id { get; set; }

        /// <summary>Nombre para mostrar.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Descripción comercial.</summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>Tipo de ítem (cosmético o de juego).</summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Clave del recurso gráfico en el cliente.</summary>
        public string NombreSprite { get; set; } = string.Empty;

        /// <summary>Precio en fichas; cero indica que no aplica pago con fichas (RN-702).</summary>
        public int CostoFichas { get; set; }

        /// <summary>Precio en estrellas; cero indica que no aplica pago con estrellas.</summary>
        public int CostoEstrellas { get; set; }

        /// <summary>Nivel mínimo del perfil para habilitar la compra (RN-302).</summary>
        public int NivelDesbloqueo { get; set; }

        /// <summary>Indica si el perfil ya posee al menos una unidad del ítem.</summary>
        public bool YaPoseido { get; set; }

        /// <summary>Indica si el nivel actual del perfil cumple el umbral <see cref="NivelDesbloqueo"/>.</summary>
        public bool NivelSuficiente { get; set; }
    }

    /// <summary>
    /// Cuerpo de la solicitud de compra (CU-05).
    /// </summary>
    public class ComprarItemDTO
    {
        /// <summary>Comprador.</summary>
        public int PerfilId { get; set; }

        /// <summary>Ítem a adquirir.</summary>
        public int ItemId { get; set; }
    }

    /// <summary>
    /// Respuesta exitosa de compra con saldos actualizados y la fila de inventario creada.
    /// </summary>
    public class CompraResultadoDTO
    {
        /// <summary>Mensaje orientado al usuario.</summary>
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>Fichas restantes tras el cargo.</summary>
        public int FichasRestantes { get; set; }

        /// <summary>Estrellas restantes tras el cargo.</summary>
        public int EstrellasRestantes { get; set; }

        /// <summary>Registro de inventario generado por la compra.</summary>
        public InventarioDTO ItemAgregado { get; set; } = null!;
    }
}
