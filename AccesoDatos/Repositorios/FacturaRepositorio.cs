//Adaptar anular: https://learn.microsoft.com/es-mx/ef/core/saving
using AccesoDatos.Contexto;
using AccesoDatos.Contexto.AccesoDatos.Contexto;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
using Models;

namespace AccesoDatos.Repositorios
{
    public class FacturaRepositorio
    {
        private readonly string conexionString;
        private readonly FacturaDbContexto contexto;
        Conectar conectar = new Conectar();

        public FacturaRepositorio(FacturaDbContexto contexto)
        {
            this.contexto = contexto;
        }

        public bool Registrar(Factura factura)
        {
            using (var transaccion = contexto.Database.BeginTransaction())
            {
                try
                {

                    var clientes = contexto.Clientes.ToList();

                    var cliente = contexto.Clientes
                        .FirstOrDefault(c => c.Id == factura.clienteId);

                    var empleado = contexto.Empleados .FirstOrDefault(e => e.Id == factura.empleadoId);

                    if (cliente == null)
                        throw new Exception("El cliente seleccionado no existe en la base de datos.");

                    Guid clienteId = cliente.Id;

                    if (empleado == null)
                        throw new Exception("El empleado seleccionado no existe en la base de datos.");

                    Guid empleadoId = empleado.Id;

                    //Calcular antes de guardar
                    factura.subtotal = factura.detalles.Sum(d => d.cantidad * d.precioUnitario);
                    factura.montoDescuento = (factura.subtotal * factura.descuento) / 100m;
                    factura.baseGravable = factura.subtotal - factura.montoDescuento;
                    factura.iva = factura.baseGravable * 0.13m;
                    factura.total = factura.baseGravable + factura.iva;

                    //Asignar los IDs
                    factura.clienteId = clienteId;
                    factura.empleadoId = empleadoId;

                    //Guardar factura
                    contexto.Facturas.Add(factura);
                    contexto.SaveChanges();

                    //Guardar detalles
                    foreach (var detalle in factura.detalles)
                    {
                        //Buscar el producto por su nombre
                        var producto = contexto.Productos
                            .FirstOrDefault(p => p.nombre == detalle.producto);

                        if (producto == null)
                            throw new Exception($"El producto '{detalle.producto}' no existe en la base de datos.");

                        Guid productoId = producto.Id;

                        //Comprobar inventario
                        if (factura.estado == "Pagada")
                        {
                            decimal inventarioActual = producto.existenciaActual;

                            if (inventarioActual < detalle.cantidad)
                            {
                                throw new Exception(
                                    $"Su solicitud supera la cantidad almacenada en el inventario. " +
                                    $"Solicitado: {detalle.cantidad}, Disponible: {inventarioActual}");
                            }

                            // Actualizar inventario
                            producto.existenciaActual -= detalle.cantidad;
                        }

                        //Calcular subtotal del detalle
                        detalle.subtotal = detalle.cantidad * detalle.precioUnitario;

                        //Crear detalle
                        var nuevoDetalle = new DetalleFactura
                        {
                            Id = Guid.NewGuid(),
                            numeroFactura = factura.numeroFactura,
                            ProductoId = productoId,
                            unidadMedida = detalle.unidadMedida,
                            cantidad = detalle.cantidad,
                            precioUnitario = detalle.precioUnitario,
                            subtotal = detalle.subtotal
                        };

                        contexto.DetallesFactura.Add(nuevoDetalle);
                    }

                    //Guardar cambios
                    contexto.SaveChanges();

                    //Confirmar transacción
                    transaccion.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    // Cancelar todos los cambios
                    transaccion.Rollback();
                    contexto.ChangeTracker.Clear();

                    throw new Exception("Error al guardar la factura: " + ex.Message);
                }
            }
        }

        public List<Factura> Listar()
        {
            var lista = new List<Factura>();

            lista = (
                from f in contexto.Facturas
                join c in contexto.Clientes on f.clienteId equals c.Id
                join e in contexto.Empleados on f.empleadoId equals e.Id
                select new Factura
                {
                    numeroFactura = f.numeroFactura,
                    fecha = f.fecha,
                    clienteId = f.clienteId,
                    empleadoId = f.empleadoId,
                    nombreCliente = c.nombre,
                    empleadoResponsable = e.nombre,
                    metodoPago = f.metodoPago,
                    descuento = f.descuento,
                    estado = f.estado,
                    subtotal = f.subtotal,
                    montoDescuento = f.montoDescuento,
                    baseGravable = f.baseGravable,
                    iva = f.iva,
                    total = f.total,
                    detalles = new List<DetalleFactura>()
                }
            ).ToList();

            foreach (var factura in lista)
            {
                factura.detalles = (
                    from d in contexto.DetallesFactura
                    join p in contexto.Productos on d.ProductoId equals p.Id
                    where d.numeroFactura == factura.numeroFactura
                    select new DetalleFactura
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        producto = p.nombre,
                        unidadMedida = d.unidadMedida,
                        cantidad = d.cantidad,
                        precioUnitario = d.precioUnitario,
                        subtotal = d.subtotal
                    }
                ).ToList();
            }
            return lista;
        }

