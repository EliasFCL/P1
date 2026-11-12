using AccesoDatos.Configuraciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccesoDatos.Contexto
{
    public class Conectar
    {
        
        public string CadenaConexion ()
        {
            string cadena = "Data Source=DESKTOP-5EQR0M8\\SQLEXPRESS07;Initial Catalog=Carniceria;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            return cadena;
        }
    }
}
