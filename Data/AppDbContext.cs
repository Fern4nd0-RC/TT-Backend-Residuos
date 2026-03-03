using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Models;

namespace ResiduosBackend.Data;

public class AppDbContext : DbContext
{
    // Constructor que recibe las opciones de conexión
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Aquí registramos nuestra clase Perfil como una tabla llamada "Perfiles"
    public DbSet<Perfil> Perfiles { get; set; }
}