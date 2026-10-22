using Carniceria.API.Modulos;
using Models;

namespace Carniceria.API.Validadores
{
    public class FacturaValidador
    {
        public List<string> Validar(Factura factura)
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(factura.nombreCliente))
            {
                errores.Add("Debe seleccionar un cliente");
            }

            if (string.IsNullOrWhiteSpace(factura.empleadoResponsable))
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
                
                // Validar que los detalles no vengan con datos vacíos
                foreach (var detalle in factura.detalles)
                {
                    if (string.IsNullOrWhiteSpace(detalle.producto))
                    {
                        errores.Add("Hay una línea de detalle sin producto seleccionado");
                    }
                    if (detalle.cantidad <= 0)
                    {
                        errores.Add($"La cantidad para el producto '{detalle.producto}' debe ser mayor a 0");
                    }

                    var cantidad = moduloProducto.ListarProductos().FirstOrDefault(p => p.nombre == detalle.producto);
                    if (cantidad.existenciaActual < detalle.cantidad)
                    {
                        errores.Add($"La cantidad para el producto '{detalle.producto}' excede la existencia disponible de {cantidad.existenciaActual}");
                    }
                }
            }
            return errores;
        }
    }
}