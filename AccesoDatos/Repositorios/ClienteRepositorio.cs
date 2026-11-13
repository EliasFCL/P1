using AccesoDatos.Contexto;
using Microsoft.Data.SqlClient;
using Models;
using AccesoDatos.Contexto;

namespace AccesoDatos.Repositorios
{
    public class ClienteRepositorio
    {
        private readonly string conexionString;
        Conectar conectar = new Conectar();
        public ClienteRepositorio(string connectionString)
        {
            conexionString = conectar.CadenaConexion();
        }

        public bool Registrar(Cliente cliente)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = @"INSERT INTO Clientes (Id, tipoIdentificacion, numId, nombre, apellido1, apellido2, fechaNacimiento) 
                                    VALUES (@Id, @tipoIdentificacion, @numId, @nombre, @apellido1, @apellido2, @fechaNacimiento)";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    // Si el ID viene vacío, generamos uno nuevo
                    if (cliente.Id == Guid.Empty) cliente.Id = Guid.NewGuid();

                    comando.Parameters.AddWithValue("@Id", cliente.Id);
                    comando.Parameters.AddWithValue("@tipoIdentificacion", cliente.tipoIdentificacion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@numId", cliente.numId);
                    comando.Parameters.AddWithValue("@nombre", cliente.nombre);
                    comando.Parameters.AddWithValue("@apellido1", cliente.apellido1);
                    comando.Parameters.AddWithValue("@apellido2", cliente.apellido2);
                    comando.Parameters.AddWithValue("@fechaNacimiento", cliente.fechaNacimiento ?? (object)DBNull.Value);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        public void Actualizar(Guid id, Cliente cliente)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = @"UPDATE Clientes 
                                    SET tipoIdentificacion = @tipoIdentificacion, 
                                        numId = @numId, 
                                        nombre = @nombre, 
                                        apellido1 = @apellido1, 
                                        apellido2 = @apellido2, 
                                        fechaNacimiento = @fechaNacimiento 
                                    WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    comando.Parameters.AddWithValue("@tipoIdentificacion", cliente.tipoIdentificacion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@numId", cliente.numId);
                    comando.Parameters.AddWithValue("@nombre", cliente.nombre);
                    comando.Parameters.AddWithValue("@apellido1", cliente.apellido1);
                    comando.Parameters.AddWithValue("@apellido2", cliente.apellido2);
                    comando.Parameters.AddWithValue("@fechaNacimiento", cliente.fechaNacimiento ?? (object)DBNull.Value);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public bool eliminar(Guid id)
        {
            try
            {
                using (var conexion = new SqlConnection(conexionString))
                {
                    string consulta = "DELETE FROM Clientes WHERE Id = @Id";
                    using (var comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id", id);
                        conexion.Open();
                        int filasAfectadas = comando.ExecuteNonQuery();
                        return filasAfectadas > 0; //Devolver true si se borra
                    }
                }
            }
            catch (Exception)
            {
                // Si SQL Server bloquea el borrado se devuelve false
                return false;
            }
        }
        public List<Cliente> Listar()
        {
            var lista = new List<Cliente>();

            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT Id, tipoIdentificacion, numId, nombre, apellido1, apellido2, fechaNacimiento FROM Clientes";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Cliente
                            {
                                Id = (Guid)lector["Id"],
                                tipoIdentificacion = lector["tipoIdentificacion"].ToString(),
                                numId = lector["numId"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                apellido1 = lector["apellido1"].ToString(),
                                apellido2 = lector["apellido2"].ToString(),
                                fechaNacimiento = lector["fechaNacimiento"] == DBNull.Value ? null : (DateTime)lector["fechaNacimiento"]
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Cliente BuscarPorId(Guid id)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT Id, tipoIdentificacion, numId, nombre, apellido1, apellido2, fechaNacimiento FROM Clientes WHERE Id = @Id";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Cliente
                            {
                                Id = (Guid)lector["Id"],
                                tipoIdentificacion = lector["tipoIdentificacion"].ToString(),
                                numId = lector["numId"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                apellido1 = lector["apellido1"].ToString(),
                                apellido2 = lector["apellido2"].ToString(),
                                fechaNacimiento = lector["fechaNacimiento"] == DBNull.Value ? null : (DateTime)lector["fechaNacimiento"]
                            };
                        }
                    }
                }
            }
            return null; // Si no lo encuentra en la base de datos
        }

        public Cliente buscarPorNumId(string numId)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT Id, tipoIdentificacion, numId, nombre, apellido1, apellido2, fechaNacimiento FROM Clientes WHERE numId = @numId";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@numId", numId);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Cliente
                            {
                                Id = (Guid)lector["Id"],
                                tipoIdentificacion = lector["tipoIdentificacion"].ToString(),
                                numId = lector["numId"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                apellido1 = lector["apellido1"].ToString(),
                                apellido2 = lector["apellido2"].ToString(),
                                fechaNacimiento = lector["fechaNacimiento"] == DBNull.Value ? null : (DateTime)lector["fechaNacimiento"]

                            };
                        };
                    }
                }
            }
            return null; // Si no lo encuentra en la base de datos
        }
    }
}