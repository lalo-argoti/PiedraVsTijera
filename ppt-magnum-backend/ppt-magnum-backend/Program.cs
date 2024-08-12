using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore; // Necesario para EF Core
using ppt.Data; // Asegúrate de que el namespace coincida
using ppt.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

//using MySql.EntityFrameworkCore.Extensions; // Namespace para MySqlServerVersion
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS: Permite solicitudes desde el origen especificado
var _myCorsPolicy = "Front"; // Nombre de la política CORS

        builder.Services.AddCors(
            options => {
                options.AddPolicy("Front",
                    policy => {
                        policy.WithOrigins("http://51.222.141.101:4201")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                    });
            });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Sesion";
        // Configuración adicional si es necesaria
    });

// Configuración de la cadena de conexión a la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23)))); // Ajusta la versión según tu servidor MySQL

// Agregar servicios al contenedor
builder.Services.AddControllers(); // Registra los servicios de controladores
builder.Services.AddScoped<IPartidaService, PartidaService>(); // Registro del servicio IPartidaService con su implementación PartidaService

var app = builder.Build();

// Configuración del middleware HTTP
if (app.Environment.IsDevelopment())
{
    // Muestra una página de error detallada en desarrollo
    app.UseDeveloperExceptionPage();
}

// Configuración de CORS: Aplicar la política CORS definida
app.UseCors("Front");

// Configuración del middleware de enrutamiento: Habilita el enrutamiento de solicitudes
app.UseRouting();

// Agregar autorización: Permite la autorización de solicitudes, si es necesario
//app.UseAuthentication();  
app.UseAuthorization();

// Mapear los controladores a las rutas: Define las rutas para los controladores
app.MapControllers();

// Inicia la aplicación y comienza a escuchar las solicitudes
app.Run();
