using Microsoft.EntityFrameworkCore;
using ProyectoFinalG5.Models;

namespace ProyectoFinalG5.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<TipoServicio> TipoServicios { get; set; }
        public DbSet<Servicio> Servicios { get; set; }

        public DbSet<HorarioFijo> HorariosFijos { get; set; }

        public DbSet<SalaEstudio> SalaEstudios { get; set; }

        public DbSet<Pago> Pagos { get; set; }

        public DbSet<Reserva> Reservas { get; set; }

        public DbSet<ReservaServicio> ReservaServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Servicio>()
                .HasOne(s => s.TipoServicio)
                .WithMany()
                .HasForeignKey(s => s.IdTipoServicio)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Empleado)
                .WithMany()
                .HasForeignKey(r => r.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.SalaEstudio)
                .WithMany()
                .HasForeignKey(r => r.IdSala)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.HorarioFijo)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.IdHorarioFijo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Pago)
                .WithMany()
                .HasForeignKey(r => r.IdPago)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ReservaServicio>()
                .HasOne(rs => rs.Reserva)
                .WithMany(r => r.ReservaServicios)
                .HasForeignKey(rs => rs.IdReserva)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservaServicio>()
                .HasOne(rs => rs.Servicio)
                .WithMany()
                .HasForeignKey(rs => rs.IdServicio)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}