using Microsoft.Data.SqlClient;
using Models;

namespace AccesoDatos.Repositorios
{
    public class ProductoRepositorio
    {
        private readonly string _connectionString;

        public ProductoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Registrar(Producto producto)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                string consulta = @"INSERT INTO Productos (Id, codigo, nombre, categoria, unidadMedida, precio, existenciaActual, existenciaMinima, ProveedorId) 
                                    VALUES (@Id, @codigo, @nombre, @categoria, @unidadMedida, @precio, @existenciaActual, @existenciaMinima, @ProveedorId)";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    if (producto.Id == Guid.Empty) producto.Id = Guid.NewGuid();

                    comando.Parameters.AddWithValue("@Id", producto.Id);
                    comando.Parameters.AddWithValue("@codigo", producto.codigo);
                    comando.Parameters.AddWithValue("@nombre", producto.nombre);
                    comando.Parameters.AddWithValue("@categoria", producto.categoria);
                    comando.Parameters.AddWithValue("@unidadMedida", producto.unidadMedida);
                    comando.Parameters.AddWithValue("@precio", producto.precio);
                    comando.Parameters.AddWithValue("@existenciaActual", producto.existenciaActual);
                    comando.Parameters.AddWithValue("@existenciaMinima", producto.existeciaMinima);

                    // Nota: Aquí asumimos que en tu clase Producto el proveedor es string. 
                    // Si SQL espera un Guid, habrá que hacer una conversión. 
                    comando.Parameters.AddWithValue("@ProveedorId", producto.proveedorId);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();
                    return filas > 0;
                }
            }
        }

        public List<Producto> Listar()
        {
            var lista = new List<Producto>();
            using (var conexion = new SqlConnection(_connectionString))
            {
                string consulta = "SELECT Id, codigo, nombre, categoria, unidadMedida, precio, existenciaActual, existenciaMinima, ProveedorId FROM Productos";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Producto
                            {
                                Id = (Guid)lector["Id"],
                                codigo = lector["codigo"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                categoria = lector["categoria"].ToString(),
                                unidadMedida = lector["unidadMedida"].ToString(),
                                precio = Convert.ToDouble(lector["precio"]),
                                existenciaActual = Convert.ToInt32(lector["existenciaActual"]),
                                existeciaMinima = Convert.ToInt32(lector["existenciaMinima"]),
                                proveedorId = (Guid)lector["ProveedorId"] // Se lee como Guid
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Producto BuscarPorId(Guid id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                string consulta = "SELECT Id, codigo, nombre, categoria, unidadMedida, precio, existenciaActual, existenciaMinima, ProveedorId FROM Productos WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Producto
                            {
                                Id = (Guid)lector["Id"],
                                codigo = lector["codigo"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                categoria = lector["categoria"].ToString(),
                                unidadMedida = lector["unidadMedida"].ToString(),
                                precio = Convert.ToDouble(lector["precio"]),
                                existenciaActual = Convert.ToInt32(lector["existenciaActual"]),
                                existeciaMinima = Convert.ToInt32(lector["existenciaMinima"]),
                                proveedorId = (Guid)lector["ProveedorId"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool Actualizar(Producto producto)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                string consulta = @"UPDATE Productos 
                                    SET codigo = @codigo, nombre = @nombre, categoria = @categoria, unidadMedida = @unidadMedida, 
                                        precio = @precio, existenciaActual = @existenciaActual, existenciaMinima = @existenciaMinima, ProveedorId = @ProveedorId 
                                    WHERE Id = @Id";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", producto.Id);
                    comando.Parameters.AddWithValue("@codigo", producto.codigo);
                    comando.Parameters.AddWithValue("@nombre", producto.nombre);
                    comando.Parameters.AddWithValue("@categoria", producto.categoria);
                    comando.Parameters.AddWithValue("@unidadMedida", producto.unidadMedida);
                    comando.Parameters.AddWithValue("@precio", producto.precio);
                    comando.Parameters.AddWithValue("@existenciaActual", producto.existenciaActual);
                    comando.Parameters.AddWithValue("@existenciaMinima", producto.existeciaMinima);
                    comando.Parameters.AddWithValue("@ProveedorId", producto.proveedorId);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();
                    return filas > 0;
                }
            }
        }

        public bool Eliminar(Guid id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                string consulta = "DELETE FROM Productos WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();
                    return filas > 0;
                }
            }
        }
    }
}