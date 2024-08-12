	using Microsoft.EntityFrameworkCore;
using ppt.Models;

namespace ppt.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Juego> Juegos { get; set; }

        // Configuración de los modelos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para la entidad Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Id); // Definir la clave primaria

            // Configuración para la entidad Partida
            modelBuilder.Entity<Partida>()
                .HasKey(p => p.Id); // Definir la clave primaria

            // Configuración para la entidad Juego
            modelBuilder.Entity<Juego>()
                .HasKey(j => j.Id); // Definir la clave primaria

	    modelBuilder.Entity<Partida>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

	    modelBuilder.Entity<Partida>()
		.Property(p => p.CodigoJuego)
		.IsRequired()
		.HasMaxLength(50); 

            // Configuración de propiedades adicionales para Juego
            modelBuilder.Entity<Juego>()
                .Property(j => j.CodigoInscrip)
                .IsRequired()
                .HasMaxLength(50); // Definir restricciones para el campo CodigoInscrip

            // Configuración de propiedades adicionales para Partida
            
        modelBuilder.Entity<Partida>()
            .Property(p => p.UsuarioLocalId)
            .IsRequired(false);

        modelBuilder.Entity<Partida>()
            .Property(p => p.UsuarioRemotoId)
            .IsRequired(false);
        }
    }
}