        public List<Factura> buscarFactura(string criterio)
        {
            criterio = criterio.Trim();

            var facturas = (
                from f in contexto.Facturas
                join c in contexto.Clientes on f.clienteId equals c.Id
                join e in contexto.Empleados on f.empleadoId equals e.Id
                where f.numeroFactura == criterio || c.numId == criterio
                select new Factura
                {
                    numeroFactura = f.numeroFactura,
                    fecha = f.fecha,
                    clienteId = f.clienteId,
                    empleadoId = f.empleadoId,
                    nombreCliente = c.nombre,
                    empleadoResponsable = e.nombre,
                    metodoPago = f.metodoPago,
                    descuento = f.descuento,
                    estado = f.estado,
                    subtotal = f.subtotal,
                    montoDescuento = f.montoDescuento,
                    baseGravable = f.baseGravable,
                    iva = f.iva,
                    total = f.total,
                    detalles = new List<DetalleFactura>()
                }
            ).ToList();

            foreach (var factura in facturas)
            {
                factura.detalles = (
                    from d in contexto.DetallesFactura
                    join p in contexto.Productos on d.ProductoId equals p.Id
                    where d.numeroFactura == factura.numeroFactura
                    select new DetalleFactura
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        producto = p.nombre,
                        unidadMedida = d.unidadMedida,
                        cantidad = d.cantidad,
                        precioUnitario = d.precioUnitario,
                        subtotal = d.subtotal
                    }
                ).ToList();
            }

