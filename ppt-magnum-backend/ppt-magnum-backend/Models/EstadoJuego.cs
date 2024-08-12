namespace ppt.Models
{


public class EstadoJuego
{
    public string CodigoJuego { get; set; }
    public Dictionary<string, Jugador> Usuarios { get; set; } = new Dictionary<string, Jugador>();
}
}
