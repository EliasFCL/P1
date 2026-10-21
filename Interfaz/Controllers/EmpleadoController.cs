//Cambio de cargos: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-10.0

using Microsoft.AspNetCore.Mvc;
using Models;
using Proyecto1_EliasCorderoLeon.Services;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadoController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        public async Task<ActionResult> EmpleadoHome()
        {
            var empleados = await _empleadoService.ListarAsync();
            return View(empleados);
        }

        public async Task<ActionResult> Index()
        {
            var empleados = await _empleadoService.ListarAsync();
            return View(empleados);
        }

        public async Task<ActionResult> IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var empleados = await _empleadoService.ListarAsync();
            return View(empleados);
        }

        public async Task<ActionResult> IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var empleados = await _empleadoService.ListarAsync();
            return View(empleados);
        }

        public async Task<ActionResult> Details(string numId)
        {
            if (string.IsNullOrWhiteSpace(numId)) return View();

            var empleado = await _empleadoService.BuscarPorNumIdAsync(numId);
            if (empleado == null)
            {
                ViewBag.NoEncontrado = "No se encontró un empleado con ese número de identificación.";
                return View();
            }
            ViewBag.Encontrado = "Empleado encontrado";
            return View(empleado);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Empleado empleado)
        {
            if (empleado == null) return View();

            var (ok, errores) = await _empleadoService.CrearAsync(empleado);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(empleado);
            }

            ModelState.Clear();
            ViewBag.MensajeExito = "Empleado registrado exitosamente.";
            return View(new Empleado());
        }

        public async Task<ActionResult> Edit(int id)
        {
            var empleado = await _empleadoService.BuscarPorIdAsync(id);
            if (empleado == null) return NotFound();
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Empleado empleado)
        {
            empleado.Id = id;
            var (ok, errores) = await _empleadoService.ActualizarAsync(id, empleado);
            if (!ok)
            {
                foreach (var err in errores) ModelState.AddModelError(string.Empty, err);
                return View(empleado);
            }

            TempData["Actualizado"] = "Empleado con id único " + empleado.Id + " actualizado";
            return RedirectToAction(nameof(IndexActualizar));
        }

        public async Task<ActionResult> Delete(int id)
        {
            var empleado = await _empleadoService.BuscarPorIdAsync(id);
            if (empleado == null) return NotFound();
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Empleado empleado)
        {
            await _empleadoService.EliminarAsync(id);
            TempData["Eliminado"] = "Empleado eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }
    }
}