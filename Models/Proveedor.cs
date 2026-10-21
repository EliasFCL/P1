using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Models
{
    public class Proveedor : Persona
    {
        //[Required(ErrorMessage = "El nombre no puede estar vacio")]
        //public string Nombre { get; set; }
        [Required(ErrorMessage = "La razón social no puede estar vacia")]
        public string RazonSocial { get; set; }
        [Required(ErrorMessage = "El telefóno no puede estar vacio")]
        public int Telefono { get; set; }
        [Required(ErrorMessage = "El correo electrónico no puede estar vacio")]
        public string CorreoElectronico { get; set; }
        public string TipoSuministro { get; set; }
        public string FrecuenciaEntrega { get; set; }
    }   
}
