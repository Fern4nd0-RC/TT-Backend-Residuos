namespace ResiduosBackend.DTOs
{
    public class InventarioDTO
    {
        public int IdRegistro { get; set; } // El ID de la tabla Inventario
        public int ItemId { get; set; }
        public string NombreItem { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        // Puedes agregar más datos visuales para Unity si los tienes en la tabla Item, como "IconoUrl" o "Tipo"
    }
}