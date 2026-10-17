//Cambio de cargos: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-10.0

using Microsoft.AspNetCore.Mvc;

using Proyecto1_EliasCorderoLeon.Areas.Empleados;
using Models;
using Proyecto1_EliasCorderoLeon.Validadores;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class EmpleadoController : Controller
    {
        private static ModuloEmpleado moduloEmpleado = new ModuloEmpleado();
        // GET: EmpleadoController
        public ActionResult Index()
        {
            var empleados = moduloEmpleado.ListarEmpleados();
            return View(empleados);
        }
        public ActionResult IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var empleados = moduloEmpleado.ListarEmpleados();
            return View(empleados);
        }

        public ActionResult IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var empleados = moduloEmpleado.ListarEmpleados();
            return View(empleados);
        }

        public ActionResult EmpleadoHome()
        {
            var empleados = moduloEmpleado.ListarEmpleados();
            return View(empleados);
        }

        // GET: EmpleadoController/Details/5
        public ActionResult Details(string numId)
        {
            if (string.IsNullOrWhiteSpace(numId))
            {
                return View();
            }

            var empleado = moduloEmpleado.BuscarEmpleado(numId);

            if (empleado == null)
            {
                ViewBag.NoEncontrado = "No se encontró un empleado con ese número de identificación.";
                return View();
            }
            else
            {
                ViewBag.Encontrado = "Empleado encontrado";
            }

            return View(empleado);
        }

        // GET: EmpleadoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpleadoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Empleado empleado)
        {
            try
            {
                if (empleado == null)
                {
                    return View();
                }
                //Incrementar Id
                var Id = moduloEmpleado.ListarEmpleados();

                if (Id.Any())
                {
                    empleado.Id = Id.Max(e => e.Id) + 1;
                }
                else
                {
                    empleado.Id = 1;
                }

                if (moduloEmpleado.ListarEmpleados().Any(p => p.numId == empleado.numId))
                {
                    ModelState.AddModelError(string.Empty, "Ese identificador ya existe.");
                    return View(empleado);
                }
                
                EmpleadoValidador validador = new EmpleadoValidador();
                // Llamada al método estático y uso de la lista de errores
                var errores = validador.Validar(empleado);
                if (errores != null && errores.Any())
                {
                    // Añadir errores al ModelState para mostrarlos en la vista
                    foreach (var err in errores)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                    return View(empleado);
                }
                moduloEmpleado.RegistrarEmpleado(empleado);
                ModelState.Clear();
                ViewBag.MensajeExito = "Empleado registrado exitosamente.";
                return View(new Empleado());

            }
            catch
            {
                return View(empleado);
            }

        }

        // GET: ClienteController/Edit/5
        public ActionResult Edit(int id)
        {
            var empleado = moduloEmpleado.ListarEmpleados()
            .FirstOrDefault(e => e.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Empleado empleado)
        {
            try
            {
                var empleadoValidador = new EmpleadoValidador();
                // Mantener el ID original
                empleado.Id = id;

                // Comprobar que el numId no pertenezca a OTRO empleado
                if (moduloEmpleado.ListarEmpleados()
                    .Any(p => p.numId == empleado.numId && p.Id != id))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ese identificador ya existe."
                    );

                    return View(empleado);
                }

                // Validar
                var errores = empleadoValidador.Validar(empleado);

                if (errores != null && errores.Any())
                {
                    foreach (var err in errores)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }

                    return View(empleado);
                }

                // Actualizar usando el ID original
                moduloEmpleado.ActualizarEmpleado(id.ToString(), empleado);
                TempData["Actualizado"] = ViewBag.Actualizado = "Empleado con id único " + empleado.Id + " actualizado";
                return RedirectToAction(nameof(IndexActualizar));
            }
            catch
            {
                return View(empleado);
            }
        }

        // GET: EmpleadoController/Delete/5
        public ActionResult Delete(int id)
        {
            var empleado = moduloEmpleado.ListarEmpleados()
                .FirstOrDefault(e => e.Id == id);
            return View(empleado);
        }

        // POST: EmpleadoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Empleado empleado)
        {
            try
            {
                moduloEmpleado.EliminarEmpleado(id.ToString());
            }
            catch
            {
                return View(empleado);
            }
            TempData["Eliminado"] = ViewBag.Actualizado = "Empleado eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }


    }
}