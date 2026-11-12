using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Microsoft.Data.SqlClient;
using AccesoDatos.Contexto;

namespace AccesoDatos.Repositorios
{
    public class EmpleadoRepositorio
    {
        private readonly string conexionString;
        Conectar conectar = new Conectar();
        public EmpleadoRepositorio(string connectionString)
        {
            conexionString = conectar.CadenaConexion();
        }

        public bool Registrar(Empleado empleado)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                //Buscar Guid del puesto
                Guid puestoIdReal = Guid.Empty;
                string consultaPuesto = @"
            SELECT p.Id 
            FROM Puestos p
            INNER JOIN AreaTrabajo a ON p.AreaTrabajoId = a.Id
            WHERE p.nombre = @cargo AND a.nombre = @area";

                using (var comandoBuscar = new SqlCommand(consultaPuesto, conexion))
                {
                    comandoBuscar.Parameters.AddWithValue("@cargo", empleado.cargo);
                    comandoBuscar.Parameters.AddWithValue("@area", empleado.areaTrabajo);

                    var resultado = comandoBuscar.ExecuteScalar();
                    if (resultado != null)
                    {
                        puestoIdReal = (Guid)resultado;
                    }
                    else
                    {
                        return false; //No se encontró el puesto
                    }
                }

                //Insertar el empleado
                string consultaInsert = @"INSERT INTO Empleados (Id, tipoIdentificacion, numId, nombre, apellido1, apellido2, fechaNacimiento, salario, fechaIngreso, turno, PuestoId) 
                                  VALUES (@Id, @tipoIdentificacion, @numId, @nombre, @apellido1, @apellido2, @fechaNacimiento, @salario, @fechaIngreso, @turno, @PuestoId)";

                using (var comando = new SqlCommand(consultaInsert, conexion))
                {
                    if (empleado.Id == Guid.Empty) empleado.Id = Guid.NewGuid();

                    comando.Parameters.AddWithValue("@Id", empleado.Id);
                    comando.Parameters.AddWithValue("@tipoIdentificacion", empleado.tipoIdentificacion);
                    comando.Parameters.AddWithValue("@numId", empleado.numId);
                    comando.Parameters.AddWithValue("@nombre", empleado.nombre);
                    comando.Parameters.AddWithValue("@apellido1", empleado.apellido1);
                    comando.Parameters.AddWithValue("@apellido2", empleado.apellido2);
                    comando.Parameters.AddWithValue("@fechaNacimiento", empleado.fechaNacimiento ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@salario", empleado.salario);
                    comando.Parameters.AddWithValue("@fechaIngreso", empleado.fechaIngreso);
                    comando.Parameters.AddWithValue("@turno", empleado.turno);
                    comando.Parameters.AddWithValue("@PuestoId", puestoIdReal);

                    int filas = comando.ExecuteNonQuery();
                    return filas > 0;
                }
            }
        }

