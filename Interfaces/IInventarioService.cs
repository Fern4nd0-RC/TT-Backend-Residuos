using ResiduosBackend.DTO;

namespace ResiduosBackend.Interfaces
{
    /// <summary>
    /// Contrato del inventario por perfil (CU-05).
    /// </summary>
    public interface IInventarioService
    {
        /// <summary>
        /// Devuelve las filas de inventario del jugador con datos denormalizados del ítem.
        /// </summary>
        Task<IEnumerable<InventarioDTO>> ObtenerInventarioPorPerfilAsync(int perfilId);

        /// <summary>
        /// Incrementa la cantidad o crea la fila. Lanza <see cref="KeyNotFoundException"/> si el perfil o el ítem no existen.
        /// </summary>
        Task<InventarioDTO> AgregarItemAsync(AgregarItemDTO dto);

        /// <summary>
        /// Resta cantidad para ítems consumibles. Devuelve <c>false</c> si no aplica o no hay stock (RN-702).
        /// </summary>
        Task<bool> ConsumirItemAsync(ConsumirItemDTO dto);
    }
}
