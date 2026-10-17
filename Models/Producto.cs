using System.ComponentModel.DataAnnotations;

namespace Proyecto1_EliasCorderoLeon.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El código no puede estar vacio")]
        public string codigo { get; set; }
        [Required(ErrorMessage = "El nombre no puede estar vacio")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La categoría")]
        public string categoria { get; set; }
        [Required(ErrorMessage = "La unidad de medida no puede estar vacia")]
        public string unidadMedida { get; set; }
        [Required(ErrorMessage = "El precio no puede estar vacio")]
        public double precio { get; set; }
        public int? existenciaActual { get; set; }
        public int? existeciaMinima { get; set; }
    }
}