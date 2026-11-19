using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Contexto
{
    public class EmpleadoDbContext : DbContext
    {
        public EmpleadoDbContext() : base()
        {
            new DbContextOptionsBuilder<EmpleadoDbContext>().UseSqlServer(new Conectar().CadenaConexion());
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

        public DbSet<Empleado> Empleados { get; set; }
    }
}