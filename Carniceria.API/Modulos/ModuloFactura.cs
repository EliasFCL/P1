//Logica para el numero de la factura: https://stackoverflow.com/posts/63604623/edit
//Adaptar la inicialización de objeto y colección: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers
using Models;

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
            factura.descuento = 14;
            factura.subtotal = factura.detalles.Sum(d => d.subtotal);
            factura.montoDescuento = (factura.subtotal * factura.descuento) / 100;
            factura.baseGravable = factura.subtotal - factura.montoDescuento;
            factura.iva = factura.baseGravable * 0.13;
            factura.total = factura.baseGravable + factura.iva;

            //Si es pagada, se resta la cantidad
            if (factura.estado == "Pagada")
            {
                foreach (var d in factura.detalles)
                {
                    AjustarInventario(d.producto, -d.cantidad);
                }
            }

            facturas.Add(factura);
        }

        public Factura BuscarFactura(string numeroFactura)
        {
            var factura = facturas.FirstOrDefault(f => f.numeroFactura == numeroFactura);
            return factura;
        }

        //Metodo de eliminación
        public bool eliminarFactura(string numero)
        {
            var factura = facturas.FirstOrDefault(f => f.numeroFactura == numero);
            if (factura != null)
            {
                //Solo las facturas pendientes pueden ser eliminadas
                if (factura.estado != "Pendiente")
                {
                    return false; //Rechazar operacion
                }
                facturas.Remove(factura);
                return true;
            }
            return false;
        }

        public List<Factura> ListarFacturas() => facturas;



        public bool ActualizarFactura(string numero, Factura facturaActualizada)
        {
            var facturaOriginal = facturas.FirstOrDefault(f => f.numeroFactura == numero);
            if (facturaOriginal == null || facturaOriginal.estado == "Anulada") return false;

            //Si antes estaba pagada, devolver las cantidades viejas
            if (facturaOriginal.estado == "Pagada")
            {
                foreach (var d in facturaOriginal.detalles)
                    AjustarInventario(d.producto, d.cantidad);
            }
            //Si ahora está pagada, descontar las cantidades nuevas
            if (facturaActualizada.estado == "Pagada")
            {
                foreach (var d in facturaActualizada.detalles)
                    AjustarInventario(d.producto, -d.cantidad);
            }

            //Actualizar la cabecera
            facturaOriginal.nombreCliente = facturaActualizada.nombreCliente;
            facturaOriginal.empleadoResponsable = facturaActualizada.empleadoResponsable;
            facturaOriginal.metodoPago = facturaActualizada.metodoPago;
            facturaOriginal.descuento = facturaActualizada.descuento;
            facturaOriginal.estado = facturaActualizada.estado;

            //Actualizar detalles y recalular totales
            facturaOriginal.detalles = facturaActualizada.detalles ?? new List<DetalleFactura>();
            foreach (var d in facturaOriginal.detalles)
            {
                if (d.Id == 0) d.Id = ++FacturaCounter;
                d.subtotal = d.cantidad * d.precioUnitario;
            }

            facturaOriginal.subtotal = facturaOriginal.detalles.Sum(d => d.subtotal);
            facturaOriginal.montoDescuento = (facturaOriginal.subtotal * facturaOriginal.descuento) / 100.0;
            facturaOriginal.baseGravable = facturaOriginal.subtotal - facturaOriginal.montoDescuento;
            facturaOriginal.iva = facturaOriginal.baseGravable * 0.13;
            facturaOriginal.total = facturaOriginal.baseGravable + facturaOriginal.iva;

            return true;
        }

        private void AjustarInventario(string nombreProducto, int cantidadCambio)
        {
            // Instanciamos el módulo de productos para buscar el producto
            var moduloProd = new ModuloProducto();
            var producto = moduloProd.ListarProductos().FirstOrDefault(p => p.nombre.Equals(nombreProducto, StringComparison.OrdinalIgnoreCase));

            if (producto != null)
            {
                //?? 0 por si la existencia es nula al inicio
                producto.existenciaActual = (producto.existenciaActual ?? 0) + cantidadCambio;
            }
        }

        public bool anularFactura(string numero)
        {
            var factura = facturas.FirstOrDefault(f => f.numeroFactura == numero);
            if (factura != null && factura.estado != "Anulada")
            {
                //Si estaba pagada, devolver las cantidades al inventario
                if (factura.estado == "Pagada")
                {
                    foreach (var d in factura.detalles)
                        AjustarInventario(d.producto, d.cantidad);
                }
                factura.estado = "Anulada";
                return true;
            }
            return false;
        }
        //Metodo para registrar una factura por defecto
        static ModuloFactura()
        {
            //Buscar los datos existentes en los otros módulos
            var productos = new ModuloProducto().ListarProductos();
            var cliente = new ModuloCliente().ListarClientes().FirstOrDefault();
            var empleado = new ModuloEmpleado().ListarEmpleados().FirstOrDefault();

            //Si hay al menos 1 producto, 1 cliente y 1 empleado, crea la factura
            if (productos.Count >= 1 && cliente != null && empleado != null)
            {
                var facturaDefecto = new Factura
                {
                    nombreCliente = cliente.nombre,
                    empleadoResponsable = empleado.nombre,
                    metodoPago = "Efectivo",
                    descuento = 0,
                    estado = "Pendiente",
                    detalles = new List<DetalleFactura>
        {
            //Usar el primer producto
            new DetalleFactura {
                producto = productos[0].nombre,
                unidadMedida = productos[0].unidadMedida,
                cantidad = 1,
                precioUnitario = productos[0].precio
            },
            //Usar el mismo producto para el minimo de 2 líneas de detalle
            new DetalleFactura {
                producto = productos[0].nombre,
                unidadMedida = productos[0].unidadMedida,
                cantidad = 2,
                precioUnitario = productos[0].precio
            }
        }
                };
                
                var instancia = new ModuloFactura();//Instancia para no dar error de estático
                instancia.registrarFactura(facturaDefecto);
            }
        }
    }
    }