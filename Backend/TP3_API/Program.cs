using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TP3_API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Serilog leyendo desde appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuración de Entity Framework Core con MariaDB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ConexionDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConexionDB"))
    ));

// ** Configurar CORS para permitir peticiones desde Blazor WebAssembly **
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Configurar Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "ClaveSecretaSuperSeguraTP3_2026!ConSuficienteLongitud12345"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ** Activar la política de CORS **
app.UseCors("AllowBlazorApp");

// ** NUEVO: Middleware global de auditoría, manejo de errores y persistencia en base de datos **
app.UseMiddleware<TP3_API.Middlewares.AuditoriaMiddleware>();

// 3. Middlewares de Seguridad
app.UseAuthentication();
app.UseAuthorization();

// 4. Mapear los controladores creados
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    // Forzamos un error controlado para probar el middleware
    throw new Exception("¡Prueba de fuego del sistema de auditoría y logs!");

    // (El resto del código de weatherforecast queda abajo sin ejecutarse por ahora)
});

// 5. Ejecutar la app envuelta en un bloque Try-Catch con Serilog
try
{
    Log.Information("Iniciando la API del Trabajo Práctico...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló de forma imprevista durante la ejecución.");
}
finally
{
    Log.CloseAndFlush();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}