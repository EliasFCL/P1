using System.ComponentModel.DataAnnotations;

namespace Proyecto1_EliasCorderoLeon.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string tipoIdentificacion { get; set; }
        [Required(ErrorMessage = "La identificación no puede estar vacia")]
        public string numId { get; set; }
        [Required(ErrorMessage = "El nombre no puede estar vacio")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "El primer apellido no puede estar vacio")]
        public string apellido1 { get; set; }
        [Required(ErrorMessage = "El segundo apellido no puede estar vacio")]
        public string apellido2 { get; set; }
        public DateTime? fechaNacimiento { get; set; } //Evitar que aparesza con formato 00/00/00 --:00

    }
}
