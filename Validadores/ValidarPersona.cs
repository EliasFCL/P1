using Proyecto1_EliasCorderoLeon.Models;
using System.Text.RegularExpressions;

namespace Proyecto1_EliasCorderoLeon.Validadores
{
    public class ValidarPersona
    {
        public List<string> Validar(Persona persona)
        {
            List<string> errores = new List<string>();//Lista para devolver errores
            try
            {
                //Validar el nombre
                if (string.IsNullOrWhiteSpace(persona.nombre))
            {
                errores.Add("El nombre no puede estar vacío");
            }
            else if (!Regex.IsMatch(persona.nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errores.Add("El nombre solo puede contener letras y espacios");
            }
            else if (persona.nombre.Length < 3 || persona.nombre.Length > 50)
            {
                errores.Add("El nombre debe tener entre 3 y 50 caracteres");
            }

            //Validar apellidos
            if (string.IsNullOrWhiteSpace(persona.apellido1) || string.IsNullOrWhiteSpace(persona.apellido2))
            {
                errores.Add("Los apellidos no pueden estar vacío");
            }
            else if (!Regex.IsMatch(persona.apellido1, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$") || !Regex.IsMatch(persona.apellido2, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errores.Add("Los apellidos solo pueden contener letras y espacios");
            }
            else if ((persona.apellido1.Length < 3 || persona.apellido1.Length > 75) || (persona.apellido2.Length < 3 || persona.apellido2.Length > 75))
            {
                errores.Add("Los apellidos deben tener entre 3 y 75 caracteres");
            }
           
                //validar identificacion
                if (persona.numId == null || persona.numId.Trim() == "")
                {
                    errores.Add("El número de identificación no puede estar vacío");
                }
                else
                {
                    if (persona.tipoIdentificacion == "Cédula" || persona.tipoIdentificacion == "Cédula nacional")
                    {

                        if (!Regex.IsMatch(persona.numId, @"^\d-\d{4}-\d{4}$"))
                        {
                            errores.Add("La Cédula debe tener el formato correcto (X-XXXX-XXXX)");
                        }

                    }
                    else if (persona.tipoIdentificacion == "DIMEX" && !Regex.IsMatch(persona.numId, @"^\d{12}$"))
                        {
                            errores.Add("El DIMEX debe tener 12 dígitos numericos");
                        }
                    else if (persona.tipoIdentificacion == "Pasaporte" && !Regex.IsMatch(persona.numId, @"^[A-Za-z0-9]{1,50}$"))
                        {
                            errores.Add("El Pasaporte debe tener entre 1 y 50 caracteres alfanumericos sin espacios");
                        }
                     else if (persona.tipoIdentificacion == "Cédula jurídica" && !Regex.IsMatch(persona.numId, @"^\d-\d{3}-\d{6}$"))
                        {
                            errores.Add("La Cédula debe tener el formato correcto (X-XXX-XXXXXX)");
                        }      
                }
            }
            catch (Exception ex)
            {
                errores.Add("Error al validar el número de identificación: " + ex.Message);
            }
            //Validar fecha 
            if (persona.fechaNacimiento == default(DateTime?))
            {
                errores.Add("La fecha de nacimiento no puede estar por defecto");
            }
            else if (persona.fechaNacimiento > DateTime.Now)
            {
                errores.Add("La fecha de nacimiento no puede ser mayor a la fecha actual");
            }
            return errores;
        }
    }
}