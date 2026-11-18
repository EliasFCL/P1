using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

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
