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
            var facturas = _facturaRepositorio.buscarFactura(numero);

            var factura = facturas.FirstOrDefault();

            if (factura == null) return NotFound();

            return Ok(factura);
        }
        private readonly FacturaValidador _validador = new FacturaValidador();
        [HttpPost]
        public ActionResult<Factura> Create(Factura factura)
        {
            try { 
            if (factura == null)
                return BadRequest(new { errores = new List<string> { "Factura vacía." } });
            if (factura.descuento < 0 || factura.descuento > 25)
            {
                return BadRequest(new { errores = new List<string> { "El descuento debe estar entre 0 y 25" } });
            }

            if (factura.detalles == null || !factura.detalles.Any())
                return BadRequest(new { errores = new List<string> { "La factura debe tener al menos un detalle" } });


            bool guardado = _facturaRepositorio.Registrar(factura);

            if (!guardado)
                return BadRequest(new { errores = new List<string> { "Error al guardar la factura en la base de datos." } });

            return Ok(factura);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errores = new List<string> { ex.Message } });
            }
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
            var facturas = _facturaRepositorio.buscarFactura(criterio);

            // Si no encuentra nada, devolvemos un error 404 con el criterio recibido
            if (facturas == null || facturas.Count == 0)
            {
                return NotFound(new { mensaje = "No se encontro nada para: " + criterio });
            }

            return Ok(facturas);
        }

        [HttpDelete("{numero}")]
        public IActionResult Delete(string numero)
        {
            var facturas = _facturaRepositorio.buscarFactura(numero);

            var factura = facturas.FirstOrDefault();

            if (factura == null) return NotFound();

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
            try {
            if (factura == null) return BadRequest();

            factura.numeroFactura = numero; //Asegurar el número de factura que se está actualizando

            if (!_facturaRepositorio.Actualizar(numero, factura))
                return NotFound();

            return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { errores = new List<string> { ex.Message } });
            }
        }

        [HttpPut("{numero}/anular")]
        public IActionResult Anular(string numero)
        {
            bool exito = _facturaRepositorio.Anular(numero);

            if (!exito)
            {
                return BadRequest(new { errores = new List<string> { "No se puede anular, compruebe que la factura exista y no esté anulada" } });
            }

            return NoContent();
        }

        [HttpGet("reporte-semanal")]
        public ActionResult<List<Factura>> GetReporteSemanal()
        {
            var reporte = _facturaRepositorio.obtenerVentas();
            return Ok(reporte);
        }
    }
}