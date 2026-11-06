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
        private readonly IProveedorService _proveedorService;

        public ProductoController(IProductoService productoService, IProveedorService proveedorService)
        {
            _productoService = productoService;
            _proveedorService = proveedorService;
        }

        public async Task<ActionResult> ProductoHome()
        {
            var productos = await _productoService.ListarAsync();
            return View(productos);
        }

        private async Task CargarCombosAsync()
        {
            var proveedor = await _proveedorService.ListarAsync();

            ViewBag.Proveedores = proveedor.Select(p => p.nombre).ToList();
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
            if (string.IsNullOrWhiteSpace(busqueda)) return View();
            var productos = await _productoService.ListarAsync();

            var producto = productos.Where(p => p.nombre.Equals(busqueda, StringComparison.OrdinalIgnoreCase)|| p.codigo.ToString() == busqueda).ToList();
            if (producto == null || !producto.Any())
            {
                ViewBag.NoEncontrado = "No se encontró ningún producto con ese código o nombre";
                return View(new List<Producto>());
            }
            if (producto.Count() > 1)
            {
                ViewBag.NoEncontrado = "Existen 2 o más productos con ese nombre";
            }
            else
            {
                ViewBag.Encontrado = "Producto encontrado";
            }
            return View(producto);
        }
        public async Task<ActionResult> Create()
        {
            var nuevoGuid = new Producto { Id = Guid.NewGuid() };
            ViewBag.Proveedores = await _proveedorService.ListarNombresAsync();
            
            return View(nuevoGuid);
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
                ViewBag.Proveedores = await _proveedorService.ListarNombresAsync();
                await CargarCombosAsync();
                return View(producto);
            }


            ModelState.Clear();
            if (producto.existenciaActual <= producto.existeciaMinima)
            {
                ViewBag.Aviso = "Producto registrado: Existe un faltante de productos para el pedido correspondiente";
            }else { 
                ViewBag.MensajeExito = "Producto registrado exitosamente";
            }
            await CargarCombosAsync();
            return View(new Producto() { Id = Guid.NewGuid() });
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var producto = await _productoService.BuscarPorIdAsync(id);
            ViewBag.Proveedores = await _proveedorService.ListarNombresAsync();
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, Producto producto)
        {
            producto.Id = id;
            var (ok, errores) = await _productoService.ActualizarAsync(id, producto);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                ViewBag.Proveedores = await _proveedorService.ListarNombresAsync();
                return View(producto);
            }

            TempData["Actualizado"] = "Producto con id único " + producto.Id + " actualizado";
            return RedirectToAction(nameof(IndexActualizar));
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var producto = await _productoService.BuscarPorIdAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, Producto producto)
        {
            var (ok, errores) = await _productoService.EliminarAsync(id);

            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                var productoCompleto = await _productoService.BuscarPorIdAsync(id);
                return View(productoCompleto ?? producto);
            }

            TempData["Eliminado"] = "Producto eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }


        //Recibir la peticion desde el MVC y devolver el resultado
        [HttpPost]
        public async Task<ActionResult> verificarCombinacion(Producto producto)
        {
            var (existe, precio) = await _productoService.VerificarCombinacionAsync(producto);
            return Json(new { existe, precio });
        }
    }
}