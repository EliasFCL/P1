//Estructura de los nuevos controllers: https://learn.microsoft.com/is-is/aspnet/core/web-api/?view=aspnetcore-7.0#1 y
//https://learn.microsoft.com/lb-lu/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&preserve-view=true&tabs=visual-studio

using Microsoft.AspNetCore.Mvc;
using Models;
using Carniceria.API.Modulos;
using Carniceria.API.Validadores;

namespace Carniceria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private static ModuloCliente _modulo = new ModuloCliente();
        private readonly ClienteValidador _validador = new ClienteValidador();

        [HttpGet]
        public ActionResult<List<Cliente>> GetAll() => _modulo.ListarClientes();

        [HttpGet("{id}")]
        public ActionResult<Cliente> GetById(int id)
        {
            var c = _modulo.ListarClientes().FirstOrDefault(x => x.Id == id);
            if (c == null) return NotFound();
            return c;
        }

        [HttpGet("porNumId/{numId}")]
        public ActionResult<Cliente> GetByNumId(string numId)
        {
            var c = _modulo.BuscarCliente(numId);
            if (c == null) return NotFound();
            return c;
        }

        [HttpPost]
        public ActionResult<Cliente> Create(Cliente cliente)
        {
            if (cliente == null) return BadRequest("Cliente vacío");

            if (_modulo.ListarClientes().Any(c => c.numId == cliente.numId))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            var errores = _validador.Validar(cliente);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            _modulo.RegistrarCliente(cliente);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Cliente cliente)
        {
            if (cliente == null) return BadRequest();
            cliente.Id = id;

            var errores = _validador.Validar(cliente);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_modulo.ListarClientes().Any(c => c.numId == cliente.numId && c.Id != id))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            if (!_modulo.ActualizarCliente(id.ToString(), cliente)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cliente = _modulo.ListarClientes().FirstOrDefault(c => c.Id == id);
            if (cliente == null) return NotFound();

            // No se puede borrar a un cliente con una o más facturas
            var moduloFactura = new ModuloFactura();
            bool tieneFacturas = moduloFactura.ListarFacturas()
                .Any(f => string.Equals(f.nombreCliente, cliente.nombre, StringComparison.OrdinalIgnoreCase));

            if (tieneFacturas)
            {
                return BadRequest(new { errores = new List<string> { "No se puede eliminar el cliente porque tiene facturas asociadas" } });
            }

            _modulo.EliminarCliente(id.ToString());
            return NoContent();
        }
    }
}