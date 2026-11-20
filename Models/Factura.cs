using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models
{
    public class Factura
    {
        [Key]
        public string? numeroFactura { get; set; }// automatico
        public DateTime fecha { get; set; }
        public Guid clienteId { get; set; }
        public Guid empleadoId { get; set; }
        [NotMapped]
        public string nombreCliente { get; set; }
        [NotMapped]
        public string empleadoResponsable { get; set; }
        public string metodoPago { get; set; }
        public decimal descuento { get; set; } // porcentaje 
        public string estado { get; set; }

        // Calculados por el sistema (el usuario no debe verlos)
        public decimal subtotal { get; set; }
        public decimal montoDescuento { get; set; }
        public decimal baseGravable { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }

        public List<DetalleFactura> detalles { get; set; } = new List<DetalleFactura>();
    }

    public class DetalleFactura
    {
        public Guid Id { get; set; }
        public string numeroFactura { get; set; }
        public Guid ProductoId { get; set; }
        [NotMapped]
        public string producto { get; set; }
        public string unidadMedida { get; set; }
        public decimal cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal subtotal { get; set; }//cantidad × precioUnitario
    }
}