using Microsoft.AspNetCore.Mvc;
using Models;
using Proyecto1_EliasCorderoLeon.Services;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public async Task<ActionResult> ClienteHome()
        {
            var clientes = await _clienteService.ListarAsync();
            return View(clientes);
        }

        public async Task<ActionResult> Index()
        {
            var clientes = await _clienteService.ListarAsync();
            return View(clientes);
        }

        public async Task<ActionResult> IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var clientes = await _clienteService.ListarAsync();
            return View(clientes);
        }

        public async Task<ActionResult> IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var clientes = await _clienteService.ListarAsync();
            return View(clientes);
        }

        public async Task<ActionResult> Details(string numId)
        {
            if (string.IsNullOrWhiteSpace(numId)) return View();

            var cliente = await _clienteService.BuscarPorNumIdAsync(numId);
            if (cliente == null)
            {
                ViewBag.NoEncontrado = "No se encontró un cliente con ese número de identificación.";
                return View();
            }
            ViewBag.Encontrado = "Cliente encontrado";
            return View(cliente);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Cliente cliente)
        {
            if (cliente == null) return View();

            var (ok, errores) = await _clienteService.CrearAsync(cliente);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(cliente);
            }

            ModelState.Clear();
            ViewBag.MensajeExito = "Cliente registrado exitosamente.";
            return View(new Cliente());
        }

        public async Task<ActionResult> Edit(int id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Cliente cliente)
        {
            cliente.Id = id;
            var (ok, errores) = await _clienteService.ActualizarAsync(id, cliente);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(cliente);
            }

            TempData["Actualizado"] = "Cliente con id único " + cliente.Id + " actualizado";
            return RedirectToAction(nameof(IndexActualizar));
        }

        public async Task<ActionResult> Delete(int id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Cliente cliente)
        {
            await _clienteService.EliminarAsync(id);
            TempData["Eliminado"] = "Cliente eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }
    }
}