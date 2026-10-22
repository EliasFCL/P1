//Cambio de cargos: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-10.0

using Microsoft.AspNetCore.Mvc;
using Models;
using Carniceria.API.Validadores;
using Carniceria.API.Modulos;

namespace Carniceria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private static ModuloEmpleado _modulo = new ModuloEmpleado();
        private readonly EmpleadoValidador _validador = new EmpleadoValidador();

        [HttpGet]
        public ActionResult<List<Empleado>> GetAll() => _modulo.ListarEmpleados();

        [HttpGet("{id}")]
        public ActionResult<Empleado> GetById(int id)
        {
            var e = _modulo.ListarEmpleados().FirstOrDefault(x => x.Id == id);
            if (e == null) return NotFound();
            return e;
        }

        [HttpGet("porNumId/{numId}")]
        public ActionResult<Empleado> GetByNumId(string numId)
        {
            var e = _modulo.BuscarEmpleado(numId);
            if (e == null) return NotFound();
            return e;
        }

        [HttpPost]
        public ActionResult<Empleado> Create(Empleado empleado)
        {
            if (empleado == null) return BadRequest("Empleado vacío");

            if (_modulo.ListarEmpleados().Any(e => e.numId == empleado.numId))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            var errores = _validador.Validar(empleado);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            _modulo.RegistrarEmpleado(empleado);
            return CreatedAtAction(nameof(GetById), new { id = empleado.Id }, empleado);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Empleado empleado)
        {
            if (empleado == null) return BadRequest();
            empleado.Id = id;

            var errores = _validador.Validar(empleado);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_modulo.ListarEmpleados().Any(e => e.numId == empleado.numId && e.Id != id))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            if (!_modulo.ActualizarEmpleado(id.ToString(), empleado)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var empleado = _modulo.ListarEmpleados().FirstOrDefault(e => e.Id == id);
            if (empleado == null) return NotFound();

            //No se puede eliminar un empleado con una o más facturas.
            var moduloFactura = new ModuloFactura();
            //Usar string.Equals para evitar NullReferenceException
            bool tieneFacturas = moduloFactura.ListarFacturas()
                .Any(f => string.Equals(f.empleadoResponsable, empleado.nombre, StringComparison.OrdinalIgnoreCase));

            if (tieneFacturas)
            {
                return BadRequest(new { errores = new List<string> { "No se puede eliminar el empleado porque tiene facturas asociadas" } });
            }

            _modulo.EliminarEmpleado(id.ToString());
            return NoContent();
        }
    }
}