using Microsoft.AspNetCore.Mvc;
using ppt.Services; 
namespace ppt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PartidoController : ControllerBase
    {
        private readonly IPartidaService _partidaService;
	private readonly ILogger<PartidoController> _logger;

        public PartidoController(IPartidaService partidaService, ILogger<PartidoController> logger)
        {
            _partidaService = partidaService;
	    _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string user, [FromQuery] string partida, [FromQuery] int movimiento)
        {
            // Lógica para determinar el movimiento basado en el parámetro
            string movimientoString = movimiento switch
            {
                1 => "piedra",
                2 => "papel",
                3 => "tijera",
                _ => "movimiento desconocido"
            };

        if (movimiento == 0)
        {
            // Obtener o crear el juego
            var juego = await _partidaService.ObtenerOCrearJuegoAsync(partida);

            if (juego == null)
                return NotFound("No se pudo crear o encontrar el juego.");

            // Agregar el jugador al juego
            bool jugadorAgregado = await _partidaService.AgregarJugadorAsync(partida, user);
            if (!jugadorAgregado)
                return BadRequest("No se pudo agregar el jugador al juego.");

            // Completar la preparación del juego si ambos jugadores están presentes
            bool preparacionCompleta = await _partidaService.CompletarPreparacionJuegoAsync(partida);
            if (preparacionCompleta)
                return Ok($"Juego preparado correctamente. {user} jugó {movimientoString}.");
            else
                return Ok($"Esperando el segundo jugador. {user} jugó {movimientoString}.");
         }
        else
           {
	     _logger.LogInformation("Se va a guardar un movimiento {user}:{movimiento} en {partida}",user, partida, movimiento);
            var movimientoRealizado = await _partidaService.RealizarMovimientoAsync(partida, user, movimiento);

            if (movimientoRealizado)
            {
                return Ok($"{user} realizó el movimiento {movimientoString} correctamente.");
            }
            else
            {
                return BadRequest("No se pudo realizar el movimiento.");
            }
          }
        }
           [HttpGet("estado")]
        public async Task<IActionResult> ObtenerEstadoJuego([FromQuery] string codigo)
        {
            var estadoJuego = await _partidaService.ObtenerEstadoJuegoAsync(codigo);

            if (estadoJuego == null)
                return NotFound("Juego no encontrado.");

            return Ok(estadoJuego);
        }
    }
}
