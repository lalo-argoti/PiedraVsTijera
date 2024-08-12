namespace ppt.Models
{
    public class Partida
    {
   
        public Usuario UsuarioLocal { get; set; }
        public Usuario UsuarioRemoto { get; set; }

        public string UsuarioLocalId { get; set; }
        public string UsuarioRemotoId { get; set; }
        public int Id { get; set; }
        public string CodigoJuego { get; set; }
        public int ?  TiradaServidor { get; set; }=0;        
        public int ? TiradaInvitado { get; set; } =0;       

    }
}
