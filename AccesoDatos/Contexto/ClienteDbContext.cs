//Contextos: https://learn.microsoft.com/es-es/ef/core/dbcontext-configuration y video guía para el configurar el acceso a datos parte #3 7:00

using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Contexto
{
    public class ClienteDbContext : DbContext
    {
        public ClienteDbContext() : base()
        {
            new DbContextOptionsBuilder<ClienteDbContext>().UseSqlServer(new Conectar().CadenaConexion());
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

        public DbSet<Cliente> Clientes { get; set; }
    }
}