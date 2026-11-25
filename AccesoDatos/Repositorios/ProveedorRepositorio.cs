using AccesoDatos.Contexto;
using Microsoft.EntityFrameworkCore;
using Models;
namespace AccesoDatos.Repositorios
{
    public class ProveedorRepositorio
    {
        private readonly ProveedorDbContext contexto;
        //Obtener datos
        public IReadOnlyCollection<Proveedor> Listar => contexto.Proveedores.AsNoTracking().OrderBy(p => p.nombre).ToList();
        public Proveedor? buscarPorId(Guid id) => contexto.Proveedores.AsNoTracking().FirstOrDefault(p => p.Id == id);
        public Proveedor? buscarPorNumId(string numId) => contexto.Proveedores.AsNoTracking().FirstOrDefault(p => p.numId == numId);
        public ProveedorRepositorio(ProveedorDbContext contexto)
        {
            this.contexto = contexto;
        }

        public bool Registrar(Proveedor proveedor)
        {
            if(contexto.Proveedores.Any(existing => existing.numId == proveedor.numId))
            {
                return false;
            }
            contexto.Proveedores.Add(proveedor);
            contexto.SaveChanges();
            return true;
        }

        public bool Eliminar(Guid id)
        {
            try {
                var existe = contexto.Proveedores.FirstOrDefault(p => p.Id == id);
                if (existe != null)
                {
                    contexto.Proveedores.Remove(existe);
                    contexto.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool Actualizar(Guid id, Proveedor proveedor)
        {
            var existe = contexto.Proveedores.FirstOrDefault(p => p.Id == id);
            if(existe != null)
            {
                existe.tipoIdentificacion = proveedor.tipoIdentificacion;
                existe.numId = proveedor.numId;
                existe.nombre = proveedor.nombre;
                existe.apellido1 = proveedor.apellido1;
                existe.apellido2 = proveedor.apellido2;
                existe.fechaNacimiento = proveedor.fechaNacimiento;
                existe.RazonSocial = proveedor.RazonSocial;
                existe.Telefono = proveedor.Telefono;
                existe.TipoSuministro = proveedor.TipoSuministro;
                existe.CorreoElectronico = proveedor.CorreoElectronico;
                existe.FrecuenciaEntrega = proveedor.FrecuenciaEntrega;
                contexto.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
