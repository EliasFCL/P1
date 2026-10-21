using Carniceria.API.Modulos;
using Carniceria.API.Validadores;
using Microsoft.AspNetCore.Mvc;
using Models;
using Carniceria.API.Modulos;

namespace Carniceria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private static ModuloProducto _modulo = new ModuloProducto();
        private readonly validarProducto _validador = new validarProducto();

        // GET: api/productos
        [HttpGet]
        public ActionResult<List<Producto>> GetAll() => _modulo.ListarProductos();

        // GET: api/productos/5
        [HttpGet("{id}")]
        public ActionResult<Producto> GetById(int id)
        {
            var p = _modulo.ListarProductos().FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound();
            return p;
        }

        // GET: api/productos/buscar/CODIGO_O_NOMBRE
        [HttpGet("buscar/{busqueda}")]
        public ActionResult<Producto> Buscar(string busqueda)
        {

            var p = _modulo.BuscarProducto(busqueda);
            if (p == null) return NotFound();
            return p;
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> Create(Producto producto)
        {
            if (producto == null) return BadRequest("Producto vacío");

            var errores = _validador.Validar(producto);
            
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_modulo.ListarProductos().Any(p => p.codigo == producto.codigo))
                return BadRequest(new { errores = new List<string> { "Ese código de producto ya existe." } });

            _modulo.RegistrarProducto(producto);
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Producto producto)
        {
            if (producto == null) return BadRequest();
            producto.Id = id;

            var errores = _validador.Validar(producto);
            if (errores != null && errores.Any())
                return BadRequest(new { errores });

            if (_modulo.ListarProductos().Any(p => p.codigo == producto.codigo && p.Id != id))
                return BadRequest(new { errores = new List<string> { "Ese código de producto ya existe." } });

            if (!_modulo.ActualizarProducto(id.ToString(), producto)) return NotFound();
            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_modulo.EliminarProducto(id.ToString())) return NotFound();
            return NoContent();
        }

        // Clase auxiliar para recibir la solicitud de verificación de combinación
        public class CombinacionRequest
        {
            public string? unidadMedida { get; set; }
            public string? categoria { get; set; }
        }

        // POST: api/productos/verificarCombinacion
        [HttpPost("verificarCombinacion")]
        public ActionResult VerificarCombinacion([FromBody] CombinacionRequest request)
        {
            var productos = _modulo.ListarProductos();
            var match = productos.FirstOrDefault(p =>
                p.unidadMedida == request.unidadMedida &&
                p.categoria == request.categoria);
            return Ok(new { existe = match != null, precio = match?.precio });
        }

    }
}