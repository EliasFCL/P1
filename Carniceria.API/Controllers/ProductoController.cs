using Carniceria.API.Modulos;
using Carniceria.API.Validadores;
using Microsoft.AspNetCore.Mvc;
using Models;
using AccesoDatos.Repositorios;

namespace Carniceria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoRepositorio _productoRepositorio;
        private readonly validarProducto _validador = new validarProducto();

        public ProductosController(ProductoRepositorio productoRepositorio)
        {
            _productoRepositorio = productoRepositorio;
        }

        [HttpGet]
        public ActionResult<List<Producto>> GetAll() => _productoRepositorio.Listar();

        [HttpGet("{id}")]
        public ActionResult<Producto> GetById(Guid id)
        {
            var p = _productoRepositorio.BuscarPorId(id);
            if (p == null) return NotFound();
            return p;
        }

        [HttpPost]
        public ActionResult<Producto> Create(Producto producto)
        {
            if (producto == null) return BadRequest("Producto vacío");

            var errores = _validador.Validar(producto);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            _productoRepositorio.Registrar(producto);
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Producto producto)
        {
            if (producto == null) return BadRequest();
            producto.Id = id;

            var errores = _validador.Validar(producto);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (!_productoRepositorio.Actualizar(producto)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!_productoRepositorio.Eliminar(id)) return NotFound();
            return NoContent();
        }
    }
}