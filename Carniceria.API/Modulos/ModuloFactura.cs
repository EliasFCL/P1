//Logica para el numero de la factura: https://stackoverflow.com/posts/63604623/edit

using Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Carniceria.API.Modulos
{
    public class ModuloFactura
    {
        private static List<Factura> facturas = new List<Factura>();
        private static int FacturaCounter = 0;
        private static int DetalleCounter = 0;
        private static readonly Random _random = new Random();
        private const string _caracteres = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        //Metodo para generar un numero de factura
        private string GenerarNumeroFactura()
        {
            string numero;
            do
            {
                numero = "F-";
                for (int i = 0; i < 6; i++)
                {
                    numero += _caracteres[_random.Next(_caracteres.Length)];
                }
            }
            while (facturas.Any(f => f.numeroFactura == numero));

            return numero;
        }
        //Metodo de registro
        public void registrarFactura(Factura factura)
        {
            factura.numeroFactura = GenerarNumeroFactura();
            factura.fecha = DateTime.Now;

            foreach (var detalle in factura.detalles)
            {
                DetalleCounter++;
                detalle.Id = DetalleCounter;
                detalle.subtotal = detalle.cantidad * detalle.precioUnitario;
            }

            factura.subtotal = factura.detalles.Sum(d => d.subtotal);
            factura.montoDescuento = (factura.subtotal * factura.descuento) / 100;
            factura.baseGravable = factura.subtotal - factura.montoDescuento;
            factura.iva = factura.baseGravable * 0.13;
            factura.total = factura.baseGravable + factura.iva;

            facturas.Add(factura);
        }

        public Factura BuscarFactura(string numeroFactura)
        {
            var factura = facturas.FirstOrDefault(f => f.numeroFactura == numeroFactura);
            return factura;
        }

        //Metodo de eliminación
        public bool eliminarFactura(string numeroFactura)
        {
            var factura = facturas.FirstOrDefault(f => f.numeroFactura == numeroFactura);
            if (factura != null)
            {
                facturas.Remove(factura);
                return true;
            } else
            return false;
        }

        public List<Factura> ListarFacturas() => facturas;

        public bool ActualizarFactura(string numero, Factura facturaActualizada)
        {
            var factura = facturas.FirstOrDefault(f => f.numeroFactura == numero);
            if (factura == null) return false;

            // Actualizar cabecera
            factura.nombreCliente = facturaActualizada.nombreCliente;
            factura.empleadoResponsable = facturaActualizada.empleadoResponsable;
            factura.metodoPago = facturaActualizada.metodoPago;
            factura.descuento = facturaActualizada.descuento;
            factura.estado = facturaActualizada.estado;

            //Reemplazar detalles y recalcular subtotales
            factura.detalles = facturaActualizada.detalles ?? new List<DetalleFactura>();
            foreach (var d in factura.detalles)
            {
                // Si el detalle es nuevo (Id llega en 0), le asignamos uno
                if (d.Id == 0)
                {
                    DetalleCounter++;
                    d.Id = DetalleCounter;
                }
                d.subtotal = d.cantidad * d.precioUnitario;
            }

            // Recalcular totales
            factura.subtotal = factura.detalles.Sum(d => d.subtotal);
            factura.montoDescuento = (factura.subtotal * factura.descuento) / 100.0;
            factura.baseGravable = factura.subtotal - factura.montoDescuento;
            factura.iva = factura.baseGravable * 0.13;
            factura.total = factura.baseGravable + factura.iva;

            return true;
        }
    }
}
