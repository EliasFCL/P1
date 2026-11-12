using AccesoDatos.Repositorios;
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
        private readonly FacturaRepositorio _facturaRepositorio;

        public FacturasController(FacturaRepositorio facturaRepositorio)
        {
            _facturaRepositorio = facturaRepositorio;
  
        }

        [HttpGet("{numero}")]
        public ActionResult<Factura> GetByNumero(string numero)
        {
            var factura = _facturaRepositorio.buscarFactura(numero);
            if (factura == null) return NotFound();
            return factura;
        }
        private readonly FacturaValidador _validador = new FacturaValidador();
        [HttpPost]
        public ActionResult<Factura> Create(Factura factura)
        {
            if (factura == null)
                return BadRequest(new { errores = new List<string> { "Factura vacía." } });

            if (factura.detalles == null || !factura.detalles.Any())
                return BadRequest(new { errores = new List<string> { "La factura debe tener al menos un detalle." } });


            bool guardado = _facturaRepositorio.Registrar(factura);

            if (!guardado)
                return BadRequest(new { errores = new List<string> { "Error al guardar la factura en la base de datos." } });

            return Ok(factura);
        }

        [HttpGet]
        public ActionResult<List<Factura>> GetAll()
        {
            return _facturaRepositorio.Listar();
        }

        private static ModuloCliente _moduloCliente = new ModuloCliente();

        [HttpGet("buscar/{criterio}")]
        public ActionResult<List<Factura>> Buscar(string criterio)
        {
            var facturas = _facturaRepositorio.Listar();

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
                    .Where(f => f.clienteId.ToString() == cliente.numId)
                    .ToList();
                if (porCliente.Any()) return porCliente;
            }

            return NotFound();
        }

        [HttpDelete("{numero}")]
        public IActionResult Delete(string numero)
        {
            var factura = _facturaRepositorio.buscarFactura(numero);
            if (factura == null) return NotFound(new { errores = new List<string> { "Factura no encontrada" } });

            //Validar estado antes de intentar borrar
            if (factura.estado != "Pendiente")
            {
                return BadRequest(new { errores = new List<string> { "No se puede eliminar una factura pagada o anulada" } });
            }

            _facturaRepositorio.Eliminar(numero);
            return NoContent();
        }

        [HttpPut("{numero}")]
        public IActionResult Update(string numero, Factura factura)
        {
            if (factura == null) return BadRequest();

            factura.numeroFactura = numero; // Nos aseguramos de que el número viaje

            if (!_facturaRepositorio.Actualizar(factura))
                return NotFound();

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