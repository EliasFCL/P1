//Devolver un objeto por medio de un json: https://stackoverflow.com/questions/13100198/mvc-pass-parameter-to-action-using-jquery-ajax y 

using Microsoft.AspNetCore.Mvc;
using Models;
using Proyecto1_EliasCorderoLeon.Services;
using System.Globalization;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<ActionResult> ProductoHome()
        {
            var productos = await _productoService.ListarAsync();
            return View(productos);
        }

        public async Task<ActionResult> Index()
        {
            var productos = await _productoService.ListarAsync();
            return View(productos);
        }

        public async Task<ActionResult> IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var productos = await _productoService.ListarAsync();
            return View(productos);
        }

        public async Task<ActionResult> IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var productos = await _productoService.ListarAsync();
            return View(productos);
        }

        public async Task<ActionResult> Details(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return View();

            var producto = await _productoService.BuscarAsync(busqueda);

            if (producto == null)
            {
                ViewBag.NoEncontrado = "No se encontró un producto con ese código o nombre.";
                return View();
            }

            int contador = 0;
            var productos = await _productoService.ListarAsync();
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
            return View(producto);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Producto producto)
        {
            if (producto == null) return View();

            var (ok, errores) = await _productoService.CrearAsync(producto);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(producto);
            }

            ModelState.Clear();
            ViewBag.MensajeExito = "Producto registrado exitosamente.";
            return View(new Producto());
        }

        public async Task<ActionResult> Edit(int id)
        {
            var producto = await _productoService.BuscarPorIdAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Producto producto)
        {
            producto.Id = id;
            var (ok, errores) = await _productoService.ActualizarAsync(id, producto);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(producto);
            }

            TempData["Actualizado"] = "Producto con id único " + producto.Id + " actualizado";
            return RedirectToAction(nameof(IndexActualizar));
        }

        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _productoService.BuscarPorIdAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Producto producto)
        {
            await _productoService.EliminarAsync(id);
            TempData["Eliminado"] = "Producto eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }

        [HttpPost]
        public async Task<ActionResult> verificarCombinacion(Producto producto)
        {
            var (existe, precio) = await _productoService.VerificarCombinacionAsync(producto);
            return Json(new { existe, precio });
        }
    }
}