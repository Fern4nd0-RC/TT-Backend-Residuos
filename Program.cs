using Microsoft.EntityFrameworkCore;
using ResiduosBackend.Data;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios y cadena de conexión definida en appsettings (MariaDB).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// CORS: el cliente Unity suele originar peticiones desde otro origen; sin esta política el navegador bloquearía las respuestas.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnityApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ITiendaService, TiendaService>();
builder.Services.AddScoped<IEnciclopediaService, EnciclopediaService>();
builder.Services.AddScoped<IPartidaService, PartidaService>();

// ReferenceHandler.IgnoreCycles evita fallos de serialización cuando una entidad se serializa con navegaciones circulares (p. ej. Perfil ↔ Inventario).
builder.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// La política CORS debe ejecutarse antes de MapControllers para que aplique a todas las rutas de la API.
app.UseCors("AllowUnityApp");

app.MapControllers();

app.Run();
