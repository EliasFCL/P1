using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Contexto
{
    public class ProductoDbContext : DbContext
    {
        public ProductoDbContext() : base()
        {
            new DbContextOptionsBuilder<ProductoDbContext>().UseSqlServer(new Conectar().CadenaConexion());
        }
        //Configurar conexión
        protected override void OnConfiguring(
         DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Conectar conectar = new Conectar();

                optionsBuilder.UseSqlServer(conectar.CadenaConexion());
            }
        }
        //Definir tablas
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
    }
}