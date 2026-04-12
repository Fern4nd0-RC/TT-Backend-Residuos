using System.ComponentModel.DataAnnotations;

namespace ResiduosBackend.Models;

public class Residuo
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
}