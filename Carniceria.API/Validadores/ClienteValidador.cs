using Models;
using System.Text.RegularExpressions;

namespace Carniceria.API.Validadores
{
    public class ClienteValidador
    {
        public List<string> Validar(Cliente cliente)
        {
            ValidarPersona validador = new ValidarPersona();

            return validador.Validar(cliente);
        }
    }
}