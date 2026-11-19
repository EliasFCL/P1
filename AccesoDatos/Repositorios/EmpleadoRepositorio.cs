using AccesoDatos.Contexto;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Repositorios
{
    public class EmpleadoRepositorio
    {
        private readonly EmpleadoDbContext contexto;
        public IReadOnlyCollection<Empleado> Listar => contexto.Empleados.AsNoTracking().OrderBy(e => e.nombre).ToList();
        public Empleado? buscarPorId(Guid id) => contexto.Empleados.AsNoTracking().FirstOrDefault(e => e.Id == id);
        public Empleado? buscarPorNumId(string numId) => contexto.Empleados.AsNoTracking().FirstOrDefault(e => e.numId == numId);
        public EmpleadoRepositorio(EmpleadoDbContext contexto)
        {
            this.contexto = contexto;
        }

        public bool Registrar(Empleado empleado)
        {
            if (contexto.Empleados.Any(existing => existing.numId == empleado.numId))
            {
                return false;
            }

            if (empleado.puestoID == Guid.Empty && !string.IsNullOrEmpty(empleado.cargo))
            {
                var conexion = contexto.Database.GetDbConnection();
                if (conexion.State != System.Data.ConnectionState.Open)
                {
                    conexion.Open();
                }

                using (var consulta = conexion.CreateCommand())
                {
                    consulta.CommandText = "SELECT Id FROM Puestos WHERE nombre = @nombre";

                    var parametro = consulta.CreateParameter();
                    parametro.ParameterName = "@nombre";
                    parametro.Value = empleado.cargo;
                    consulta.Parameters.Add(parametro);

                    var resultado = consulta.ExecuteScalar();
                    if (resultado != null)
                    {
                        empleado.puestoID = (Guid)resultado;
                    }
                    else
                    {
                        return false; 
                    }
                }
            }

            contexto.Empleados.Add(empleado);
            contexto.SaveChanges();
            return true;
        }
        public bool eliminar(Guid id)
        {
            try
            {
                var existe = contexto.Empleados.FirstOrDefault(e => e.Id == id);

                if (existe != null)
                {
                    contexto.Empleados.Remove(existe);
                    contexto.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Actualizar(Guid id, Empleado empleado)
        {
            var  existe = contexto.Empleados.FirstOrDefault(e => e.Id == id);
            if(existe != null)
            {
                existe.tipoIdentificacion = empleado.tipoIdentificacion;
                existe.numId = empleado.numId;
                existe.nombre = empleado.nombre;
                existe.apellido1 = empleado.apellido1;
                existe.apellido2 = empleado.apellido2;
                existe.fechaNacimiento = empleado.fechaNacimiento;
                contexto.SaveChanges();
                return true;
            }
            return false;
        }
    }
}