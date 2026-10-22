using System;
using System.Collections.Generic;
using System.Text;


namespace Models
{
    public class Factura
    {
        public string? numeroFactura { get; set; }     // automatico
        public DateTime fecha { get; set; }
        public string nombreCliente { get; set; }
        public string empleadoResponsable { get; set; }
        public string metodoPago { get; set; }
        public int descuento { get; set; }            // porcentaje 
        public string estado { get; set; }

        // Calculados por el sistema (el usuario no debe verlos)
        public double subtotal { get; set; }
        public double montoDescuento { get; set; }
        public double baseGravable { get; set; }
        public double iva { get; set; }
        public double total { get; set; }

        public List<DetalleFactura> detalles { get; set; } = new List<DetalleFactura>();
    }

    public class DetalleFactura
    {
        public int Id { get; set; }
        public string producto { get; set; }
        public string unidadMedida { get; set; }
        public int cantidad { get; set; }
        public double precioUnitario { get; set; }
        public double subtotal { get; set; }          // cantidad × precioUnitario
    }
}