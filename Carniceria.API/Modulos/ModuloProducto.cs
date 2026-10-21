using Models;

namespace Carniceria.API.Modulos
{
    public class ModuloProducto
    {
        private List<Producto> listaProductos = new List<Producto>();
        public ModuloProducto()
        {
            listaProductos.Add(new Producto
            {
                Id = 1,
                codigo = "P001",
                nombre = "Lomo de vaca",
                categoria = "Carne de Res",
                unidadMedida = "Unidad",
                precio = 3000.4,
                existenciaActual = 100,
                existeciaMinima = 20
            });
        }
        public void RegistrarProducto(Producto producto)
        {
            producto.Id = listaProductos.Any() ? listaProductos.Max(p => p.Id) + 1 : 1;
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