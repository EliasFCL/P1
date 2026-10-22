using Microsoft.AspNetCore.Mvc;
using Models;
using Proyecto1_EliasCorderoLeon.Services;
using System.Globalization;
using static System.Net.WebRequestMethods;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        public async Task<ActionResult> ProveedorHome()
        {
            var proveedores = await _proveedorService.ListarAsync();
            return View(proveedores);
        }

        public async Task<ActionResult> Index()
        {
            var proveedores = await _proveedorService.ListarAsync();
            return View(proveedores);
        }

        public async Task<ActionResult> IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var proveedores = await _proveedorService.ListarAsync();
            return View(proveedores);
        }

        public async Task<ActionResult> IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var proveedores = await _proveedorService.ListarAsync();
            return View(proveedores);
        }

        public async Task<ActionResult> Details(string busqueda)
        {
            //Agregar lo de 2 o mas proveedores
            if (string.IsNullOrWhiteSpace(busqueda)) return View();
            var proveedor = await _proveedorService.BuscarAsync(busqueda);
            if (proveedor == null)
            {
                ViewBag.NoEncontrado = "No se encontró un proveedor con ese número de identificación.";
                return View();
            }
            int contador = 0;
            var proveedores = await _proveedorService.ListarAsync();
            for (int i = 0; i < proveedores.Count; i++)
            {
                if (proveedores[i].RazonSocial.ToString() == busqueda)
                {
                    contador = contador + 1;
                }
            }

            if (contador > 1)
            {
                ViewBag.NoEncontrado = "Existen 2 o más proveedores con esa razón social";
            }
            else
            {
                ViewBag.Encontrado = "Proveedor encontrado";
            }

            return View(proveedor);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Proveedor proveedor)
        {
            if (proveedor == null) return View();

            var (ok, errores) = await _proveedorService.CrearAsync(proveedor);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(proveedor);
            }

            ModelState.Clear();
            ViewBag.MensajeExito = "Proveedor registrado exitosamente.";
            return View(new Proveedor());
        }

        public async Task<ActionResult> Edit(int id)
        {
            var proveedor = await _proveedorService.BuscarPorIdAsync(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Proveedor proveedor)
        {
            proveedor.Id = id;
            var (ok, errores) = await _proveedorService.ActualizarAsync(id, proveedor);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(proveedor);
            }

            TempData["Actualizado"] = "Proveedor con id único " + proveedor.Id + " actualizado";
            return RedirectToAction(nameof(IndexActualizar));
        }

        public async Task<ActionResult> Delete(int id)
        {
            var proveedor = await _proveedorService.BuscarPorIdAsync(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Proveedor proveedor)
        {
            var (ok, errores) = await _proveedorService.EliminarAsync(id);

            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                var proveedorCompleto = await _proveedorService.BuscarPorIdAsync(id);
                return View(proveedorCompleto ?? proveedor);
            }

            TempData["Eliminado"] = "Proveedor eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }
    }
}
