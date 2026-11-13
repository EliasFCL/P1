using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Producto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El código no puede estar vacio")]
        public string codigo { get; set; }
        [Required(ErrorMessage = "El nombre no puede estar vacio")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La categoría")]
        public string categoria { get; set; }
        [Required(ErrorMessage = "La unidad de medida no puede estar vacia")]
        public string unidadMedida { get; set; }
        [Required(ErrorMessage = "El precio no puede estar vacio")]
        public decimal precio { get; set; }
        public decimal existenciaActual { get; set; }
        public decimal existeciaMinima { get; set; }
        public Guid proveedorId { get; set; }
        public string nombreProveedor { get; set; }
    }
}