using Models;
using AccesoDatos.Contexto;
using Microsoft.Data.SqlClient;
namespace AccesoDatos.Repositorios
{
    public class ProveedorRepositorio
    {
        private readonly string conexionString;
        Conectar conectar = new Conectar();
        public ProveedorRepositorio(string connectionString)
        {
            conexionString = connectionString;
        }


        public bool Registrar(Proveedor proveedor)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = @"
            INSERT INTO Proveedores (Id, tipoIdentificacion, numId, nombre, RazonSocial, Telefono, CorreoElectronico, TipoSuministro, FrecuenciaEntrega)
            VALUES (@Id, @tipoIdentificacion, @numId, @nombre, @RazonSocial, @Telefono, @CorreoElectronico, @TipoSuministro, @FrecuenciaEntrega)";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", proveedor.Id);
                    comando.Parameters.AddWithValue("@tipoIdentificacion", proveedor.tipoIdentificacion);
                    comando.Parameters.AddWithValue("@numId", proveedor.numId);
                    comando.Parameters.AddWithValue("@nombre", proveedor.nombre);
                    comando.Parameters.AddWithValue("@RazonSocial", proveedor.RazonSocial);
                    comando.Parameters.AddWithValue("@Telefono", proveedor.Telefono);
                    comando.Parameters.AddWithValue("@CorreoElectronico", proveedor.CorreoElectronico);
                    comando.Parameters.AddWithValue("@TipoSuministro", proveedor.TipoSuministro);
                    comando.Parameters.AddWithValue("@FrecuenciaEntrega", proveedor.FrecuenciaEntrega);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        public List<Proveedor> Listar()
        {
            var proveedores = new List<Proveedor>();
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT * FROM Proveedores";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var proveedor = new Proveedor
                            {
                                Id = lector.GetGuid(0),
                                tipoIdentificacion = lector.GetString(1),
                                numId = lector.GetString(2),
                                nombre = lector.GetString(3),
                                RazonSocial = lector.GetString(4),
                                Telefono = lector.GetInt32(5),
                                CorreoElectronico = lector.GetString(6),
                                TipoSuministro = lector.GetString(7),
                                FrecuenciaEntrega = lector.GetString(8)
                            };
                            proveedores.Add(proveedor);
                        }
                    }
                }
            }
            return proveedores;
        }

        public Proveedor BuscarPorId(Guid id)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT * FROM Proveedores WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Proveedor
                            {
                                Id = lector.GetGuid(0),
                                tipoIdentificacion = lector.GetString(1),
                                numId = lector.GetString(2),
                                nombre = lector.GetString(3),
                                RazonSocial = lector.GetString(4),
                                Telefono = lector.GetInt32(5),
                                CorreoElectronico = lector.GetString(6),
                                TipoSuministro = lector.GetString(7),
                                FrecuenciaEntrega = lector.GetString(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Proveedor BuscarPorNumId(string numId)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT * FROM Proveedores WHERE numId = @numId";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@numId", numId);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Proveedor
                            {
                                Id = lector.GetGuid(0),
                                tipoIdentificacion = lector.GetString(1),
                                numId = lector.GetString(2),
                                nombre = lector.GetString(3),
                                RazonSocial = lector.GetString(4),
                                Telefono = lector.GetInt32(5),
                                CorreoElectronico = lector.GetString(6),
                                TipoSuministro = lector.GetString(7),
                                FrecuenciaEntrega = lector.GetString(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool Eliminar(Guid id)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "DELETE FROM Proveedores WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        public bool Actualizar(Guid id, Proveedor proveedor)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = @"
            UPDATE Proveedores
            SET tipoIdentificacion = @tipoIdentificacion,
                numId = @numId,
                nombre = @nombre,
                RazonSocial = @RazonSocial,
                Telefono = @Telefono,
                CorreoElectronico = @CorreoElectronico,
                TipoSuministro = @TipoSuministro,
                FrecuenciaEntrega = @FrecuenciaEntrega
            WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    comando.Parameters.AddWithValue("@tipoIdentificacion", proveedor.tipoIdentificacion);
                    comando.Parameters.AddWithValue("@numId", proveedor.numId);
                    comando.Parameters.AddWithValue("@nombre", proveedor.nombre);
                    comando.Parameters.AddWithValue("@RazonSocial", proveedor.RazonSocial);
                    comando.Parameters.AddWithValue("@Telefono", proveedor.Telefono);
                    comando.Parameters.AddWithValue("@CorreoElectronico", proveedor.CorreoElectronico);
                    comando.Parameters.AddWithValue("@TipoSuministro", proveedor.TipoSuministro);
                    comando.Parameters.AddWithValue("@FrecuenciaEntrega", proveedor.FrecuenciaEntrega);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }
    }
}