            return facturas;
        }
        public bool Eliminar(string numeroFactura)
        {
            using (var transaccion = contexto.Database.BeginTransaction())
            {
                try
                {
                    var detalles = contexto.DetallesFactura
                        .Where(d => d.numeroFactura == numeroFactura)
                        .ToList();

                    contexto.DetallesFactura.RemoveRange(detalles);

                    var factura = contexto.Facturas
                        .FirstOrDefault(f => f.numeroFactura == numeroFactura);

                    if (factura == null)
                        throw new Exception("No se encontró la factura para eliminar.");

                    contexto.Facturas.Remove(factura);

                    contexto.SaveChanges();

                    transaccion.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    contexto.ChangeTracker.Clear();

                    throw new Exception("ERROR AL ELIMINAR FACTURA: " + ex.Message);
                }
            }
        }

        public bool Actualizar(string numero, Factura factura)
        {
            using (var transaccion = contexto.Database.BeginTransaction())
            {
                try
                {
                    factura.numeroFactura = numero;

                    var facturaExistente = contexto.Facturas
                        .FirstOrDefault(f => f.numeroFactura == numero);

                    if (facturaExistente == null)
                        throw new Exception("La factura no existe en la base de datos.");

                    string estadoAnterior = facturaExistente.estado;

                    if (estadoAnterior == "Pagada")
                    {
                        var detallesAnteriores = contexto.DetallesFactura
                            .Where(d => d.numeroFactura == numero)
                            .ToList();

                        foreach (var detalleAnterior in detallesAnteriores)
                        {
                            var productoAnterior = contexto.Productos
                                .FirstOrDefault(p => p.Id == detalleAnterior.ProductoId);

                            if (productoAnterior != null)
                            {
                                productoAnterior.existenciaActual += detalleAnterior.cantidad;
                            }
                        }
                    }

                    decimal subtotalCalculado = factura.detalles.Sum(
                        d => d.cantidad * d.precioUnitario);

                    if (subtotalCalculado > 0)
                    {
                        factura.subtotal = subtotalCalculado;
                        factura.montoDescuento = (factura.subtotal * factura.descuento) / 100m;
                        factura.baseGravable = factura.subtotal - factura.montoDescuento;
                        factura.iva = factura.baseGravable * 0.13m;
                        factura.total = factura.baseGravable + factura.iva;
                    }
                    else
                    {
                        factura.subtotal = facturaExistente.subtotal;
                        factura.montoDescuento = facturaExistente.montoDescuento;
                        factura.baseGravable = facturaExistente.baseGravable;
                        factura.iva = facturaExistente.iva;
                        factura.total = facturaExistente.total;
                    }
                    facturaExistente.clienteId = factura.clienteId;
                    facturaExistente.empleadoId = factura.empleadoId;
                    facturaExistente.metodoPago = factura.metodoPago;
                    facturaExistente.descuento = factura.descuento;
                    facturaExistente.estado = factura.estado;
                    facturaExistente.subtotal = factura.subtotal;
                    facturaExistente.montoDescuento = factura.montoDescuento;
                    facturaExistente.baseGravable = factura.baseGravable;
                    facturaExistente.iva = factura.iva;
                    facturaExistente.total = factura.total;

                    var detallesViejos = contexto.DetallesFactura
                        .Where(d => d.numeroFactura == numero)
                        .ToList();

                    contexto.DetallesFactura.RemoveRange(detallesViejos);

                    foreach (var detalle in factura.detalles)
                    {
                        var producto = contexto.Productos
                            .FirstOrDefault(p =>
                                p.nombre.Trim() == detalle.producto.Trim());

                        if (producto == null)
                            throw new Exception(
                                $"El producto '{detalle.producto}' no existe en el inventario.");

                        Guid productoId = producto.Id;

                        if (factura.estado == "Pagada")
                        {
                            decimal inventarioActual = producto.existenciaActual;

                            if (inventarioActual < detalle.cantidad)
                            {
                                throw new Exception(
                                    $"La cantidad solicitada para '{detalle.producto}' supera " +
                                    "la cantidad almacenada en el inventario");
                            }

                            producto.existenciaActual -= detalle.cantidad;
                        }

                        var nuevoDetalle = new DetalleFactura
                        {
                            Id = Guid.NewGuid(),
                            numeroFactura = numero,
                            ProductoId = productoId,
                            unidadMedida = detalle.unidadMedida,
                            cantidad = detalle.cantidad,
                            precioUnitario = detalle.precioUnitario,
                            subtotal = detalle.cantidad * detalle.precioUnitario
                        };

                        contexto.DetallesFactura.Add(nuevoDetalle);
                    }

                    contexto.SaveChanges();
                    transaccion.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    contexto.ChangeTracker.Clear();

                    throw new Exception("Error al actualizar factura: " + ex.Message);
                }
            }
        }
        public List<Factura> obtenerVentas()
        {
            var lista = new List<Factura>();

            int diferencia = (7 + ((int)DateTime.Now.DayOfWeek -
                (int)DayOfWeek.Monday)) % 7;

            DateTime lunes = DateTime.Now.AddDays(-diferencia).Date;
            DateTime domingo = lunes.AddDays(6).Date;

            lista = (
                from f in contexto.Facturas
                join c in contexto.Clientes on f.clienteId equals c.Id
                join e in contexto.Empleados on f.empleadoId equals e.Id
                where f.estado == "Pagada"
                    && f.fecha >= lunes
                    && f.fecha < domingo.AddDays(1)
                orderby f.total descending
                select new Factura
                {
                    numeroFactura = f.numeroFactura,
                    fecha = f.fecha,
                    clienteId = f.clienteId,
                    empleadoId = f.empleadoId,
                    nombreCliente = c.nombre,
                    empleadoResponsable = e.nombre,
                    subtotal = f.subtotal,
                    montoDescuento = f.montoDescuento,
                    iva = f.iva,
                    total = f.total
                }
            ).ToList();
            return lista;
        }

        public bool Anular(string numero)
        {
            using (var transaccion = contexto.Database.BeginTransaction())
            {
                try
                {
                    var factura = contexto.Facturas
                        .FirstOrDefault(f => f.numeroFactura == numero.Trim());

                    if (factura == null)
                        throw new Exception("La factura no existe en la base de datos.");

                    string estadoActual = factura.estado.Trim();

                    if (estadoActual == "Anulada")
                        throw new Exception("La factura ya se encuentra anulada.");

                    if (estadoActual == "Pagada")
                    {
                        var detalles = contexto.DetallesFactura
                            .Where(d => d.numeroFactura == numero.Trim())
                            .ToList();

                        foreach (var detalle in detalles)
                        {
                            var producto = contexto.Productos
                                .FirstOrDefault(p => p.Id == detalle.ProductoId);

                            if (producto != null)
                            {
                                producto.existenciaActual += detalle.cantidad;
                            }
                        }
                    }

                    factura.estado = "Anulada";
                    contexto.SaveChanges();
                    transaccion.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    contexto.ChangeTracker.Clear();

                    throw new Exception("Hubo un error al anular la factura: " + ex.Message);
                }
            }
        }
    }
}