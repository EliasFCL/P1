//Estructura de los nuevos controllers: Guía para la creación de una web API 14:14 y
//https://learn.microsoft.com/lb-lu/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&preserve-view=true&tabs=visual-studio

using Microsoft.AspNetCore.Mvc;
using Models;
using Carniceria.API.Modulos;
using Carniceria.API.Validadores;
using AccesoDatos.Repositorios;

namespace Carniceria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteRepositorio _clienteRepositorio;
        private static ModuloCliente _modulo = new ModuloCliente();
        private readonly ClienteValidador _validador = new ClienteValidador();

        public ClientesController(ClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        [HttpGet]
        public ActionResult<List<Cliente>> GetAll()
        {
            var clientes = _clienteRepositorio.Listar();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public ActionResult<Cliente> GetById(Guid id)
        {
            var cliente = _clienteRepositorio.BuscarPorId(id);
            if (cliente == null) return NotFound();
            return cliente;
        }

        [HttpGet("porNumId/{numId}")]
        public ActionResult<Cliente> GetByNumId(string numId)
        {
            var cliente = _clienteRepositorio.buscarPorNumId(numId);
            if (cliente == null) return NotFound();
            return cliente;
        }

        [HttpPost]
        public ActionResult<Cliente> Create(Cliente cliente)
        {
            //Validar que no venga vacío
            if (cliente == null) return BadRequest("Cliente vacío");

            //Validar reglas de negocio (nombres, formato de cédula, etc.)
            var errores = _validador.Validar(cliente);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            //Verificar que la cédula no exista en la Base de Datos
            if (_clienteRepositorio.buscarPorNumId(cliente.numId) != null)
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            //Guardar en SQL Server
            bool guardadoExitoso = _clienteRepositorio.Registrar(cliente);

            if (!guardadoExitoso)
                return BadRequest(new { errores = new List<string> { "Error al guardar en la base de datos" } });

            //Retornar la respuesta exitosa
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Cliente cliente)
        {
            if (cliente == null) return BadRequest();
            cliente.Id = id;

            var errores = _validador.Validar(cliente);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_clienteRepositorio.Listar().Any(c => c.numId == cliente.numId && c.Id != id))
                return BadRequest(new { errores = new List<string> { "Ese identificador ya existe" } });

            _clienteRepositorio.Actualizar(id, cliente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var cliente = _clienteRepositorio.BuscarPorId(id);
            if (cliente == null) return NotFound();

            bool eliminado = _clienteRepositorio.eliminar(id);

            if (!eliminado)
            {
                //La base de datos no permite eliminar el cliente porque tiene facturas asociadas
                return BadRequest(new { errores = new List<string> { "No se puede eliminar el cliente porque tiene facturas asociadas" } });
            }

            return NoContent(); 
        }
    }
}