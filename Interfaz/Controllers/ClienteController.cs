// ViewBag: https://learn.microsoft.com/es-mx/aspnet/core/mvc/views/overview?view=aspnetcore-9.0#2
// Crear un controller: Guía para crear un proyecto básico de MVC (Alojado en Aprende U): 6:26

using Microsoft.AspNetCore.Mvc;
using Proyecto1_EliasCorderoLeon.Areas.Clientes;
using Models;
using Proyecto1_EliasCorderoLeon.Validadores;

namespace Proyecto1_EliasCorderoLeon.Controllers
{
    public class ClienteController : Controller
    {
        private static ModuloCliente moduloCliente = new ModuloCliente();
        ClienteValidador clienteValidador = new ClienteValidador();
        // GET: ClienteController lleva al home de clientes
        public ActionResult ClienteHome()
        {
            var clientes = moduloCliente.ListarClientes();
            return View(clientes);
        }

        public ActionResult IndexActualizar()
        {
            ViewBag.Actualizado = TempData["Actualizado"];
            var clientes = moduloCliente.ListarClientes();
            return View(clientes);
        }

        public ActionResult IndexEliminar()
        {
            ViewBag.Eliminado = TempData["Eliminado"];
            var clientes = moduloCliente.ListarClientes();
            return View(clientes);
        }

        public ActionResult Index()
        {
            var clientes = moduloCliente.ListarClientes();
            return View(clientes);
        }

        // GET: ClienteController, se encarga de buscar al cliente por su idetificación
        public ActionResult Details(string numId)
        {
            if (string.IsNullOrWhiteSpace(numId))
            {
                return View();
            }

            var cliente = moduloCliente.BuscarCliente(numId);

            if (cliente == null)
            {
                ViewBag.NoEncontrado = "No se encontró un cliente con ese número de identificación.";
                return View();
            }
            else
            {
                ViewBag.Encontrado = "Cliente encontrado";
            }

            return View(cliente);
        }

        // GET: ClienteController/Create        
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            try
            {

                if (cliente == null)
                {
                    return View();
                }
                //Incrementar Id
                var Id = moduloCliente.ListarClientes();

                if (Id.Any())
                {
                    cliente.Id = Id.Max(c => c.Id) + 1;
                }
                else
                {
                    cliente.Id = 1;
                }

                if (moduloCliente.ListarClientes().Any(p => p.numId == cliente.numId))
                {
                    ModelState.AddModelError(string.Empty, "Ese identificador ya existe.");
                    return View(cliente);
                }

                // Llamada al método estático y uso de la lista de errores
                var errores = clienteValidador.Validar(cliente);


                if (errores != null && errores.Any())
                {
                    // Añadir errores al ModelState para mostrarlos en la vista
                    foreach (var err in errores)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                    return View(cliente); // Evita que el cliente se guarde si hay errores de validación
                }

                moduloCliente.RegistrarCliente(cliente);
                ModelState.Clear();
                ViewBag.MensajeExito = "Cliente registrado exitosamente.";
                return View(new Cliente());
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteController/Edit/5
        public ActionResult Edit(int id)
        {
            var cliente = moduloCliente.ListarClientes()
                .FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cliente cliente)
        {
            try
            {
                var clienteValidador = new ClienteValidador();
                // Mantener el ID original
                cliente.Id = id;

                // Validar
                var errores = clienteValidador.Validar(cliente);

                if (errores != null && errores.Any())
                {
                    foreach (var err in errores)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }

                    return View(cliente);
                }
                // Comprobar que el numId no pertenezca a OTRO empleado
                if (moduloCliente.ListarClientes()
                    .Any(p => p.numId == cliente.numId && p.Id != id))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ese identificador ya existe."
                    );

                    return View(cliente);
                }

                // Actualizar usando el ID original
                moduloCliente.ActualizarCliente(id.ToString(), cliente);
                TempData["Actualizado"] = ViewBag.Actualizado = "Cliente con id único " + cliente.Id + " actualizado";

                return RedirectToAction(nameof(IndexActualizar));
            }
            catch
            {
                return View(cliente);
            }
        }
        // GET: ClienteController/Delete/5
        public ActionResult Delete(int id)
        {
            var cliente = moduloCliente.ListarClientes()
                .FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);

        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Cliente cliente)
        {
            try
            {
                moduloCliente.EliminarCliente(id.ToString());
            }
            catch
            {
                return View(cliente);
            }
            TempData["Eliminado"] = ViewBag.Actualizado = "Cliente eliminado correctamente";
            return RedirectToAction(nameof(IndexEliminar));
        }
    }
}