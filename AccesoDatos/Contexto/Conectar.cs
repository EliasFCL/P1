//Clase que devuelve la dirección de la base de datos
namespace AccesoDatos.Contexto
{
    public class Conectar
    {
        
        public string CadenaConexion ()
        {
            string cadena = @"Data Source=.\SQLEXPRESS07;Initial Catalog=Carniceria;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

            return cadena;
        }
    }
}
