using Microsoft.AspNetCore.Mvc;
using Models;
using Proyecto1_EliasCorderoLeon.Services;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class FacturaController : Controller
    {
        private readonly IFacturaService _facturaService;
        private readonly IClienteService _clienteService;
        private readonly IEmpleadoService _empleadoService;
        private readonly IProductoService _productoService;

        public FacturaController(
            IFacturaService facturaService,
            IClienteService clienteService,
            IEmpleadoService empleadoService,
            IProductoService productoService)
        {
            _facturaService = facturaService;
            _clienteService = clienteService;
            _empleadoService = empleadoService;
            _productoService = productoService;
        }

        public async Task<ActionResult> Create()
        {
            await CargarCombosAsync();
            return View();
        }

        public async Task<ActionResult> FacturaHome()
        {
            var facturas = await _facturaService.ListarAsync();
            return View(facturas);
        }

        public async Task<ActionResult> Index()
        {
            var facturas = await _facturaService.ListarAsync();
            return View(facturas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Factura factura)
        {
            if (factura == null) return View();

            var (ok, errores, creada) = await _facturaService.CrearAsync(factura);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                await CargarCombosAsync();
                return View(factura);
            }

            ModelState.Clear();
            ViewBag.MensajeExito = "Factura registrada exitosamente. Número: " + creada!.numeroFactura;
            return View(new Factura());
        }

        private async Task CargarCombosAsync()
        {
            var clientes = await _clienteService.ListarAsync();
            var empleados = await _empleadoService.ListarAsync();
            var productos = await _productoService.ListarAsync();

            ViewBag.Clientes = clientes.Select(c => c.nombre).ToList();
            ViewBag.Empleados = empleados.Select(e => e.nombre).ToList();
            ViewBag.Productos = productos;
        }

        public async Task<ActionResult> Details(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio)) return View();

            var facturas = await _facturaService.BuscarAsync(criterio);
            if (facturas == null || !facturas.Any())
            {
                ViewBag.NoEncontrado = "No se encontró ninguna factura con ese número ni con esa identificación de cliente.";
                return View(new List<Factura>());
            }

            ViewBag.Encontrado = $"Se encontraron {facturas.Count} factura(s).";
            return View(facturas);
        }

        public async Task<ActionResult> Delete(string numeroFactura)
        {
            var factura = await _facturaService.BuscarPorNumeroAsync(numeroFactura);
            if (factura == null) return NotFound();
            return View(factura);
        }

        public async Task<ActionResult> IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var facturas = await _facturaService.ListarAsync();
            return View(facturas);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string numeroFactura)
        {
            await _facturaService.EliminarAsync(numeroFactura);
            TempData["Eliminado"] = "Factura eliminada correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }

        public async Task<ActionResult> IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var facturas = await _facturaService.ListarAsync();
            return View(facturas);
        }

        public async Task<ActionResult> Edit(string numeroFactura)
        {
            var factura = await _facturaService.BuscarPorNumeroAsync(numeroFactura);
            if (factura == null) return NotFound();

            if (factura.estado == "Anulada")
            {
                TempData["Error"] = "Una factura anulada no puede modificarse.";
                return RedirectToAction(nameof(IndexActualizar));
            }

            await CargarCombosAsync();
            return View(factura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string numeroFactura, Factura factura)
        {
            if (factura == null) return View();

            var (ok, errores) = await _facturaService.ActualizarAsync(numeroFactura, factura);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                await CargarCombosAsync();
                return View(factura);
            }

            TempData["Actualizado"] = "Factura " + numeroFactura + " actualizada";
            return RedirectToAction(nameof(IndexActualizar));
        }
    }
}