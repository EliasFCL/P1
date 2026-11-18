using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Contexto
{
    public class EmpleadoDbContext : DbContext
    {
        public EmpleadoDbContext() : base()
        {
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Conectar conectar = new Conectar();

                optionsBuilder.UseSqlServer(
                    conectar.CadenaConexion()
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empleado>()
                .Property(e => e.puestoID)
                .HasColumnName("PuestoId");

            modelBuilder.Entity<Empleado>()
                .Property(e => e.puestoID)
                .HasColumnName("PuestoId");
        }
        public DbSet<Empleado> Empleados { get; set; }
    }
}