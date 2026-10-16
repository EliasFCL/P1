using Proyecto1_EliasCorderoLeon.Models;
using System.Text.RegularExpressions;

namespace Proyecto1_EliasCorderoLeon.Validadores
{
    public class ValidarProveedor
    {
        public List<string> Validar(Proveedor proveedor)
        {
            proveedor.nombre = "ABC";
            List<string> errores = new List<string>();
            ValidarPersona validador = new ValidarPersona();
            //Validar Razón social
            if(proveedor.RazonSocial == null || proveedor.RazonSocial.Trim() == "")
            {
                errores.Add("La razón social no puede estar vacía.");
            }
            else if (!Regex.IsMatch(proveedor.RazonSocial, @"^[a-zA-Z0-9\s]+$"))
            {
                errores.Add("La razón social solo puede contener caracteres alfanumericos y espacios");
            }
            else if (proveedor.RazonSocial.Length < 3 || proveedor.RazonSocial.Length > 100)
            {
                errores.Add("La razón social debe tener entre 3 y 100 caracteres.");
            }

            //Validar nombre
            if(proveedor.Nombre == null || proveedor.Nombre.Trim() == "")
            {
                errores.Add("El nombre no puede estar vacío.");
            }
            else if (!Regex.IsMatch(proveedor.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errores.Add("El nombre solo puede contener letras y espacios");
            }
            else if (proveedor.Nombre.Length < 3 || proveedor.Nombre.Length > 100)
            {
                errores.Add("El nombre debe tener entre 3 y 100 caracteres.");
            }

            //Validar telefono
            if(proveedor.Telefono < 10000000 || proveedor.Telefono > 99999999)
            {
                errores.Add("El teléfono debe tener 8 dígitos numéricos, que en conjunto formen un número válido.");
            }

            //Validar correo electrónico
            if (proveedor.CorreoElectronico == null || proveedor.CorreoElectronico.Trim() == "")
            {
                errores.Add("El correo electrónico no puede estar vacío");
            }else if (!Regex.IsMatch(proveedor.CorreoElectronico, @"^[^\s@]{3,}@gmail\.com$"))
            {
                errores.Add("El correo electrónico debe ser un correo válido de Gmail con al menos 3 caracteres antes del @.");
            }

            if (proveedor.TipoSuministro == null || proveedor.TipoSuministro.Trim() == "")
            {
                errores.Add("El tipo de suministro no puede estar vacío.");
            }

            if (proveedor.FrecuenciaEntrega == null || proveedor.FrecuenciaEntrega.Trim() == "")
            {
                errores.Add("La frecuencia de entrega no puede estar vacía.");
            }
            errores.AddRange(validador.Validar(proveedor));
            return errores;
        }
    }
}
