// OnModelCreating: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-11.0&tabs=aspnetcore y
// https://www.learnentityframeworkcore.com/configuration/fluent-api/ignore-method
using Models;
using Microsoft.EntityFrameworkCore;
using AccesoDatos.Contexto;

namespace AccesoDatos.Contexto
{
    namespace AccesoDatos.Contexto
    {
        public class FacturaDbContexto : DbContext
        {
            public FacturaDbContexto()
            {
            }
            Conectar cadena = new Conectar();
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer(
                        cadena.CadenaConexion()
                    );
                }
            }

            public DbSet<Factura> Facturas { get; set; }
            public DbSet<DetalleFactura> DetallesFactura { get; set; }
            public DbSet<Cliente> Clientes { get; set; }
            public DbSet<Empleado> Empleados { get; set; }
            public DbSet<Producto> Productos { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Factura>()
                    .HasKey(f => f.numeroFactura);

                modelBuilder.Entity<DetalleFactura>()
                    .HasKey(d => d.Id);

                modelBuilder.Entity<Factura>()
                    .Ignore(f => f.detalles);

                modelBuilder.Entity<DetalleFactura>()
                    .Ignore(d => d.producto);

                base.OnModelCreating(modelBuilder);
            }
        }
    }
}