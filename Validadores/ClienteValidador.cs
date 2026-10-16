using Proyecto1_EliasCorderoLeon.Models;
using System.Text.RegularExpressions;

namespace Proyecto1_EliasCorderoLeon.Validadores
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