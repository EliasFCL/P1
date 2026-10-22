using Carniceria.API.Modulos;
using Carniceria.API.Validadores;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Carniceria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private static ModuloFactura _modulo = new ModuloFactura();

        [HttpGet("{numero}")]
        public ActionResult<Factura> GetByNumero(string numero)
        {
            var factura = _modulo.BuscarFactura(numero);
            if (factura == null) return NotFound();
            return factura;
        }
        private readonly FacturaValidador _validador = new FacturaValidador();
        [HttpPost]
        public ActionResult<Factura> Create(Factura factura)
        {
            if (factura == null)
                return BadRequest(new { errores = new List<string> { "Factura vacía" } });

            if (factura.detalles == null || !factura.detalles.Any())
                return BadRequest(new { errores = new List<string> { "La factura debe tener al menos un detalle" } });

            var errores = _validador.Validar(factura);
            if (errores.Any())
                return BadRequest(new { errores });

            _modulo.registrarFactura(factura);
            return CreatedAtAction(nameof(GetByNumero), new { numero = factura.numeroFactura }, factura);
        }

        [HttpGet]
        public ActionResult<List<Factura>> GetAll() => _modulo.ListarFacturas();

        private static ModuloCliente _moduloCliente = new ModuloCliente();

        [HttpGet("buscar/{criterio}")]
        public ActionResult<List<Factura>> Buscar(string criterio)
        {
            var facturas = _modulo.ListarFacturas();

            //Buscar por de factura
            var porNumero = facturas
                .Where(f => f.numeroFactura.Contains(criterio, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (porNumero.Any()) return porNumero;

            //Buscar por cliente
            var cliente = _moduloCliente.ListarClientes()
                .FirstOrDefault(c => c.numId == criterio);
            if (cliente != null)
            {
                var porCliente = facturas
                    .Where(f => f.nombreCliente == cliente.nombre)
                    .ToList();
                if (porCliente.Any()) return porCliente;
            }

            return NotFound();
        }

        [HttpDelete("{numero}")]
        public IActionResult Delete(string numero)
        {
            var factura = _modulo.BuscarFactura(numero);
            if (factura == null) return NotFound();

            if (factura.estado != "Pendiente")
                return BadRequest(new { errores = new List<string> { "Solo se pueden eliminar facturas en estado Pendiente" } });

            _modulo.eliminarFactura(numero);
            return NoContent();
        }

        [HttpPut("{numero}")]
        public IActionResult Update(string numero, Factura factura)
        {
            if (factura == null) return BadRequest();

            var existente = _modulo.BuscarFactura(numero);
            if (existente == null) return NotFound();

            if (existente.estado == "Anulada")
                return BadRequest(new { errores = new List<string> { "Una factura anulada no puede modificarse" } });

            var errores = _validador.Validar(factura);
            if (errores.Any())
                return BadRequest(new { errores });
            if (!_modulo.ActualizarFactura(numero, factura)) return NotFound();
            return NoContent();
        }

        [HttpPut("{numero}/anular")]
        public IActionResult Anular(string numero)
        {
            var exito = _modulo.anularFactura(numero);

            if (!exito)
                return BadRequest(new { errores = new List<string> { "No se puede anular, compruebe que la factura exista y no esté anulada" } });

            return NoContent();
        }
    }
}