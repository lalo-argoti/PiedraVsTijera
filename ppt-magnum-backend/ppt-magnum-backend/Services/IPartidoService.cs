using System.Threading.Tasks;
using ppt.Models;

namespace ppt.Services
{
    public interface IPartidaService
    {
        Task<bool> VerificarCodigoExistenteAsync(string codigo);
        Task<Juego?> ObtenerJuegoPorCodigoAsync(string codigo);
        Task<bool> AgregarJugadorAsync(string codigo, string jugador);
        Task<bool> CompletarPreparacionJuegoAsync(string codigo);
        Task<Juego?> ObtenerOCrearJuegoAsync(string codigo); 
        Task<bool> RealizarMovimientoAsync(string partida, string usuario, int movimiento);
        Task<EstadoJuego?> ObtenerEstadoJuegoAsync(string codigoJuego);
        Task<string> Tirada(string partida, int JugadaActual);
        Task<string> Puntos(string partida);
        Task<string> Movimiento(string partida, int JugadaActual);


    }
}
 
