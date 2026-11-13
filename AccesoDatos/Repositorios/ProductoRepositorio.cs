using Microsoft.Data.SqlClient;
using Models;
using AccesoDatos.Contexto;
using AccesoDatos.Configuraciones;

namespace AccesoDatos.Repositorios
{
    public class ProductoRepositorio
    {
        private readonly string conexionString;
        Conectar conectar = new Conectar();
        public ProductoRepositorio(string connectionString)
        {
            conexionString = conectar.CadenaConexion();
        }

        public bool Registrar(Producto producto)
        {
            using (var conexion = new SqlConnection(conexionString))
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
            using (var conexion = new SqlConnection(conexionString))
            {
                // Hacemos el JOIN con la tabla Proveedores
                string consulta = @"
            SELECT p.Id, p.codigo, p.nombre, p.categoria, p.unidadMedida, p.precio, 
                   p.existenciaActual, p.existenciaMinima, p.ProveedorId, 
                   pr.nombre AS nombreProveedor
            FROM Productos p
            LEFT JOIN Proveedores pr ON p.ProveedorId = pr.Id";

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
                                precio = Convert.ToDecimal(lector["precio"]),
                                existenciaActual = Convert.ToDecimal(lector["existenciaActual"]),
                                existeciaMinima = Convert.ToDecimal(lector["existenciaMinima"]),

                                proveedorId = (Guid)lector["ProveedorId"],

                                // Guardar nombre que trajo el JOIN
                                nombreProveedor = lector["nombreProveedor"] == DBNull.Value ? "Sin Proveedor" : lector["nombreProveedor"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Producto BuscarPorId(Guid id)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = @"
            SELECT p.Id, p.codigo, p.nombre, p.categoria, p.unidadMedida, p.precio, 
                   p.existenciaActual, p.existenciaMinima, p.ProveedorId, 
                   pr.nombre AS nombreProveedor
            FROM Productos p
            LEFT JOIN Proveedores pr ON p.ProveedorId = pr.Id
            WHERE p.Id = @Id";

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
                                precio = Convert.ToDecimal(lector["precio"]),
                                existenciaActual = Convert.ToInt32(lector["existenciaActual"]),
                                existeciaMinima = Convert.ToInt32(lector["existenciaMinima"]),
                                proveedorId = (Guid)lector["ProveedorId"],
                                nombreProveedor = lector["nombreProveedor"] == DBNull.Value ? "Sin Proveedor" : lector["nombreProveedor"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool Actualizar(Producto producto)
        {
            using (var conexion = new SqlConnection(conexionString))
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
            using (var conexion = new SqlConnection(conexionString))
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

        public decimal? ObtenerPrecioPorId(Guid id)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT precio FROM Productos WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    var resultado = comando.ExecuteScalar();
                    return resultado != null ? Convert.ToDecimal(resultado) : (decimal?)null;
                }
            }
        }

        public decimal? ObtenerPrecioPorCombinacion(string categoria, string unidadMedida)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                //Buscar si existe un producto con esa categoría y unidad
                string consulta = "SELECT TOP 1 precio FROM Productos WHERE categoria = @categoria AND unidadMedida = @unidad";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@categoria", categoria);
                    comando.Parameters.AddWithValue("@unidad", unidadMedida);
                    conexion.Open();
                    var resultado = comando.ExecuteScalar();

                    return resultado != null ? Convert.ToDecimal(resultado) : (decimal?)null;
                }
            }
        }
    }
}