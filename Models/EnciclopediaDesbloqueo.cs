namespace ResiduosBackend.Models;

public class EnciclopediaDesbloqueo
{
    public int PerfilId { get; set; }
    public int ResiduoId { get; set; }
    public DateTime FechaDesbloqueo { get; set; } = DateTime.UtcNow;

    public Perfil? Perfil { get; set; }
    public Residuo? Residuo { get; set; }
}