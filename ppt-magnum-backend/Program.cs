using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore; // Necesario para EF Core
using ppt.Data; // Asegúrate de que el namespace coincida
using ppt.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(8, 0, 23)))); // Ajusta la versión según tu servidor MySQL

// Agregar servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddScoped<IPartidaService, PartidaService>(); // Registro del servicio

var app = builder.Build();

// Configurar el middleware HTTP.
app.UseRouting();

// Agregar autorización, si es necesario.
app.UseAuthorization();

// Mapear los controladores a las rutas.
app.MapControllers();

app.Run();
