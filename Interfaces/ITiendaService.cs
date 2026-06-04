using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato de la tienda: catálogo contextual y compra (RF-505, CU-05).
    /// </summary>
    public interface ITiendaService
    {
        /// <summary>
        /// Obtiene ítems con precios y banderas de elegibilidad según nivel e inventario del perfil.
        /// </summary>
        Task<IEnumerable<TiendaItemDTO>> ObtenerCatalogoAsync(int perfilId);

        /// <summary>
        /// Descuenta moneda e inserta el ítem en inventario. Las excepciones comunican reglas de negocio al controlador.
        /// </summary>
        Task<CompraResultadoDTO> ComprarItemAsync(ComprarItemDTO dto);
    }
}
