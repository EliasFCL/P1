//LinkQ: https://learn.microsoft.com/es-mx/ef/core/querying/complex-query-operators
using Models;
using AccesoDatos.Contexto;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositorios
{
    public class ProductoRepositorio
    {
        private readonly ProductoDbContext contexto;

        //Obtener datos, Listar con LinkQ
        public IReadOnlyCollection<Producto> Listar => (from p in contexto.Productos.AsNoTracking()
            join prov in contexto.Proveedores.AsNoTracking() on p.proveedorId equals prov.Id into proveedores
            from prov in proveedores.DefaultIfEmpty() //Hacer JOIN
            orderby p.nombre
        select new Producto
        {
            Id = p.Id,
            codigo = p.codigo,
            nombre = p.nombre,
            categoria = p.categoria,
            unidadMedida = p.unidadMedida,
            precio = p.precio,
            existenciaActual = p.existenciaActual,
            existenciaMinima = p.existenciaMinima,
            proveedorId = p.proveedorId,
            nombreProveedor = prov != null ? prov.nombre : "Sin Proveedor"
        }).ToList();
        public Producto? buscarPorId(Guid id) => contexto.Productos.AsNoTracking().FirstOrDefault(p => p.Id == id);
        public Producto? buscarPorCodigo(string codigo) => contexto.Productos.AsNoTracking().FirstOrDefault(p => p.codigo == codigo);

        public ProductoRepositorio(ProductoDbContext contexto)
        {
            this.contexto = contexto;
        }

        public bool Registrar(Producto producto)
        {
            if(contexto.Productos.Any(existing => existing.codigo == producto.codigo))
            {
                return false;
            }
            contexto.Productos.Add(producto);
            contexto.SaveChanges();
            return true;
        }

        public bool Actualizar(Producto producto)
        {
            var existe = contexto.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if(existe != null)
            {
                existe.codigo = producto.codigo;
                existe.nombre = producto.nombre;
                existe.categoria = producto.categoria;
                existe.unidadMedida = producto.unidadMedida;
                existe.precio = producto.precio;
                existe.existenciaActual = producto.existenciaActual;
                existe.existenciaMinima = producto.existenciaMinima;
                existe.proveedorId = producto.proveedorId;
                contexto.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Eliminar(Guid id)
        {
            var existe = contexto.Productos.FirstOrDefault(p => p.Id == id);
            if (existe != null)
            {
                contexto.Productos.Remove(existe);
                contexto.SaveChanges();
                return true;
            }
            return false;
        }

        public decimal? ObtenerPrecioPorCombinacion(string categoria, string unidadMedida)
        {
            var producto = contexto.Productos.AsNoTracking().FirstOrDefault(p => p.categoria == categoria && p.unidadMedida == unidadMedida);
            return producto?.precio;
        }
    }
}