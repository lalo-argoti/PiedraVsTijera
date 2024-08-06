using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ppt.Data;
using ppt.Models;

namespace ppt.Services
{
    public class PartidaService : IPartidaService
    {
        private readonly AppDbContext _context;
	 private readonly ILogger<PartidaService> _logger;

        public PartidaService(AppDbContext context, ILogger<PartidaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Juego?> ObtenerOCrearJuegoAsync(string codigo)
        {
            _logger.LogInformation("Iniciando el proceso de crear juego {codigo}", codigo);

            string codigoParaBuscar = (codigo.Length == 6) ? codigo.Substring(1) : codigo;

            var juego = await _context.Juegos.FirstOrDefaultAsync(j => j.CodigoInscrip == codigoParaBuscar);

            if (juego == null)
            {
                juego = new Juego { CodigoInscrip = codigoParaBuscar };
                _context.Juegos.Add(juego);
                await _context.SaveChangesAsync();
            }

            return juego;
        }

        public async Task<bool> VerificarCodigoExistenteAsync(string codigo)
        {
            string codigoParaBuscar = (codigo.Length == 6) ? codigo.Substring(1) : codigo;
            return await _context.Juegos.AnyAsync(j => j.CodigoInscrip == codigoParaBuscar);
        }

        public async Task<Juego?> ObtenerJuegoPorCodigoAsync(string codigo)
        {
            string codigoParaBuscar = (codigo.Length == 6) ? codigo.Substring(1) : codigo;
            return await _context.Juegos.FirstOrDefaultAsync(j => j.CodigoInscrip == codigoParaBuscar);
        }

        public async Task<bool> AgregarJugadorAsync(string codigo, string jugador)
        {
            var juego = await ObtenerOCrearJuegoAsync(codigo);

            if (juego == null)
                return false;

            if (codigo.Length == 6)
            {
                if (string.IsNullOrEmpty(juego.Jugador1))
                {
                    juego.Jugador1 = jugador;
                }
                else
                {  if (juego.Jugador1!=jugador)
                   {     return false; }// Ya hay un jugador en la posición 1
                }
            }
            else if (codigo.Length == 5)
            {
                if (string.IsNullOrEmpty(juego.Jugador2))
                {
                    juego.Jugador2 = jugador;
                }
                else
                {
                    if (juego.Jugador2!=jugador)
                   { return false;} // Ya hay un jugador en la posición 2
                }
            }
            else
            {
                return false; // Código no válido
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompletarPreparacionJuegoAsync(string codigo)
        {
            var juego = await ObtenerJuegoPorCodigoAsync(codigo);

            if (juego == null || string.IsNullOrEmpty(juego.Jugador1) || string.IsNullOrEmpty(juego.Jugador2))
                return false;

            // Implementa aquí la lógica para completar la preparación del juego si es necesario

            return true;
        }
//-------------------------------------------------------------------------------


public async Task<bool> RealizarMovimientoAsync(string partidaCodigo, string usuarioId, int tirada)
{
    _logger.LogInformation("Intentando obtener la partida con código {partidaCodigo}", partidaCodigo);

    // Extrae los últimos 5 dígitos del código de partida para la comparación
    string codigoComparacion = partidaCodigo.Length > 5 ? partidaCodigo.Substring(partidaCodigo.Length - 5) : partidaCodigo;

    // Buscar el juego correspondiente en la base de datos
    var juego = await _context.Juegos
        .FirstOrDefaultAsync(j => j.CodigoInscrip.EndsWith(codigoComparacion));

    if (juego == null)
    {
        _logger.LogWarning("No se encontró el código de inscripción {codigoComparacion} en Juegos.", codigoComparacion);
        return false;
    }

    // Buscar la partida correspondiente en la base de datos
    var partida = await _context.Partidas
        .FirstOrDefaultAsync(p => p.CodigoJuego.EndsWith(codigoComparacion));

    if (partida == null)
    {
        _logger.LogInformation("No se encontró la partida con código {partidaCodigo}, creando una nueva.");
         string CodigoNormalizado = partidaCodigo.Length == 5 ? partidaCodigo : partidaCodigo.Substring(1);
        partida = new Partida
        {
            CodigoJuego = CodigoNormalizado,
            UsuarioLocalId = (partidaCodigo.Length == 6) ? usuarioId : null,
            UsuarioRemotoId = (partidaCodigo.Length == 5) ? usuarioId : null,
            TiradaInvitado = null,
            TiradaServidor = null
        };

        _context.Partidas.Add(partida);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Nueva partida creada con código {partidaCodigo}", partidaCodigo);
    }

    // Verificar si el código de la partida coincide
    if (partida.CodigoJuego.EndsWith(codigoComparacion))
    {
        bool esServidor = partidaCodigo.Length == 6;

        if (esServidor)
        {
            // Se actualiza la tirada del servidor sin verificar la asociación del usuario
            partida.TiradaServidor = tirada;
        }
        else
        {
            // Se actualiza la tirada del invitado sin verificar la asociación del usuario
            partida.TiradaInvitado = tirada;
        }

        try
        {
            _logger.LogInformation("Actualizando la partida con código {partidaCodigo}", partidaCodigo);
            _context.Partidas.Update(partida);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Movimiento guardado correctamente en la partida con código {partidaCodigo}", partidaCodigo);
             await ComprobarGanadorYActualizarJuegoAsync(partidaCodigo);
             return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el movimiento en la partida con código {partidaCodigo}. Detalles: CodigoJuego='{CodigoJuego}', UsuarioId='{UsuarioId}', Tirada='{Tirada}', MensajeError='{ErrorMensaje}'",
                              partidaCodigo,
                              partida.CodigoJuego,
                              usuarioId,
                              tirada,
                              ex.Message);
            return false;
        }

        return true;
    }

    return false;
}


   //-----------------------------------------------------------------------------

   public async Task<EstadoJuego?> ObtenerEstadoJuegoAsync(string codigoJuego)
        {
	var juego = await _context.Juegos
        .Where(j => j.CodigoInscrip == codigoJuego)
        .FirstOrDefaultAsync();

    if (juego == null)
        return null;

    return new EstadoJuego
    {
        CodigoJuego = juego.CodigoInscrip,
        Usuarios = new Dictionary<string, Jugador>
        {
            { "Servidor1", new Jugador { Username = juego.Jugador1, Puntos = juego.PuntajeJ1 ?? 0 } },
            { "Servidor2", new Jugador { Username = juego.Jugador2, Puntos = juego.PuntajeJ2 ?? 0 } }
        }
    };
        }


     // (= (= (= (= (= (= (= (= (= (= (= (= (= (= (= (= (= (=

	private async Task ComprobarGanadorYActualizarJuegoAsync(string codigoComparacionOriginal)
	{
    // Recorta el código a 5 dígitos si es necesario
    string codigoComparacion = codigoComparacionOriginal.Length == 6
        ? codigoComparacionOriginal.Substring(1, 5) // Obtiene del índice 1 al 5
        : codigoComparacionOriginal;

    // Obtener la partida para el código comparado
    var partida = await _context.Partidas
        .FirstOrDefaultAsync(p => p.CodigoJuego.EndsWith(codigoComparacion));

    if (partida == null)
    {
        Console.WriteLine($"No se encontró la partida con código {codigoComparacion}");
        return;
    }

    // Verifica si ambas tiradas están presentes
    if (partida.TiradaInvitado.HasValue && partida.TiradaServidor.HasValue)
    {
        int tiradaInvitado = partida.TiradaInvitado.Value;
        int tiradaServidor = partida.TiradaServidor.Value;

    

        // Determina el ganador
        string ganador = DeterminarGanador(tiradaInvitado, tiradaServidor, partida.UsuarioRemotoId, partida.UsuarioLocalId);

        // Actualiza el puntaje en Juegos
        var juego = await _context.Juegos
            .FirstOrDefaultAsync(j => j.CodigoInscrip.EndsWith(codigoComparacion));

        if (juego == null)
        {
            Console.WriteLine($"No se encontró el juego con código {codigoComparacion}");
            return;
        }

        if (ganador == juego.Jugador1)
        {
            juego.PuntajeJ1 = (juego.PuntajeJ1 ?? 0) + 1;
        }
        else if (ganador == juego.Jugador2)
        {
            juego.PuntajeJ2 = (juego.PuntajeJ2 ?? 0) + 1;
        }

        _context.Juegos.Update(juego);
        await _context.SaveChangesAsync();

        // Elimina la partida
        _context.Partidas.Remove(partida);
        await _context.SaveChangesAsync();

    

        // Lógica para crear y retornar un objeto de tipo `Movimientos`
	  await Task.CompletedTask; // Finaliza el método sin devolver un valor

     }
    else
    {
        Console.WriteLine("No se encontraron ambas tiradas para determinar el ganador.");
    }
	}



	private string DeterminarGanador(int tiradaInvitado, int tiradaServidor, string invitado, string servidor)
	{
    // Implementar la lógica para determinar el ganador según piedra, papel o tijera.
    if (tiradaInvitado == tiradaServidor) return null; // Empate
    if ((tiradaInvitado == 1 && tiradaServidor == 3) || 
        (tiradaInvitado == 2 && tiradaServidor == 1) || 
        (tiradaInvitado == 3 && tiradaServidor == 2))
        return invitado;
    return servidor;
	}


      //
}


}
