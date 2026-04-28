using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResiduosBackend.Data;
using ResiduosBackend.Interfaces;
using ResiduosBackend.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios y cadena de conexión definida en appsettings (MariaDB).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

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
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (allowedOrigins is { Length: > 0 })
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Fallback para demo/local si no se define configuración de orígenes.
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("No se encontró la configuración 'Jwt:Key'.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ITiendaService, TiendaService>();
builder.Services.AddScoped<IEnciclopediaService, EnciclopediaService>();
builder.Services.AddScoped<IPartidaService, PartidaService>();
builder.Services.AddScoped<IEstadisticasService, EstadisticasService>();
builder.Services.AddScoped<ILogroService, LogroService>();

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

app.UseExceptionHandler();
// app.UseHttpsRedirection();

// La política CORS debe ejecutarse antes de MapControllers para que aplique a todas las rutas de la API.
app.UseCors("AllowUnityApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
