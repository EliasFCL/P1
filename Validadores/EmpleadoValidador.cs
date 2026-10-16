using Proyecto1_EliasCorderoLeon.Models;
using System.Text.RegularExpressions;

namespace Proyecto1_EliasCorderoLeon.Validadores
{
    public class EmpleadoValidador
    {
        public List<string> Validar(Empleado empleado)
        {

            List<string> errores = new List<string>();
            ValidarPersona validador = new ValidarPersona();

            DateTime mayorEdad = new DateTime(2008, 5, 5);//Fecha límite para ser empleado 
            var fechaActual = empleado.fechaNacimiento;

            if (empleado.salario <= 0 || empleado.salario > 5000000 || empleado.salario.ToString().Trim() == "")
            {
                errores.Add("El salario del empleado debe estar entre 0 y 5000000 colones. Debe ser completo");
            }
            // Validar fecha
            if (mayorEdad < fechaActual)
            {
                errores.Add("Debe ser mayor de edad");
            }
            if (empleado.fechaIngreso == default(DateTime?))
            {
                errores.Add("La fecha de ingreso no puede estar por defecto");
            }
            else if (empleado.fechaIngreso > DateTime.Now)
            {
                errores.Add("La fecha de ingreso no puede ser mayor a la fecha actual");
            }

            if(empleado.areaTrabajo == null)
            {
                errores.Add("Debe seleccionar un área de trabajo");
            }

            //Validar turno
            if (empleado.turno == null)
            {
                errores.Add("Debe seleccionar un turno");
            }

            errores.AddRange(validador.Validar(empleado));

            return errores;
        }
    }
}