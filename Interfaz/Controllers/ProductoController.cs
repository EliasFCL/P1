//Devolver un objeto por medio de un json: https://stackoverflow.com/questions/13100198/mvc-pass-parameter-to-action-using-jquery-ajax y 

using Microsoft.AspNetCore.Mvc;
using Proyecto1_EliasCorderoLeon.Areas.Productos;
using Models;
using Proyecto1_EliasCorderoLeon.Validadores;
using System.Globalization;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class ProductoController : Controller
    {
        private static ModuloProducto moduloProducto = new ModuloProducto();
        // GET: ProductoController
        public ActionResult Index()
        {
            var productos = moduloProducto.ListarProductos();
            return View(productos);
        }

        public ActionResult ProductoHome()
        {
            var productos = moduloProducto.ListarProductos();
            return View(productos);
        }

        public ActionResult IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var productos = moduloProducto.ListarProductos();
            return View(productos);
        }

        public ActionResult IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var productos = moduloProducto.ListarProductos();
            return View(productos);
        }

        // GET: ProductoController/Details/5
        public ActionResult Details(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return View();
            }
            var producto = moduloProducto.BuscarProducto(busqueda);

            if (producto == null)
            {
                ViewBag.NoEncontrado = "No se encontró un producto con ese código o nombre.";
                return View();
            } else
            {
                int contador = 0;
                var productos = moduloProducto.ListarProductos();
                for (int i = 0; i < productos.Count; i++)
                {
                    if (productos[i].nombre.ToString() == busqueda)
                    {
                        contador = contador + 1;
                    }
                }
                if (contador > 1)
                {
                    ViewBag.NoEncontrado = "Existen 2 o más productos con ese nombre";
                }
                else
                {
                    ViewBag.Encontrado = "Producto encontrado";
                }
            }

            return View(producto);
        }
        // GET: ProductoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Producto producto)
        {
            validarProducto validador = new validarProducto();
            List<string> errores = validador.Validar(producto);
            
            var Id = moduloProducto.ListarProductos();

            if (errores.Any())
            {
                foreach (var error in errores)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View();
            }

            try
            {
                //Incrementar Id
                if (Id.Any())
                {
                    producto.Id = Id.Max(p => p.Id) + 1;
                }
                else
                {
                    producto.Id = 1;
                }


                if (moduloProducto.ListarProductos().Any(p => p.codigo == producto.codigo))
                {
                    ModelState.AddModelError(string.Empty, "Ese código de producto ya existe.");
                    return View(producto);
                }

                moduloProducto.RegistrarProducto(producto);
                ModelState.Clear();
                ViewBag.MensajeExito = "Producto registrado exitosamente";
                return View(new Producto());
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductoController/Edit/5
        public ActionResult Edit(int id)
        {
            var producto = moduloProducto.ListarProductos()
                .FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: ProductoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Producto producto, string precio)
        {
            try
            {
                var productoValidador = new validarProducto();
                // Mantener el ID original
                producto.Id = id;

                // Comprobar que el codigo no pertenezca a OTRO producto
                if (moduloProducto.ListarProductos() .Any(p => p.codigo == producto.codigo && p.Id != id))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ese código de producto ya existe."
                    );

                    return View(producto);
                }

                // Validar
                var errores = productoValidador.Validar(producto);

                if (errores != null && errores.Any())
                {
                    foreach (var err in errores)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }

                    return View(producto);
                }

                TempData["Actualizado"] = ViewBag.Actualizado = "Producto con id único " + producto.Id + " actualizado";
                //Cambiar coma por punto para poder convertir a double y evitar el borrado del punto
                precio = precio.Replace(',', '.');
                producto.precio = double.Parse(precio, CultureInfo.InvariantCulture);
                // Actualizar usando el ID original
                moduloProducto.ActualizarProducto(id.ToString(), producto);

                return RedirectToAction(nameof(IndexActualizar));
            }
            catch
            {
                return View(producto);
            }
        }

        // GET: ProductoController/Delete/5
        public ActionResult Delete(int id)
        {
            var producto = moduloProducto.ListarProductos()
                .FirstOrDefault(e => e.Id == id);
            return View(producto);
        }

        // POST: ProductoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Producto producto)
        {
            try
            {
                TempData["Eliminado"] = ViewBag.Actualizado = "Producto eliminado correctamente";
                moduloProducto.EliminarProducto(id.ToString());
                return RedirectToAction(nameof(IndexEliminar));
            }
            catch
            {
                return View();
            }
        }
        //Devolver el precio de una combinacón de unidad de medida y categoría
        [HttpPost]
        public ActionResult verificarCombinacion(Producto producto)
        {
            var productos = moduloProducto.ListarProductos();

            bool existeCombinacion = productos.Any(p =>
                p.unidadMedida == producto.unidadMedida &&
                p.categoria == producto.categoria);
             
            return Json(new { existe = existeCombinacion, precio = productos.FirstOrDefault(p => p.unidadMedida == producto.unidadMedida 
            && p.categoria == producto.categoria)?.precio });
        }
    }
}
