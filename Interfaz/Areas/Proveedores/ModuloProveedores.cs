using Models;

namespace Proyecto1_EliasCorderoLeon.Areas.Proveedores
{
    public class ModuloProveedores
    {
        private List<Proveedor> listaProveedores = new List<Proveedor>();

        public void RegistrarProveedor(Proveedor proveedor)
        {
            listaProveedores.Add(proveedor);
        }

        public Proveedor BuscarProveedor(string nombre)
        {
            var proveedor = ListarProveedores()
                .FirstOrDefault(p => p.numId.ToString() == nombre || p.RazonSocial.ToString() == nombre);

            return proveedor;
        }

        public List<Proveedor> ListarProveedores()
        {
            return listaProveedores;
        }

        public bool ActualizarProveedor(string Id, Proveedor proveedorActualizado)
        {
            var proveedor = listaProveedores.FirstOrDefault(p => p.Id.ToString() == Id.ToString());

            if (proveedor != null)
            {
                proveedor.tipoIdentificacion = proveedorActualizado.tipoIdentificacion;
                proveedor.numId = proveedorActualizado.numId;
                proveedor.RazonSocial = proveedorActualizado.RazonSocial;
                proveedor.nombre = proveedorActualizado.nombre;
                proveedor.Telefono = proveedorActualizado.Telefono;
                proveedor.CorreoElectronico = proveedorActualizado.CorreoElectronico;
                proveedor.TipoSuministro = proveedorActualizado.TipoSuministro;
                proveedor.FrecuenciaEntrega = proveedorActualizado.FrecuenciaEntrega;

                return true;
            }

            return false;
        }

        public bool EliminarProveedor(string Id)
        {
            var proveedor = listaProveedores.FirstOrDefault(p => p.Id.ToString() == Id.ToString());

            if (proveedor != null)
            {
                listaProveedores.Remove(proveedor);

                return true;
            }

            return false;
        }
    }
}
