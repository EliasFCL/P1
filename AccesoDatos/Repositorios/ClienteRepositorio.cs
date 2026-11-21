//Repositorio: Tutoría impartida por el profesor Roger Lara el 16 de Noviembre

using AccesoDatos.Contexto;
using Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositorios
{
    public class ClienteRepositorio
    {
        private readonly ClienteDbContext contexto;
        public IReadOnlyCollection<Cliente> Listar => contexto.Clientes.AsNoTracking().OrderBy(c => c.nombre).ToList();
        public Cliente? buscarPorId(Guid id) => contexto.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == id);
        public Cliente? buscarPorNumId(string numId) => contexto.Clientes.AsNoTracking().FirstOrDefault(c => c.numId == numId);
        public ClienteRepositorio(ClienteDbContext contexto)
        {
            this.contexto = contexto;
        }


        public bool Registrar(Cliente cliente)
        {
            if (contexto.Clientes.Any(existing => existing.numId == cliente.numId))
            {
                return false;
            }
            contexto.Clientes.Add(cliente);
            contexto.SaveChanges();
            return true;
        }

        public void Actualizar(Guid id, Cliente cliente)
        {
            var existe = contexto.Clientes.FirstOrDefault(c => c.Id == id);
            if (existe != null)
            {
                existe.tipoIdentificacion = cliente.tipoIdentificacion;
                existe.numId = cliente.numId;
                existe.nombre = cliente.nombre;
                existe.apellido1 = cliente.apellido1;
                existe.apellido2 = cliente.apellido2;
                existe.fechaNacimiento = cliente.fechaNacimiento;

                contexto.SaveChanges();
            }
        }

        public bool eliminar(Guid id)
        {
            try
            {
                var existe = contexto.Clientes.FirstOrDefault(c => c.Id == id);

                if (existe != null)
                {
                    contexto.Clientes.Remove(existe);
                    contexto.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}