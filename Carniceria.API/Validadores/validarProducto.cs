using Models;
using System.Text.RegularExpressions;

namespace Carniceria.API.Validadores
{
    public class validarProducto
    {
        public List<string> Validar(Producto producto)
        {
            List<string> errores = new List<string>();
            //Validar código
            if (string.IsNullOrWhiteSpace(producto.codigo))
            {
                errores.Add("El código del producto es obligatorio");
            } else if (producto.codigo.Length > 20 || producto.codigo.Length < 1)
            {
                errores.Add("El código del producto no puede tener más de 20 caracteres y menos de 1");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(producto.codigo, @"^[a-zA-Z0-9]+$"))
            {
                errores.Add("El codigo del producto solo puede contener letras y números");
            }

            //Validar nombre
            if (string.IsNullOrWhiteSpace(producto.nombre))
            {
                errores.Add("El nombre del producto es obligatorio.");
            } else if (producto.nombre.Length > 80 || producto.nombre.Length < 3) {
                errores.Add("El nombre debe contenere entre 3 y 80 caracteres alfanuméricos.");
            } else if (!System.Text.RegularExpressions.Regex.IsMatch(producto.nombre, @"^[a-zA-Z0-9\s]*$"))
            {
                errores.Add("El nombre del producto solo puede contener letras,espacios y números");
            }

            //Validar categoría
            if (string.IsNullOrWhiteSpace(producto.categoria))
            {
                errores.Add("La categoría del producto es obligatoria.");
            }

            //Validar unidad de medida
            if (string.IsNullOrWhiteSpace(producto.unidadMedida))
            {
                errores.Add("La unidad de medida del producto es obligatoria.");
            }

            //Validar precio
            if (producto.precio == null || producto.precio == 0)
            {
                errores.Add("El precio del producto debe ser mayor que cero.");
            }

            //Validar existencias
            if (producto.existenciaActual <= 0 || producto.existenciaActual == null)
            {
                errores.Add("La existencia actual del producto debe ser mayor o igual a 0");
            }
            if (producto.existeciaMinima < 0 || producto.existeciaMinima == null)
            {
                errores.Add("La existencia mínima del producto debe ser mayor o igual a cero.");
            }
            if (producto.existenciaActual <= producto.existeciaMinima)
            {
                errores.Add("Existencias insuficientes: la cantidad actual del producto es menor que la existencia minima permitida");
            }
            if (producto.existeciaMinima.ToString().StartsWith("-") || producto.existenciaActual.ToString().StartsWith("-"))
            {
                errores.Add("Ninguna existencia puede tener un valor negativo");
            }

            if (producto.proveedor == null)
            {
                errores.Add("Seleccione un proveedor, verifique que haya al menos uno registrado");
            }

            return errores;
        }
    }
}