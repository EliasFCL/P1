using System.ComponentModel.DataAnnotations;

namespace Proyecto1_EliasCorderoLeon.Models
{
    public class Empleado : Persona
    {
        [Required(ErrorMessage = "El salario no puede estar vacio")]
        public double salario { get; set; }
        public DateTime? fechaIngreso { get; set; }
        public string areaTrabajo { get; set; }
        public string cargo { get; set; }
        public string turno { get; set; }
    }
}
