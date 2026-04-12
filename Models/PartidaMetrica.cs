using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

public class PartidaMetrica
{
    [Key]
    public int Id { get; set; }
    public int PerfilId { get; set; }
    public int PuntuacionObtenida { get; set; }
    public int ResiduosClasificadosCorrectamente { get; set; }
    public DateTime FechaPartida { get; set; } = DateTime.UtcNow;

    public Perfil? Perfil { get; set; }
}