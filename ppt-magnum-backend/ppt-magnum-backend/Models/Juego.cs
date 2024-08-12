namespace ppt.Models
{
    public class Juego
    {
        public int Id { get; set; } // Id es un identificador único para el juego

        // Las propiedades que pueden ser nulas en la base de datos deben ser definidas como anulables
        public string? Jugador1 { get; set; } // Nombre del primer jugador
        public string? Jugador2 { get; set; } // Nombre del segundo jugador
        public int? PuntajeJ1 { get; set; } // Puntaje del primer jugador
        public int? PuntajeJ2 { get; set; } // Puntaje del segundo jugador
        public string CodigoInscrip { get; set; } // Código de inscripción para el juego
    }
}
