namespace ResiduosBackend.DTO
{
    /// <summary>
    /// Línea de inventario con datos del ítem para renderizado en cliente (CU-05).
    /// </summary>
    public class InventarioDTO
    {
        /// <summary>Clave primaria de la fila <see cref="ResiduosBackend.Models.Inventario"/>.</summary>
        public int Id { get; set; }

        /// <summary>Referencia al ítem del catálogo.</summary>
        public int ItemId { get; set; }

        /// <summary>Nombre del ítem (denormalizado desde <see cref="ResiduosBackend.Models.Item.Nombre"/>).</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Discriminante <c>Cosmetico</c> u <c>ObjetoDeJuego</c> (Anexo 2).</summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Unidades apiladas en esta fila.</summary>
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Solicitud para agregar o apilar un ítem en el inventario.
    /// </summary>
    public class AgregarItemDTO
    {
        /// <summary>Perfil propietario.</summary>
        public int PerfilId { get; set; }

        /// <summary>Ítem a agregar.</summary>
        public int ItemId { get; set; }

        /// <summary>Cantidad positiva a sumar.</summary>
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Solicitud para consumir un ítem de juego (no aplica a cosméticos, RN-702).
    /// </summary>
    public class ConsumirItemDTO
    {
        /// <summary>Perfil propietario.</summary>
        public int PerfilId { get; set; }

        /// <summary>Ítem a consumir.</summary>
        public int ItemId { get; set; }

        /// <summary>Cantidad a restar.</summary>
        public int Cantidad { get; set; }
    }
}
