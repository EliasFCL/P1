//Registrar adaptado: https://stackoverflow.com/questions/60006737/efcore-dbconnection-createcommand-parameters-not-passing
using AccesoDatos.Contexto;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Repositorios
{
    public class EmpleadoRepositorio
    {
        private readonly EmpleadoDbContext contexto;

        //Obtenter datos de la BD
        public IReadOnlyCollection<Empleado> Listar => contexto.Empleados.AsNoTracking().OrderBy(e => e.nombre).ToList();
        public Empleado? buscarPorId(Guid id) => contexto.Empleados.AsNoTracking().FirstOrDefault(e => e.Id == id);
        public Empleado? buscarPorNumId(string numId) => contexto.Empleados.AsNoTracking().FirstOrDefault(e => e.numId == numId);

        //Contexto para acceder a la BD
        public EmpleadoRepositorio(EmpleadoDbContext contexto)
        {
            this.contexto = contexto;
        }

        public bool Registrar(Empleado empleado)
        {
            //Comprobar Identificación duplicada
            if (contexto.Empleados.Any(existe => existe.numId == empleado.numId))
            {
                return false;
            }

            //Verificar Boxes sin datos
            if (empleado.puestoID == Guid.Empty && !string.IsNullOrEmpty(empleado.cargo))
            {
                //Establecer conexión
                var conexion = contexto.Database.GetDbConnection();
                if (conexion.State != System.Data.ConnectionState.Open)
                {
                    conexion.Open();//Abrir conexión
                }
                
                using (var consulta = conexion.CreateCommand())
                {
                    consulta.CommandText = "SELECT Id FROM Puestos WHERE nombre = @nombre";//Preparar consulta

                    var parametro = consulta.CreateParameter();

                    //Establecer el parametro
                    parametro.ParameterName = "@nombre";
                    parametro.Value = empleado.cargo;
                    consulta.Parameters.Add(parametro);

                    var resultado = consulta.ExecuteScalar();//Ejecutar la consulta
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

            contexto.Empleados.Add(empleado);//Añadir al contexto
            contexto.SaveChanges();//Guardar los datos
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
                existe.salario = empleado.salario;
                existe.fechaIngreso = empleado.fechaIngreso;
                existe.cargo = empleado.cargo;
                existe.areaTrabajo = empleado.areaTrabajo;
                existe.turno = empleado.turno;
                contexto.SaveChanges();
                return true;
            }
            return false;
        }
    }
}