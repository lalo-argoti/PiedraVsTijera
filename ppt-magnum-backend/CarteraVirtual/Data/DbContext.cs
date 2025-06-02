using Microsoft.EntityFrameworkCore;
using pdt.Models;

namespace pdt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<GastoTipo> GastosTipos { get; set; }
        public DbSet<FondoMonetario> FondosMonetarios { get; set; }
        public DbSet<PresupuestoMovimiento> PresupuestosMovimientos { get; set; }
        public DbSet<GastoRegistro> GastosRegistros { get; set; }
        public DbSet<GastoDetalle> GastosDetalles { get; set; }
        public DbSet<Deposito> Depositos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("pdt_users");
            modelBuilder.Entity<UserGroup>().ToTable("pdt_users_groups");
            modelBuilder.Entity<GastoTipo>().ToTable("pdt_crtr_gastos_tipo");
            modelBuilder.Entity<FondoMonetario>().ToTable("pdt_fondos_monetarios");
            modelBuilder.Entity<PresupuestoMovimiento>().ToTable("pdt_presupuesto");
            modelBuilder.Entity<GastoRegistro>().ToTable("pdt_gastos");
            modelBuilder.Entity<GastoDetalle>().ToTable("pdt_gastos_detalle");
            modelBuilder.Entity<Deposito>().ToTable("pdt_depositos");


            // Relaciones si aplican
            modelBuilder.Entity<User>()
                .HasOne(u => u.GrupoNavigation)
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.Grupo);

            modelBuilder.Entity<FondoMonetario>()
                .HasOne(f => f.PropietarioNavigation)
                .WithMany(u => u.FondosMonetarios)
                .HasForeignKey(f => f.Propietario);

            modelBuilder.Entity<GastoTipo>()
                .HasOne(g => g.PropietarioNavigation)
                .WithMany(u => u.GastosTipos)
                .HasForeignKey(g => g.Propietario);
                
            
          
            modelBuilder.Entity<GastoTipo>()
                .Property(g => g.Presupuestocol)
                .HasPrecision(18, 2); // Puedes ajustar según tus necesidades

            modelBuilder.Entity<GastoTipo>()
                .Property(g => g.Presupuestousd)
                .HasPrecision(18, 2);

            // Lo mismo para otras entidades:
            modelBuilder.Entity<GastoDetalle>()
                .Property(g => g.Monto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PresupuestoMovimiento>()
                .Property(p => p.MontoPresupuestado)
                .HasPrecision(18, 2);

                

        }
    }
}
