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
       
        public bool Registrar(Empleado empleado)
        {
            var conectar = new Conectar();
            using (var conexion = new SqlConnection(conectar.CadenaConexion()))
            {
                string consulta = "INSERT INTO Empleados (Nombre, Apellido, NumID) VALUES (@Nombre, @Apellido, @NumID)";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", empleado.nombre);
                    comando.Parameters.AddWithValue("@Apellido", empleado.apellido1);
                    comando.Parameters.AddWithValue("@NumID", empleado.numId);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            return true; // Retorna true si el registro fue exitoso, false en caso contrario
        }

        public bool Actualizar(Empleado empleado)
        {
            using (var conexion = new SqlConnection("tu_cadena_de_conexion_aqui"))
            {
                string consulta = "UPDATE Empleados SET Nombre = @Nombre, Apellido = @Apellido WHERE NumID = @NumID";
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", empleado.nombre);
                    comando.Parameters.AddWithValue("@Apellido", empleado.apellido1);
                    comando.Parameters.AddWithValue("@NumID", empleado.numId);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0; // Retorna true si se actualizó al menos una fila
                }
            }
        }
    }
}
