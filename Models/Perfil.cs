using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

public class Perfil
{
    // Clave primaria para SQL Server
    [Key]
    public int Id { get; set; }

    // El nombre que el usuario ingresará en la "Pantalla de ingresar nombre"
    [Required]
    [MaxLength(50)]
    public string NombreJugador { get; set; } = string.Empty;

    // Estadísticas base del jugador al iniciar
    public int Nivel { get; set; } = 1;

    public int FichasReciclaje { get; set; } = 0;
}