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
            string cadena = "Data Source=DESKTOP-5EQR0M8\\SQLEXPRESS07;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Application Name=\"SQL Server Management Studio\";Command Timeout=0";
            return cadena;
        }
    }
}
