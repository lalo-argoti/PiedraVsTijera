using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using pdt.Data; // Asegúrate de tener este namespace


var builder = WebApplication.CreateBuilder(args);

// 🔧 Cargar conexión desde appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🌐 CORS
var allowedOrigin = "http://172.20.10.3:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Frontend",
        policy =>
        {
            policy.WithOrigins(allowedOrigin)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var jwtKey = builder.Configuration["Jwt:Key"];

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 🧩 Swagger + Endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// ✅ Habilitar CORS *antes* de rutas
app.UseCors("Frontend");
app.UseAuthentication();        // ⬅️ después de CORS
app.UseAuthorization();

// ✅ Redirección HTTP -> HTTPS (opcional si solo usas HTTP)
app.UseHttpsRedirection();

// ✅ Swagger solo en entorno Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🧪 Ping básico
app.MapGet("/ping", () =>
{
    var response = new
    {
        status = "OK",
        timestamp = DateTime.UtcNow.ToString("o"),
        message = "Backend activo"
    };
    return Results.Json(response);
});

// 🌦️ Weather dummy route
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();

// ✅ Escucha todas las IPs en el puerto 8000
app.Run("http://0.0.0.0:8000");

// 🧾 Modelo dummy para pruebas
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
