using Models;

namespace Carniceria.API.Modulos
{
    public class ModuloEmpleado
    {
        private List<Empleado> listaEmpleados = new List<Empleado>();

        public ModuloEmpleado()
        {
            listaEmpleados.Add(new Empleado
            {
                Id = 1,
                tipoIdentificacion = "DIMEX",
                numId = "101010121212",
                nombre = "Ellie",
                apellido1 = "Williams",
                apellido2 = "Miller",
                fechaNacimiento = new DateTime(2007, 4, 14),
                cargo = "Administrador",
                salario = 500000,
                fechaIngreso = new DateTime(2023, 1, 15),
                areaTrabajo = "Asistente administrativo",
                turno = "Mañana"
            });
        }
        public void RegistrarEmpleado(Empleado empleado)
        {
            empleado.Id = listaEmpleados.Any() ? listaEmpleados.Max(e => e.Id) + 1 : 1;
            listaEmpleados.Add(empleado);
        }

        public Empleado BuscarEmpleado(string numId)
        {
            var empleado = listaEmpleados.FirstOrDefault(e => e.numId == numId);

            return empleado;
        }

        public List<Empleado> ListarEmpleados()
        {
            return listaEmpleados;
        }

        public bool ActualizarEmpleado(string Id, Empleado empleadoActualizado)
        {
            var empleado = listaEmpleados.FirstOrDefault(e => e.Id.ToString() == Id.ToString());

            if (empleado != null)
            {
                empleado.tipoIdentificacion = empleadoActualizado.tipoIdentificacion;
                empleado.numId = empleadoActualizado.numId;
                empleado.nombre = empleadoActualizado.nombre;
                empleado.apellido1 = empleadoActualizado.apellido1;
                empleado.apellido2 = empleadoActualizado.apellido2;
                empleado.fechaNacimiento = empleadoActualizado.fechaNacimiento;
                empleado.cargo = empleadoActualizado.cargo;
                empleado.salario = empleadoActualizado.salario;
                empleado.fechaIngreso = empleadoActualizado.fechaIngreso;
                empleado.areaTrabajo = empleadoActualizado.areaTrabajo;
                empleado.turno = empleadoActualizado.turno;

                return true;
            }

            return false;
        }

        public bool EliminarEmpleado(string Id)
        {
            var empleado = listaEmpleados.FirstOrDefault(e => e.Id.ToString() == Id.ToString());

            if (empleado != null)
            {
                listaEmpleados.Remove(empleado);

                return true;
            }

            return false;
        }
    }
}