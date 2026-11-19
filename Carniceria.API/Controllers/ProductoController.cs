using AccesoDatos.Repositorios;
using Carniceria.API.Modulos;
using Carniceria.API.Validadores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using static Carniceria.API.Controllers.ProductosController;

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

        public class CombinacionRequest
        {
            public string categoria { get; set; }
            public string unidadMedida { get; set; }
        }

        [HttpPost("verificarCombinacion")]
        public IActionResult VerificarCombinacion([FromBody] CombinacionRequest request)
        {
            if (request == null) return BadRequest();

            var precio = _productoRepositorio.ObtenerPrecioPorCombinacion(request.categoria, request.unidadMedida);

            return Ok(new { existe = precio != null, precio = precio });
        }


        [HttpGet]
        public ActionResult<List<Cliente>> GetAll()
        {
            var clientes = _productoRepositorio.Listar;
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public ActionResult<Producto> GetById(Guid id)
        {
            var p = _productoRepositorio.buscarPorId(id);
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
            try
            {
                bool eliminado = _productoRepositorio.Eliminar(id);
                if (!eliminado) return NotFound(); 

                return NoContent(); 
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { errores = new List<string> { "No se puede eliminar el producto porque está asociado a una factura" } });
            }
        }
    }
}