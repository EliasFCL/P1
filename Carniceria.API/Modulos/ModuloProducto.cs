using Models;

namespace Carniceria.API.Modulos
{
    public class ModuloProducto
    {
        private static List<Producto> listaProductos = new List<Producto>
    {

    };
        public void RegistrarProducto(Producto producto)
        {
            if (producto.Id == Guid.Empty)
            {
                producto.Id = Guid.NewGuid();
            }
            listaProductos.Add(producto);
        }

        public Producto BuscarProducto(string nombre)
        {
            var producto = ListarProductos()
                .FirstOrDefault(p => p.codigo.ToString() == nombre || p.nombre.ToString() == nombre);

            return producto;
        }

        public List<Producto> ListarProductos()
        {
            return listaProductos;
        }

        public bool ActualizarProducto(string Id, Producto productoActualizado)
        {
            var producto = listaProductos.FirstOrDefault(p => p.Id.ToString() == Id.ToString());

            if (producto != null)
            {
                producto.codigo = productoActualizado.codigo;
                producto.nombre = productoActualizado.nombre;
                producto.categoria = productoActualizado.categoria;
                producto.unidadMedida = productoActualizado.unidadMedida;
                producto.precio = productoActualizado.precio;
                producto.existenciaActual = productoActualizado.existenciaActual;
                producto.existeciaMinima = productoActualizado.existeciaMinima;
                producto.proveedorId = productoActualizado.proveedorId;
            }
            return true;
        }

        public bool EliminarProducto(string Id)
        {
            var producto = listaProductos.FirstOrDefault(p => p.Id.ToString() == Id.ToString());

            if (producto != null)
            {
                listaProductos.Remove(producto);

                return true;
            }

            return false;
        }
    }
}