        public List<Empleado> Listar()
        {
            var lista = new List<Empleado>();
            using (var conexion = new SqlConnection(conexionString))
            {
                //JOIN para obtener el cargo y área
                string consulta = @"
            SELECT e.Id, e.tipoIdentificacion, e.numId, e.nombre, e.apellido1, e.apellido2, 
                   e.fechaNacimiento, e.salario, e.fechaIngreso, e.turno, e.PuestoId,
                   p.nombre AS cargo, a.nombre AS areaTrabajo
            FROM Empleados e
            INNER JOIN Puestos p ON e.PuestoId = p.Id
            INNER JOIN AreaTrabajo a ON p.AreaTrabajoId = a.Id";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Empleado
                            {
                                Id = lector.GetGuid(lector.GetOrdinal("Id")),
                                tipoIdentificacion = lector["tipoIdentificacion"].ToString(),
                                numId = lector["numId"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                apellido1 = lector["apellido1"].ToString(),
                                apellido2 = lector["apellido2"].ToString(),
                                fechaNacimiento = lector.IsDBNull(lector.GetOrdinal("fechaNacimiento")) ? (DateTime?)null : lector.GetDateTime(lector.GetOrdinal("fechaNacimiento")),
                                salario = Convert.ToDecimal(lector["salario"]),
                                fechaIngreso = lector.GetDateTime(lector.GetOrdinal("fechaIngreso")),
                                turno = lector["turno"].ToString(),
                                puestoID = lector.GetGuid(lector.GetOrdinal("PuestoId")),

                                cargo = lector["cargo"].ToString(),
                                areaTrabajo = lector["areaTrabajo"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Empleado BuscarPorId(Guid id)
        {
            using (var conexion = new SqlConnection(conexionString))
            {

                string consulta = @"
            SELECT e.Id, e.tipoIdentificacion, e.numId, e.nombre, e.apellido1, e.apellido2, 
                   e.fechaNacimiento, e.salario, e.fechaIngreso, e.turno, e.PuestoId,
                   p.nombre AS cargo, a.nombre AS areaTrabajo
            FROM Empleados e
            LEFT JOIN Puestos p ON e.PuestoId = p.Id
            LEFT JOIN AreaTrabajo a ON p.AreaTrabajoId = a.Id
            WHERE e.Id = @Id";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Empleado
                            {
                                Id = (Guid)lector["Id"],
                                tipoIdentificacion = lector["tipoIdentificacion"].ToString(),
                                numId = lector["numId"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                apellido1 = lector["apellido1"].ToString(),
                                apellido2 = lector["apellido2"].ToString(),
                                fechaNacimiento = lector["fechaNacimiento"] == DBNull.Value ? null : (DateTime)lector["fechaNacimiento"],
                                salario = Convert.ToDecimal(lector["salario"]),
                                fechaIngreso = (DateTime)lector["fechaIngreso"],
                                turno = lector["turno"].ToString(),
                                puestoID = (Guid)lector["PuestoId"],
                                // Validación por si el LEFT JOIN no trae datos
                                cargo = lector["cargo"] == DBNull.Value ? "Sin Cargo" : lector["cargo"].ToString(),
                                areaTrabajo = lector["areaTrabajo"] == DBNull.Value ? "Sin Área" : lector["areaTrabajo"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Empleado BuscarPorNumId(string numId)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                string consulta = @"
            SELECT e.Id, e.tipoIdentificacion, e.numId, e.nombre, e.apellido1, e.apellido2, 
                   e.fechaNacimiento, e.salario, e.fechaIngreso, e.turno, e.PuestoId,
                   p.nombre AS cargo, a.nombre AS areaTrabajo
            FROM Empleados e
            INNER JOIN Puestos p ON e.PuestoId = p.Id
            INNER JOIN AreaTrabajo a ON p.AreaTrabajoId = a.Id
            WHERE e.numId = @numId";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@numId", numId);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Empleado
                            {
                                Id = (Guid)lector["Id"],
                                tipoIdentificacion = lector["tipoIdentificacion"].ToString(),
                                numId = lector["numId"].ToString(),
                                nombre = lector["nombre"].ToString(),
                                apellido1 = lector["apellido1"].ToString(),
                                apellido2 = lector["apellido2"].ToString(),
                                fechaNacimiento = lector["fechaNacimiento"] == DBNull.Value ? null : (DateTime)lector["fechaNacimiento"],
                                salario = Convert.ToDecimal(lector["salario"]),
                                fechaIngreso = (DateTime)lector["fechaIngreso"],
                                turno = lector["turno"].ToString(),
                                puestoID = (Guid)lector["PuestoId"],
                                cargo = lector["cargo"] == DBNull.Value ? "Sin Cargo" : lector["cargo"].ToString(),
                                areaTrabajo = lector["areaTrabajo"] == DBNull.Value ? "Sin Área" : lector["areaTrabajo"].ToString()
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
                string consulta = "DELETE FROM Empleados WHERE Id = @Id";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0; // Retorna true si se eliminó al menos una fila
                }
            }
        }

        public bool Actualizar(Guid id, Empleado empleado)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                //Buscar Guid del puesto
                Guid puestoIdReal = Guid.Empty;
                string consultaPuesto = @"
            SELECT p.Id 
            FROM Puestos p
            INNER JOIN AreaTrabajo a ON p.AreaTrabajoId = a.Id
            WHERE p.nombre = @cargo AND a.nombre = @area";
                using (var comandoBuscar = new SqlCommand(consultaPuesto, conexion))
                {
                    comandoBuscar.Parameters.AddWithValue("@cargo", empleado.cargo);
                    comandoBuscar.Parameters.AddWithValue("@area", empleado.areaTrabajo);
                    conexion.Open();
                    var resultado = comandoBuscar.ExecuteScalar();
                    if (resultado != null)
                    {
                        puestoIdReal = (Guid)resultado;
                    }
                    else
                    {
                        return false; //No se encontró el puesto
                    }
                }
                string consultaUpdate = @"UPDATE Empleados 
                                          SET tipoIdentificacion = @tipoIdentificacion,
                                              numId = @numId,
                                              nombre = @nombre,
                                              apellido1 = @apellido1,
                                              apellido2 = @apellido2,
                                              fechaNacimiento = @fechaNacimiento,
                                              salario = @salario,
                                              fechaIngreso = @fechaIngreso,
                                              turno = @turno,
                                              PuestoId = @PuestoId
                                          WHERE Id = @Id";
                using (var comando = new SqlCommand(consultaUpdate, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    comando.Parameters.AddWithValue("@tipoIdentificacion", empleado.tipoIdentificacion);
                    comando.Parameters.AddWithValue("@numId", empleado.numId);
                    comando.Parameters.AddWithValue("@nombre", empleado.nombre);
                    comando.Parameters.AddWithValue("@apellido1", empleado.apellido1);
                    comando.Parameters.AddWithValue("@apellido2", empleado.apellido2);
                    comando.Parameters.AddWithValue("@fechaNacimiento", empleado.fechaNacimiento ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@salario", empleado.salario);
                    comando.Parameters.AddWithValue("@fechaIngreso", empleado.fechaIngreso);
                    comando.Parameters.AddWithValue("@turno", empleado.turno);
                    comando.Parameters.AddWithValue("@PuestoId", puestoIdReal);
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0; // Retorna true si se actualizó al menos una fila
                }

            }
            
        }
    }
}


