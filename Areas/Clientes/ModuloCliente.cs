// Buscar persona: https://www.youtube.com/watch?v=pGzcd3ucIfo&feature=youtu.be 40:45
using Proyecto1_EliasCorderoLeon.Models;

namespace Proyecto1_EliasCorderoLeon.Areas.Clientes
{
    public class ModuloCliente
    {
        private List<Cliente> listaClientes = new List<Cliente>();

        public void RegistrarCliente(Cliente cliente)
        {
            listaClientes.Add(cliente);
        }

        public Cliente BuscarCliente(string numId)
        {
            var cliente = listaClientes.FirstOrDefault(c => c.numId == numId);

            return cliente;
        }

        public List<Cliente> ListarClientes()
        {
            return listaClientes;
        }

        public bool ActualizarCliente(string Id, Cliente clienteActualizado)
        {
            var cliente = listaClientes.FirstOrDefault(c => c.Id == int.Parse(Id));

            if (cliente != null)
            {
                cliente.tipoIdentificacion = clienteActualizado.tipoIdentificacion;
                cliente.numId = clienteActualizado.numId;
                cliente.nombre = clienteActualizado.nombre;
                cliente.apellido1 = clienteActualizado.apellido1;
                cliente.apellido2 = clienteActualizado.apellido2;
                cliente.fechaNacimiento = clienteActualizado.fechaNacimiento;

                return true;
            }

            return false;
        }

        public bool EliminarCliente(string Id)
        {
            var cliente = listaClientes.FirstOrDefault(c => c.Id == int.Parse(Id));

            if (cliente != null)
            {
                listaClientes.Remove(cliente);

                return true;
            }

            return false;
        }
    }
}