using Microsoft.AspNetCore.Mvc;
using Models;
using Carniceria.API.Modulos;
using Carniceria.API.Validadores;

namespace Carniceria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private static ModuloProveedores _modulo = new ModuloProveedores();
        private readonly ValidarProveedor _validador = new ValidarProveedor();

        private static void AplicarDefaults(Proveedor proveedor)
        {
          //  proveedor.nombre = "ABC";
            proveedor.apellido1 = "ABC";
            proveedor.apellido2 = "ABC";
            proveedor.fechaNacimiento = new DateTime(2004, 3, 25);
        }

        // GET: api/proveedores
        [HttpGet]
        public ActionResult<List<Proveedor>> GetAll() => _modulo.ListarProveedores();

        // GET: api/proveedores/5
        [HttpGet("{id}")]
        public ActionResult<Proveedor> GetById(int id)
        {
            var p = _modulo.ListarProveedores().FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound();
            return p;
        }

        // GET: api/proveedores/buscar/CODIGO_O_RAZONSOCIAL
        [HttpGet("buscar/{busqueda}")]
        public ActionResult<Proveedor> Buscar(string busqueda)
        {
            var p = _modulo.BuscarProveedor(busqueda);
            if (p == null) return NotFound();
            return p;
        }

        // POST: api/proveedores
        [HttpPost]
        public ActionResult<Proveedor> Create(Proveedor proveedor)
        {
            if (proveedor == null) return BadRequest("Proveedor vacío");

            AplicarDefaults(proveedor);

            var errores = _validador.Validar(proveedor);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_modulo.ListarProveedores().Any(p => p.numId == proveedor.numId))
                return BadRequest(new { errores = new List<string> { "Ese ID de proveedor ya existe." } });

            _modulo.RegistrarProveedor(proveedor);
            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Proveedor proveedor)
        {
            if (proveedor == null) return BadRequest();
            proveedor.Id = id;

            AplicarDefaults(proveedor);

            var errores = _validador.Validar(proveedor);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_modulo.ListarProveedores().Any(p => p.numId == proveedor.numId && p.Id != id))
                return BadRequest(new { errores = new List<string> { "Ese ID de proveedor ya existe." } });

            if (!_modulo.ActualizarProveedor(id.ToString(), proveedor)) return NotFound();
            return NoContent();
        }

        // DELETE: api/proveedores/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_modulo.EliminarProveedor(id.ToString())) return NotFound();
            return NoContent();
        }
    }
}