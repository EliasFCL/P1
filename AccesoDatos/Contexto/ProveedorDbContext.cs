using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Contexto
{
    public class ProveedorDbContext : DbContext
    {
        public ProveedorDbContext() : base()
        {
            new DbContextOptionsBuilder<ProveedorDbContext>().UseSqlServer(new Conectar().CadenaConexion());
        }

        //Configurar conexión
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
        public DbSet<Proveedor> Proveedores { get; set; }//Definir tabla
    }
}
