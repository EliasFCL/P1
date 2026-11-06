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
            if (string.IsNullOrWhiteSpace(busqueda)) return View();
            var proveedores = await _proveedorService.ListarAsync();

            var proveedor = proveedores.Where(p => p.nombre.Equals(busqueda, StringComparison.OrdinalIgnoreCase) || p.RazonSocial.ToString() == busqueda).ToList();
            if (proveedor == null || !proveedor.Any())
            {
                ViewBag.NoEncontrado = "No se encontró ningún proveedor con ese nombre o razón social";
                return View(new List<Proveedor>());
            }
            if (proveedor.Count() > 1)
            {
                ViewBag.NoEncontrado = "Existen 2 o más proveedores con esarazón social";
            }
            else
            {
                ViewBag.Encontrado = "Proveedor encontrado";
            }
            return View(proveedor);
        }

        public ActionResult Create()
        {
            var nuevoGuid = new Proveedor { Id = Guid.NewGuid() };
            return View(nuevoGuid);
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
            return View(new Proveedor() { Id = Guid.NewGuid() });
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var proveedor = await _proveedorService.BuscarPorIdAsync(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, Proveedor proveedor)
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

        public async Task<ActionResult> Delete(Guid id)
        {
            var proveedor = await _proveedorService.BuscarPorIdAsync(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, Proveedor proveedor)
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
