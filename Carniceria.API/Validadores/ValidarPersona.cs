using Models;
using System.Text.RegularExpressions;

namespace Carniceria.API.Validadores
{
    public class ValidarPersona
    {
        public List<string> Validar(Persona persona)
        {
            List<string> errores = new List<string>(); //Lista para devolver errores
            try
            {
                //Validar el nombre
                if (string.IsNullOrWhiteSpace(persona.nombre))
                {
                    errores.Add("El nombre no puede estar vacío");
                }
                else if (!Regex.IsMatch(persona.nombre.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    errores.Add("El nombre solo puede contener letras y espacios");
                }
                else if (persona.nombre.Trim().Length < 3 || persona.nombre.Trim().Length > 50)
                {
                    errores.Add("El nombre debe tener entre 3 y 50 caracteres");
                }

                //Validar apellidos
                if (string.IsNullOrWhiteSpace(persona.apellido1) || string.IsNullOrWhiteSpace(persona.apellido2))
                {
                    errores.Add("Los apellidos no pueden estar vacíos");
                }
                else if (!Regex.IsMatch(persona.apellido1.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$") || !Regex.IsMatch(persona.apellido2.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    errores.Add("Los apellidos solo pueden contener letras y espacios");
                }
                else if ((persona.apellido1.Trim().Length < 3 || persona.apellido1.Trim().Length > 75) || (persona.apellido2.Trim().Length < 3 || persona.apellido2.Trim().Length > 75))
                {
                    errores.Add("Los apellidos deben tener entre 3 y 75 caracteres");
                }

                //validar identificacion
                if (string.IsNullOrWhiteSpace(persona.numId))
                {
                    errores.Add("El número de identificación no puede estar vacío");
                }
                else
                {
                    if (persona.tipoIdentificacion.Trim() == "Cédula" || persona.tipoIdentificacion.Trim() == "Cédula nacional")
                    {
                        if (!Regex.IsMatch(persona.numId.Trim(), @"^\d-\d{4}-\d{4}$"))
                        {
                            errores.Add("La Cédula debe tener el formato correcto (X-XXXX-XXXX)");
                        }
                    }
                    else if (persona.tipoIdentificacion.Trim() == "DIMEX" && !Regex.IsMatch(persona.numId.Trim(), @"^\d{12}$"))
                    {
                        errores.Add("El DIMEX debe tener 12 dígitos numéricos");
                    }
                    else if (persona.tipoIdentificacion.Trim() == "Pasaporte" && !Regex.IsMatch(persona.numId.Trim(), @"^[A-Za-z0-9]{1,50}$"))
                    {
                        errores.Add("El Pasaporte debe tener entre 1 y 50 caracteres alfanuméricos sin espacios");
                    }
                    else if (persona.tipoIdentificacion.Trim() == "Cédula jurídica" && !Regex.IsMatch(persona.numId.Trim(), @"^\d-\d{3}-\d{6}$"))
                    {
                        errores.Add("La Cédula debe tener el formato correcto (X-XXX-XXXXXX)");
                    }
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
            }
            catch (Exception ex)
            {
                errores.Add("Error inesperado al validar" + ex.Message);
            }

            return errores;
        }
    }
}