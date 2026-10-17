using Microsoft.AspNetCore.Mvc;
using Proyecto1_EliasCorderoLeon.Areas.Proveedores;
using Models;
using Proyecto1_EliasCorderoLeon.Validadores;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class ProveedorController : Controller
    {
        private static ModuloProveedores moduloProveedor = new ModuloProveedores();
        // GET: ProveedorController
        public ActionResult Index()
        {
            var proveedores = moduloProveedor.ListarProveedores();
            return View(proveedores);
        }

        public ActionResult ProveedorHome()
        {
            var proveedores = moduloProveedor.ListarProveedores();
            return View(proveedores);
        }

        public ActionResult IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var proveedores = moduloProveedor.ListarProveedores();
            return View(proveedores);
        }

        public ActionResult IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var proveedores = moduloProveedor.ListarProveedores();
            return View(proveedores);
        }

        // GET: ProveedorController/Details/5
        public ActionResult Details(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return View();
            }
            var proveedor = moduloProveedor.BuscarProveedor(busqueda);

            if (proveedor == null)
            {
                ViewBag.NoEncontrado = "No se encontró un proveedor con ese código o razón social.";
                return View();
            }
            else
            {
                int contador = 0;
                var proveedores = moduloProveedor.ListarProveedores();
                for (int i = 0; i < proveedores.Count; i++)
                {
                    if (proveedores[i].RazonSocial.ToString() == busqueda)
                    {
                        contador = contador + 1;
                    }
                }
                if (contador > 1)
                {
                    ViewBag.NoEncontrado = "Existen 2 o más proveedores con asa razón social";
                }
                else
                {
                    ViewBag.Encontrado = "Proveedor encontrado";
                }
            }
            return View(proveedor);
        } 

        // GET: ProveedorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProveedorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proveedor proveedor)
        {
            //Datos por defecto para clase Persona
            proveedor.apellido1 = "ABC";
            proveedor.apellido2 = "ABC";
            proveedor.fechaNacimiento = new DateTime(2004, 3, 25);
            ValidarProveedor validador = new ValidarProveedor();
            List<string> errores = validador.Validar(proveedor);
            var Id = moduloProveedor.ListarProveedores();

            if (errores.Any())
            {
                foreach (var error in errores)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(proveedor);
            }
            try
            {   
                //Incrementar Id
                if (Id.Any())
                {
                    proveedor.Id = Id.Max(p => p.Id) + 1;
                }
                else
                {
                    proveedor.Id = 1;
                }

                if (moduloProveedor.ListarProveedores().Any(p => p.numId == proveedor.numId))
                {
                    ModelState.AddModelError(string.Empty, "Ese ID de proveedor ya existe.");
                    return View(proveedor);
                }
                

            }
            catch (Exception e)
            {
                return View();
            }
            moduloProveedor.RegistrarProveedor(proveedor);
            ModelState.Clear();
            ViewBag.MensajeExito = "Proveedor registrado exitosamente.";
            return View(new Proveedor());
        }

        // GET: ProveedorController/Edit/5
        public ActionResult Edit(int id)
        {
            var proveedor = moduloProveedor.ListarProveedores()
                .FirstOrDefault(p => p.Id == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: ProveedorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Proveedor proveedor)
        {
            try
            {
                //Datos por defecto para clase Persona
                proveedor.apellido1 = "ABC";
                proveedor.apellido2 = "ABC";
                proveedor.fechaNacimiento = new DateTime(2004, 3, 25);
                var validador = new ValidarProveedor();
                // Mantener el ID original
                proveedor.Id = id;

                // Comprobar que el codigo no pertenezca a otro productor
                if (moduloProveedor.ListarProveedores().Any(p => p.numId == proveedor.numId && p.Id != id))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ese ID de proveedor ya existe."
                    );

                    return View(proveedor);
                }

                // Validar
                var errores = validador.Validar(proveedor);

                if (errores != null && errores.Any())
                {
                    foreach (var err in errores)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }

                    return View(proveedor);
                }

                // Actualizar usando el ID original
                TempData["Actualizado"] = ViewBag.Actualizado = "Proveedor con id único " + proveedor.Id + " actualizado";
                moduloProveedor.ActualizarProveedor(id.ToString(), proveedor);
                return RedirectToAction(nameof(IndexActualizar));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProveedorController/Delete/5
        public ActionResult Delete(int id)
        {
            var proveedor = moduloProveedor.ListarProveedores().FirstOrDefault(e => e.Id == id);
            return View(proveedor);
        }

        // POST: ProveedorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Proveedor proveedor)
        {
            try
            {
                TempData["Eliminado"] = ViewBag.Actualizado = "Proveedor eliminado correctamente";
                moduloProveedor.EliminarProveedor(id.ToString());
                return RedirectToAction(nameof(IndexEliminar));
            }
            catch
            {
                return View();
            }
        }
    }
}
