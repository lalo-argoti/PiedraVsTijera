using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using pdt.Models;
using pdt.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace pdt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config; // <-- declaras _config aquí

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config; // <-- lo inyectas y asignas aquí
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        
        
        [HttpGet("verificar-autenticacion")]
        public IActionResult VerificarAutenticacion()
        {
            // Verificar el token JWT del header Authorization
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
                return Ok(new { autenticado = false });

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Ok(new { autenticado = true });
            }
            catch
            {
                return Ok(new { autenticado = false });
            }
        }

        [HttpPost("autenticar")]
        public IActionResult Autenticar([FromBody] User login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Nombre de usuario y contraseña son requeridos.");

            var user = _context.Users
                .FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);

            if (user == null)
                return Unauthorized("Credenciales inválidas.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var claveJwt = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(claveJwt))
                return StatusCode(500, "No se configuró la clave JWT en appsettings.");

            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Username", user.Username.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Ok(new { token = jwtToken, username = user.Username, id = user.Id });
        }
    }
}

