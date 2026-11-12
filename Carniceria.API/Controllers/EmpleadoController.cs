//Cambio de cargos: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-10.0

using Microsoft.AspNetCore.Mvc;
using Models;
using Carniceria.API.Validadores;
using Carniceria.API.Modulos;
using AccesoDatos.Repositorios;

namespace Carniceria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private static ModuloEmpleado _modulo = new ModuloEmpleado();
        private readonly EmpleadoRepositorio _empleadoRepositorio;
        private readonly EmpleadoValidador _validador = new EmpleadoValidador();

        public EmpleadosController(EmpleadoRepositorio empleadoRepositorio)
        {
            _empleadoRepositorio = empleadoRepositorio;
        }

        [HttpGet]
        public ActionResult<List<Empleado>> GetAll() => _empleadoRepositorio.Listar();

        [HttpGet("{id}")]
        public ActionResult<Empleado> GetById(Guid id)
        {
            var e = _empleadoRepositorio.BuscarPorId(id);
            if (e == null) return NotFound();
            return e;
        }

        [HttpGet("porNumId/{numId}")]
        public ActionResult<Empleado> GetByNumId(string numId)
        {
            var empleado = _empleadoRepositorio.BuscarPorNumId(numId);
            if (empleado == null) return NotFound();
            return empleado;
        }

        [HttpPost]
        public ActionResult<Empleado> Create(Empleado empleado)
        {
            if (empleado == null) return BadRequest("Empleado vacío");

            if (_empleadoRepositorio.Listar().Any(e => e.numId == empleado.numId))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            var errores = _validador.Validar(empleado);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            _empleadoRepositorio.Registrar(empleado);
            return CreatedAtAction(nameof(GetById), new { id = empleado.Id }, empleado);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Empleado empleado)
        {
            if (empleado == null) return BadRequest();
            empleado.Id = id;

            var errores = _validador.Validar(empleado);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_empleadoRepositorio.Listar().Any(e => e.numId == empleado.numId && e.Id != id))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            if (!_empleadoRepositorio.Actualizar(id, empleado)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var empleado = _empleadoRepositorio.BuscarPorId(id);
            if (empleado == null) return NotFound();

            //No se puede eliminar un empleado con una o más facturas
            var moduloFactura = new ModuloFactura();
            //Usar string.Equals para evitar NullReferenceException
            bool tieneFacturas = moduloFactura.ListarFacturas()
                .Any(f => string.Equals(f.empleadoId.ToString(), empleado.Id.ToString(), StringComparison.OrdinalIgnoreCase));

            if (tieneFacturas)
            {
                return BadRequest(new { errores = new List<string> { "No se puede eliminar el empleado porque tiene facturas asociadas" } });
            }

            _empleadoRepositorio.Eliminar(id);
            return NoContent();
        }
    }
}