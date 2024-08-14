using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ppt.Services;
using Newtonsoft.Json;

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

        private IActionResult CreateJsonResponse(string message,string tabla, string puntos, int mezcla)
        {
            // Crea un diccionario para la respuesta
            var resultado = new { ServeSay = message, Movimientos=tabla, Puntos= puntos, Mezcla = mezcla };

            // Convierte el diccionario a JSON
            var json = JsonConvert.SerializeObject(resultado);

            // Devuelve el JSON con el tipo de contenido adecuado y el código de estado HTTP
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200 // Código de estado HTTP 200 OK
            };
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
                    return CreateJsonResponse("No se pudo crear o encontrar el juego.","","", 367);

                // Agregar el jugador al juego
                bool jugadorAgregado = await _partidaService.AgregarJugadorAsync(partida, user);
                if (!jugadorAgregado)
                    return CreateJsonResponse("No se pudo agregar el jugador al juego.","","", 376);

                // Completar la preparación del juego si ambos jugadores están presentes
                bool preparacionCompleta = await _partidaService.CompletarPreparacionJuegoAsync(partida);
                if (preparacionCompleta  ) {
                    var mensaje = await _partidaService.Tirada(partida, 0); 
                    var puntos = await _partidaService.Puntos(partida);
                    var movimientos = await _partidaService.Movimiento(partida,0);
                    return CreateJsonResponse(mensaje,movimientos,puntos, 379);
					 }
                else
                    return CreateJsonResponse("->Invita a alguien con: "+partida.Substring(1),"","", 397);  // Debemos reintentar 3 segundos despues
            }
            else
            {
                _logger.LogInformation("Se va a guardar un movimiento {user}:{movimiento} en {partida}", user, movimiento, partida);
                var mensaje = await _partidaService.Tirada(partida, movimiento);
                
                var movimientoRealizado = await _partidaService.RealizarMovimientoAsync(partida, user, movimiento);
                var puntos = await _partidaService.Puntos(partida);
                var movimientos = await _partidaService.Movimiento(partida,movimiento);

		if (movimientoRealizado)
                    return CreateJsonResponse(mensaje,movimientos,puntos, 736);
                else
                    return CreateJsonResponse("->No se pudo realizar el movimiento.","","", 739);
            }
        }

        [HttpGet("estado") ]
        public async Task<IActionResult> ObtenerEstadoJuego([FromQuery] string codigo)
        {
            var estadoJuego = await _partidaService.ObtenerEstadoJuegoAsync(codigo);

            if (estadoJuego == null)
                return CreateJsonResponse("Juego no encontrado."+estadoJuego,"","", 793);

            return CreateJsonResponse("Estado del juego obtenido correctamente.","","", 973);
        }
    }
}
