// Buscar persona: https://www.youtube.com/watch?v=pGzcd3ucIfo&feature=youtu.be 40:45
using Models;

namespace Carniceria.API.Modulos
{
    public class ModuloCliente
    {
        private static List<Cliente> listaClientes = new List<Cliente>();
        //Dator por efecto
        public ModuloCliente()
        {
            listaClientes.Add(new Cliente
            {
                Id = 1,
                tipoIdentificacion = "Cédula",
                numId = "2-0851-0970",
                nombre = "Elias",
                apellido1 = "Cordero",
                apellido2 = "Leon",
                fechaNacimiento = new DateTime(2004, 3, 25)
            });
        }
        public void RegistrarCliente(Cliente cliente)
        { 
            cliente.Id = listaClientes.Any() ? listaClientes.Max(c => c.Id) + 1 : 1;
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