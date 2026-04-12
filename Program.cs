using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---

// Obtenemos la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registramos el contexto para usar SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Habilitar CORS (Súper importante para que Unity pueda consumir la API sin bloqueos)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnityApp", policy =>
    {
        policy.AllowAnyOrigin()    // En producción, cambia esto por la IP/Dominio de tu juego
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();

// Soporte para Controllers y evitar ciclos en el JSON
builder.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 2. PIPELINE DE PETICIONES HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar la política de CORS que creamos arriba antes de mapear los controladores
app.UseCors("AllowUnityApp"); 

app.MapControllers();

app.Run();