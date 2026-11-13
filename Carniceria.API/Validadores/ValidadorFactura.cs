using Carniceria.API.Modulos;
using Models;

namespace Carniceria.API.Validadores
{
    public class FacturaValidador
    {
        public List<string> Validar(Factura factura)
        {

            List<string> errores = new List<string>();

            if (factura.clienteId == Guid.Empty)
            {
                errores.Add("Debe seleccionar un cliente");
            }

            if (factura.empleadoId == Guid.Empty)
            {
                errores.Add("Debe seleccionar un empleado responsable");
            }

            if (string.IsNullOrWhiteSpace(factura.metodoPago))
            {
                errores.Add("Debe seleccionar un método de pago");
            }

            if (factura.descuento < 0 || factura.descuento > 25)
            {
                errores.Add("El descuento debe ser un número entero entre 0 y 25");
            }

            if (factura.detalles == null || !factura.detalles.Any())
            {
                errores.Add("La factura debe tener al menos una línea de detalle");
            }
            else
            {
                var moduloProducto = new ModuloProducto();

                foreach (var detalle in factura.detalles)
                {
                    //Validar que se haya seleccionado un producto
                    if (string.IsNullOrWhiteSpace(detalle.producto))
                    {
                        errores.Add("Hay una línea de detalle sin producto seleccionado");
                        continue; //Si no hay producto, saltarse la validación de la línea
                    }

                    if (detalle.cantidad <= 0)
                    {
                        errores.Add($"La cantidad para el producto '{detalle.producto}' debe ser mayor a 0");
                    }

                    //Buscar el producto 
                    var productoEncontrado = moduloProducto.ListarProductos()
                        .FirstOrDefault(p => p.nombre.Equals(detalle.producto, StringComparison.OrdinalIgnoreCase));

                    //Validar la existencia
                    if (productoEncontrado != null)
                    {
                        //?? 0 por si la existencia del producto es nula
                        if ((productoEncontrado.existenciaActual) < detalle.cantidad)
                        {
                            errores.Add($"La cantidad para el producto '{detalle.producto}' excede la existencia disponible de {productoEncontrado.existenciaActual}");
                        }
                    }
                    else
                    {
                        errores.Add($"El producto '{detalle.producto}' no existe en el inventario");
                    }
                }
            }
            return errores;
        }
    }
}