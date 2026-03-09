using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---
// Agregamos la conexión a SQL Server
/*
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Pruebas de base de datos en memoria.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ResiduosDB_Memoria"));
*/

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=datos_prueba.db"));

// Soporte para documentar y probar la API (OpenAPI/Swagger)
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// --- 2. CONSTRUCCIÓN DE LA APP ---
// Esto se ejecuta estrictamente UNA sola vez, después de agregar todos los servicios
var app = builder.Build();

// --- 3. PIPELINE DE PETICIONES HTTP ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

// Arrancamos el servidor
app.Run();