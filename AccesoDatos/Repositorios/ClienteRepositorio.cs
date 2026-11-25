//Repositorio: Tutoría impartida por el profesor Roger Lara el 16 de Noviembre

using AccesoDatos.Contexto;
using Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositorios
{
    public class ClienteRepositorio
    {
        private readonly ClienteDbContext contexto;

        //Obtener datos de la BD
        public IReadOnlyCollection<Cliente> Listar => contexto.Clientes.AsNoTracking().OrderBy(c => c.nombre).ToList();
        public Cliente? buscarPorId(Guid id) => contexto.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == id);
        public Cliente? buscarPorNumId(string numId) => contexto.Clientes.AsNoTracking().FirstOrDefault(c => c.numId == numId);

        //Contexto para acceder a la BD
        public ClienteRepositorio(ClienteDbContext contexto)
        {
            this.contexto = contexto;
        }

        public bool Registrar(Cliente cliente)
        {
            //Retornar false si ya existe
            if (contexto.Clientes.Any(existing => existing.numId == cliente.numId))
            {
                return false;
            }
            contexto.Clientes.Add(cliente);//Añade el cliente al contexto
            contexto.SaveChanges();//Guarda y efectua el cambio en la BD
            return true;
        }

        public void Actualizar(Guid id, Cliente cliente)
        {
            var existe = contexto.Clientes.FirstOrDefault(c => c.Id == id);//Buscar al cliente

            if (existe != null)
            {
                existe.tipoIdentificacion = cliente.tipoIdentificacion;
                existe.numId = cliente.numId;
                existe.nombre = cliente.nombre;
                existe.apellido1 = cliente.apellido1;
                existe.apellido2 = cliente.apellido2;
                existe.fechaNacimiento = cliente.fechaNacimiento;

                contexto.SaveChanges(); //Guardar cambios
            }
        }

        public bool eliminar(Guid id)
        {
            try
            {
                var existe = contexto.Clientes.FirstOrDefault(c => c.Id == id);//Buscar al cliente

                if (existe != null)
                {
                    contexto.Clientes.Remove(existe);//Remueve de la BD
                    contexto.SaveChanges();//Guarda el cambio
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