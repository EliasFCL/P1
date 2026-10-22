using Models;

namespace Carniceria.API.Validadores
{
    public class FacturaValidador
    {
        public List<string> Validar(Factura factura)
        {
            List<string> errores = new List<string>();

            if (factura.descuento < 0 || factura.descuento > 25)
            {
                errores.Add("El descuento debe ser un número entero entre 0 y 25.");
            }

            return errores;
        }
    }
}