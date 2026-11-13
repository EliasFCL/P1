using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Microsoft.Data.SqlClient;
using AccesoDatos.Contexto;

namespace AccesoDatos.Repositorios
{
    public class FacturaRepositorio
    {
        private readonly string conexionString;
        Conectar conectar = new Conectar();

        public FacturaRepositorio(string connectionString)
        {
            conexionString = conectar.CadenaConexion();
        }

        public bool Registrar(Factura factura)
        {
            // Validaciones de seguridad
            if (factura == null || factura.detalles == null || !factura.detalles.Any())
                throw new Exception("La factura debe tener al menos un detalle.");

            if (string.IsNullOrWhiteSpace(factura.nombreCliente))
                throw new Exception("Debe seleccionar un cliente válido.");

            if (string.IsNullOrWhiteSpace(factura.empleadoResponsable))
                throw new Exception("Debe seleccionar un empleado válido.");

            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                // Si algo falla, se cancela todo
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        //Buscar el ID del cliente por su nombre
                        Guid clienteId = Guid.Empty;
                        using (var cmdC = new SqlCommand("SELECT TOP 1 Id FROM Clientes WHERE nombre = @nombre", conexion, transaccion))
                        {
                            cmdC.Parameters.AddWithValue("@nombre", factura.nombreCliente);
                            var res = cmdC.ExecuteScalar();
                            if (res != null) clienteId = (Guid)res;
                        }
                        if (clienteId == Guid.Empty) throw new Exception("El cliente seleccionado no existe en la base de datos.");

                        //Buscar ID del empleado por su nombre
                        Guid empleadoId = Guid.Empty;
                        using (var cmdE = new SqlCommand("SELECT TOP 1 Id FROM Empleados WHERE nombre = @nombre", conexion, transaccion))
                        {
                            cmdE.Parameters.AddWithValue("@nombre", factura.empleadoResponsable);
                            var res = cmdE.ExecuteScalar();
                            if (res != null) empleadoId = (Guid)res;
                        }
                        if (empleadoId == Guid.Empty) throw new Exception("El empleado seleccionado no existe en la base de datos.");

                        //Calcular antes de gaurdar
                        factura.subtotal = factura.detalles.Sum(d => d.cantidad * d.precioUnitario);
                        factura.montoDescuento = (factura.subtotal * factura.descuento) / 100m;
                        factura.baseGravable = factura.subtotal - factura.montoDescuento;
                        factura.iva = factura.baseGravable * 0.13m;
                        factura.total = factura.baseGravable + factura.iva;

                        //Consulta para factura
                        string queryFactura = @"INSERT INTO Facturas (numeroFactura, fecha, ClienteId, EmpleadoId, metodoPago, descuento, estado, subtotal, montoDescuento, baseGravable, iva, total) 
                                        VALUES (@numero, @fecha, @cliente, @empleado, @pago, @descuento, @estado, @subtotal, @montoDesc, @base, @iva, @total)";

                        using (var cmdF = new SqlCommand(queryFactura, conexion, transaccion))
                        {
                            cmdF.Parameters.AddWithValue("@numero", factura.numeroFactura);
                            cmdF.Parameters.AddWithValue("@fecha", factura.fecha);
                            cmdF.Parameters.AddWithValue("@cliente", clienteId);
                            cmdF.Parameters.AddWithValue("@empleado", empleadoId);
                            cmdF.Parameters.AddWithValue("@pago", factura.metodoPago);
                            cmdF.Parameters.AddWithValue("@descuento", factura.descuento);
                            cmdF.Parameters.AddWithValue("@estado", factura.estado);
                            cmdF.Parameters.AddWithValue("@subtotal", factura.subtotal);
                            cmdF.Parameters.AddWithValue("@montoDesc", factura.montoDescuento);
                            cmdF.Parameters.AddWithValue("@base", factura.baseGravable);
                            cmdF.Parameters.AddWithValue("@iva", factura.iva);
                            cmdF.Parameters.AddWithValue("@total", factura.total);

                            cmdF.ExecuteNonQuery();
                        }

                        //Consulta para detalles
                        string queryDetalle = @"INSERT INTO DetallesFactura (Id, numeroFactura, ProductoId, unidadMedida, cantidad, precioUnitario, subtotal) 
                        VALUES (@Id, @numero, @producto, @unidad, @cantidad, @precio, @subtotal)";

                        foreach (var detalle in factura.detalles)
                        {
                            //Buscar el Guid del producto por su nombre
                            Guid productoId = Guid.Empty;
                            using (var cmdP = new SqlCommand("SELECT TOP 1 Id FROM Productos WHERE nombre = @nombre", conexion, transaccion))
                            {
                                cmdP.Parameters.AddWithValue("@nombre", detalle.producto);
                                var res = cmdP.ExecuteScalar();
                                if (res != null) productoId = (Guid)res;
                            }

                            if (productoId == Guid.Empty)
                                throw new Exception($"El producto '{detalle.producto}' no existe en la base de datos.");

                            //Calcular subtotal del detalle
                            detalle.subtotal = detalle.cantidad * detalle.precioUnitario;

                            using (var cmdD = new SqlCommand(queryDetalle, conexion, transaccion))
                            {
                                cmdD.Parameters.AddWithValue("@Id", Guid.NewGuid()); 
                                cmdD.Parameters.AddWithValue("@numero", factura.numeroFactura);
                                cmdD.Parameters.AddWithValue("@producto", productoId);
                                cmdD.Parameters.AddWithValue("@unidad", detalle.unidadMedida);
                                cmdD.Parameters.AddWithValue("@cantidad", detalle.cantidad);
                                cmdD.Parameters.AddWithValue("@precio", detalle.precioUnitario);
                                cmdD.Parameters.AddWithValue("@subtotal", detalle.subtotal);

                                cmdD.ExecuteNonQuery();
                            }
                            //Actualizar el inventario
                            if (factura.estado == "Pagada")
                            {
                                string queryUpdateStock = "UPDATE Productos SET existenciaActual = existenciaActual - @cantidad WHERE Id = @productoId";
                                using (var cmdStock = new SqlCommand(queryUpdateStock, conexion, transaccion))
                                {
                                    cmdStock.Parameters.AddWithValue("@cantidad", detalle.cantidad);
                                    cmdStock.Parameters.AddWithValue("@productoId", productoId);
                                    cmdStock.ExecuteNonQuery();
                                }
                            }
                        }

                        //Confirmar la transacción
                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Se cancela la transacción 
                        transaccion.Rollback();
                        throw new Exception("ERROR AL GUARDAR FACTURA: " + ex.Message);
                    }
                }
            }
        }


        public List<Factura> Listar()
        {
            var lista = new List<Factura>();
            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string consulta = @"
            SELECT f.numeroFactura, f.fecha, c.nombre AS nombreCliente, e.nombre AS empleadoResponsable, 
                   f.metodoPago, f.descuento, f.estado, f.subtotal, f.montoDescuento, f.baseGravable, f.iva, f.total
            FROM Facturas f
            INNER JOIN Clientes c ON f.ClienteId = c.Id
            INNER JOIN Empleados e ON f.EmpleadoId = e.Id";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Factura
                            {
                                numeroFactura = lector["numeroFactura"].ToString(),
                                fecha = (DateTime)lector["fecha"],
                                nombreCliente = lector["nombreCliente"].ToString(),
                                empleadoResponsable = lector["empleadoResponsable"].ToString(),
                                metodoPago = lector["metodoPago"].ToString(),
                                descuento = Convert.ToInt32(lector["descuento"]),
                                estado = lector["estado"].ToString(),
                                subtotal = Convert.ToDecimal(lector["subtotal"]),
                                montoDescuento = Convert.ToDecimal(lector["montoDescuento"]),
                                baseGravable = Convert.ToDecimal(lector["baseGravable"]),
                                iva = Convert.ToDecimal(lector["iva"]),
                                total = Convert.ToDecimal(lector["total"]),
                                detalles = new List<DetalleFactura>() // Inicializamos la lista
                            });
                        }
                    }
                }

                foreach (var factura in lista)
                {
                    string consultaDetalles = @"
                SELECT d.Id, p.nombre AS producto, d.cantidad, d.precioUnitario, d.subtotal
                FROM DetallesFactura d
                INNER JOIN Productos p ON d.ProductoId = p.Id
                WHERE d.numeroFactura = @numeroFactura";

                    using (var comandoDetalle = new SqlCommand(consultaDetalles, conexion))
                    {
                        comandoDetalle.Parameters.AddWithValue("@numeroFactura", factura.numeroFactura);

                        using (var lectorDetalle = comandoDetalle.ExecuteReader())
                        {
                            while (lectorDetalle.Read())
                            {
                                factura.detalles.Add(new DetalleFactura
                                {
                                    Id = (Guid)lectorDetalle["Id"], // Ojo: en tu SQL el Id es UNIQUEIDENTIFIER
                                    producto = lectorDetalle["producto"].ToString(),
                                    cantidad = Convert.ToInt32(lectorDetalle["cantidad"]),
                                    precioUnitario = Convert.ToDecimal(lectorDetalle["precioUnitario"]),
                                    subtotal = Convert.ToDecimal(lectorDetalle["subtotal"])
                                });
                            }
                        }
                    }
                }
            }
            return lista;
        }

        public List<Factura> buscarFactura(string criterio)
        {
            List<Factura> facturas = new List<Factura>();

            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string consulta = @"
            SELECT f.numeroFactura, f.fecha, c.nombre AS nombreCliente, e.nombre AS empleadoResponsable, 
                   f.metodoPago, f.descuento, f.estado, f.subtotal, f.montoDescuento, f.baseGravable, f.iva, f.total
            FROM Facturas f
            INNER JOIN Clientes c ON f.ClienteId = c.Id
            INNER JOIN Empleados e ON f.EmpleadoId = e.Id
            WHERE f.numeroFactura = @criterio OR c.numId = @criterio";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@criterio", criterio.Trim());

                    using (var lector = comando.ExecuteReader())
                    {
                        // Usamos WHILE para leer todas las facturas que encuentre
                        while (lector.Read())
                        {
                            facturas.Add(new Factura
                            {
                                numeroFactura = lector["numeroFactura"].ToString(),
                                fecha = (DateTime)lector["fecha"],
                                nombreCliente = lector["nombreCliente"].ToString(),
                                empleadoResponsable = lector["empleadoResponsable"].ToString(),
                                metodoPago = lector["metodoPago"].ToString(),
                                descuento = Convert.ToInt32(lector["descuento"]),
                                estado = lector["estado"].ToString(),
                                subtotal = Convert.ToDecimal(lector["subtotal"]),
                                montoDescuento = Convert.ToDecimal(lector["montoDescuento"]),
                                baseGravable = Convert.ToDecimal(lector["baseGravable"]),
                                iva = Convert.ToDecimal(lector["iva"]),
                                total = Convert.ToDecimal(lector["total"]),
                                detalles = new List<DetalleFactura>()
                            });
                        }
                    } 
                }

                //Leer detalles
                string consultaDetalles = @"
            SELECT d.Id, p.nombre AS producto, d.unidadMedida, d.cantidad, d.precioUnitario, d.subtotal
            FROM DetallesFactura d
            INNER JOIN Productos p ON d.ProductoId = p.Id
            WHERE d.numeroFactura = @numeroFactura";

                foreach (var factura in facturas)
                {
                    using (var comandoDetalle = new SqlCommand(consultaDetalles, conexion))
                    {
                        comandoDetalle.Parameters.AddWithValue("@numeroFactura", factura.numeroFactura);

                        using (var lectorDetalle = comandoDetalle.ExecuteReader())
                        {
                            while (lectorDetalle.Read())
                            {
                                factura.detalles.Add(new DetalleFactura
                                {
                                    Id = (Guid)lectorDetalle["Id"],
                                    producto = lectorDetalle["producto"].ToString(),
                                    unidadMedida = lectorDetalle["unidadMedida"].ToString(),
                                    cantidad = Convert.ToInt32(lectorDetalle["cantidad"]),
                                    precioUnitario = Convert.ToDecimal(lectorDetalle["precioUnitario"]),
                                    subtotal = Convert.ToDecimal(lectorDetalle["subtotal"])
                                });
                            }
                        }
                    }
                }
            }

            return facturas;
        }

        public bool Eliminar(string numeroFactura)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        // Primero, eliminar los detalles de la factura
                        string eliminarDetalles = "DELETE FROM DetallesFactura WHERE numeroFactura = @numeroFactura";
                        using (var cmdDetalles = new SqlCommand(eliminarDetalles, conexion, transaccion))
                        {
                            cmdDetalles.Parameters.AddWithValue("@numeroFactura", numeroFactura);
                            cmdDetalles.ExecuteNonQuery();
                        }
                        // Luego, eliminar la factura
                        string eliminarFactura = "DELETE FROM Facturas WHERE numeroFactura = @numeroFactura";
                        using (var cmdFactura = new SqlCommand(eliminarFactura, conexion, transaccion))
                        {
                            cmdFactura.Parameters.AddWithValue("@numeroFactura", numeroFactura);
                            int filasAfectadas = cmdFactura.ExecuteNonQuery();
                            if (filasAfectadas == 0)
                                throw new Exception("No se encontró la factura para eliminar.");
                        }
                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        throw new Exception("ERROR AL ELIMINAR FACTURA: " + ex.Message);
                    }
                }
            }
        }

        public bool Actualizar(string numero, Factura factura)
        {
            factura.numeroFactura = numero;

            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        //Encontrar el estado anterior de la factura
                        string estadoAnterior = "";
                        string queryEstado = "SELECT estado FROM Facturas WHERE numeroFactura = @numero";
                        using (var cmdEstado = new SqlCommand(queryEstado, conexion, transaccion))
                        {
                            cmdEstado.Parameters.AddWithValue("@numero", numero);
                            var res = cmdEstado.ExecuteScalar();
                            if (res != null) estadoAnterior = res.ToString();
                        }

                        //Resetear inventario si la factura estaba pagada y ahora no lo está
                        if (estadoAnterior == "Pagada")
                        {
                            var ajustes = new List<(Guid id, int cant)>();

                            string queryStockViejo = @"
                                SELECT d.ProductoId, d.cantidad 
                                FROM DetallesFactura d 
                                WHERE d.numeroFactura = @numero";

                            using (var cmdStockViejo = new SqlCommand(queryStockViejo, conexion, transaccion))
                            {
                                cmdStockViejo.Parameters.AddWithValue("@numero", numero);
                                using (var lectorStock = cmdStockViejo.ExecuteReader())
                                {
                                    //Leer y guardar sin hacer el update
                                    while (lectorStock.Read())
                                    {
                                        ajustes.Add((
                                            (Guid)lectorStock["ProductoId"],
                                            Convert.ToInt32(lectorStock["cantidad"])
                                        ));
                                    }
                                }
                            }

                            //Hacer update
                            foreach (var ajuste in ajustes)
                            {
                                string querySumar = "UPDATE Productos SET existenciaActual = existenciaActual + @cantidad WHERE Id = @id";
                                using (var cmdSumar = new SqlCommand(querySumar, conexion, transaccion))
                                {
                                    cmdSumar.Parameters.AddWithValue("@cantidad", ajuste.cant);
                                    cmdSumar.Parameters.AddWithValue("@id", ajuste.id);
                                    cmdSumar.ExecuteNonQuery();
                                }
                            }
                        }

                        //Actualizar cabecera
                        string queryFactura = @"UPDATE Facturas 
                                        SET metodoPago = @pago, descuento = @descuento, estado = @estado, 
                                            subtotal = @subtotal, montoDescuento = @montoDesc, 
                                            baseGravable = @base, iva = @iva, total = @total 
                                        WHERE numeroFactura = @numero";

                        using (var cmdF = new SqlCommand(queryFactura, conexion, transaccion))
                        {
                            cmdF.Parameters.AddWithValue("@numero", numero);
                            cmdF.Parameters.AddWithValue("@pago", factura.metodoPago);
                            cmdF.Parameters.AddWithValue("@descuento", factura.descuento);
                            cmdF.Parameters.AddWithValue("@estado", factura.estado);
                            cmdF.Parameters.AddWithValue("@subtotal", factura.subtotal);
                            cmdF.Parameters.AddWithValue("@montoDesc", factura.montoDescuento);
                            cmdF.Parameters.AddWithValue("@base", factura.baseGravable);
                            cmdF.Parameters.AddWithValue("@iva", factura.iva);
                            cmdF.Parameters.AddWithValue("@total", factura.total);
                            cmdF.ExecuteNonQuery();
                        }

                        string queryBorrar = "DELETE FROM DetallesFactura WHERE numeroFactura = @numero";
                        using (var cmdDel = new SqlCommand(queryBorrar, conexion, transaccion))
                        {
                            cmdDel.Parameters.AddWithValue("@numero", numero);
                            cmdDel.ExecuteNonQuery();
                        }

                        //Sobreescrubir detalles y actualizar inventario
                        string queryDetalle = @"INSERT INTO DetallesFactura (Id, numeroFactura, ProductoId, unidadMedida, cantidad, precioUnitario, subtotal) 
                                        VALUES (@Id, @numero, @producto, @unidad, @cantidad, @precio, @subtotal)";

                        foreach (var detalle in factura.detalles)
                        {
                            Guid productoId = Guid.Empty;
                            string queryProducto = @"
                        SELECT TOP 1 Id FROM Productos 
                        WHERE LTRIM(RTRIM(nombre)) = LTRIM(RTRIM(@nombre)) COLLATE SQL_Latin1_General_CP1_CI_AS";

                            using (var cmdP = new SqlCommand(queryProducto, conexion, transaccion))
                            {
                                cmdP.Parameters.AddWithValue("@nombre", detalle.producto);
                                var res = cmdP.ExecuteScalar();
                                if (res != null) productoId = (Guid)res;
                            }

                            if (productoId == Guid.Empty)
                                throw new Exception($"El producto '{detalle.producto}' no existe en el inventario.");

                            using (var cmdD = new SqlCommand(queryDetalle, conexion, transaccion))
                            {
                                cmdD.Parameters.AddWithValue("@Id", Guid.NewGuid());
                                cmdD.Parameters.AddWithValue("@numero", factura.numeroFactura);
                                cmdD.Parameters.AddWithValue("@producto", productoId);
                                cmdD.Parameters.AddWithValue("@unidad", detalle.unidadMedida);
                                cmdD.Parameters.AddWithValue("@cantidad", detalle.cantidad);
                                cmdD.Parameters.AddWithValue("@precio", detalle.precioUnitario);
                                cmdD.Parameters.AddWithValue("@subtotal", detalle.cantidad * detalle.precioUnitario);
                                cmdD.ExecuteNonQuery();
                            }

                            //Actualizar inventario
                            if (factura.estado == "Pagada")
                            {
                                string queryRestar = "UPDATE Productos SET existenciaActual = existenciaActual - @cantidad WHERE Id = @id";
                                using (var cmdRestar = new SqlCommand(queryRestar, conexion, transaccion))
                                {
                                    cmdRestar.Parameters.AddWithValue("@cantidad", detalle.cantidad);
                                    cmdRestar.Parameters.AddWithValue("@id", productoId);
                                    cmdRestar.ExecuteNonQuery();
                                }
                            }
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al actualizar factura: " + ex.Message);
                    }
                }
            }
        }

        public List<Factura> obtenerVentas()
        {
            var lista = new List<Factura>();

            //Calcular el lunes y domingo 
            int diferencia = (7 + (DateTime.Now.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime lunes = DateTime.Now.AddDays(-1 * diferencia).Date;
            DateTime domingo = lunes.AddDays(6).Date;

            using (var conexion = new SqlConnection(conexionString))
            {
                //Consulta con filtro (fecha, estado y ordenamiento)
                string consulta = @"
                SELECT f.numeroFactura, f.fecha, c.nombre AS nombreCliente, e.nombre AS empleadoResponsable, 
                    f.subtotal, f.montoDescuento, f.iva, f.total
                FROM Facturas f
                INNER JOIN Clientes c ON f.ClienteId = c.Id
                INNER JOIN Empleados e ON f.EmpleadoId = e.Id
                WHERE f.estado = 'Pagada' 
                    AND f.fecha >= @inicioSemana 
                    AND f.fecha <= @finSemana
                ORDER BY f.total DESC";

                using (var cmd = new SqlCommand(consulta, conexion))
                {
                    cmd.Parameters.AddWithValue("@inicioSemana", lunes);
                    cmd.Parameters.AddWithValue("@finSemana", domingo);
                    conexion.Open();
                    using (var lector = cmd.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Factura
                            {
                                numeroFactura = lector["numeroFactura"].ToString(),
                                fecha = (DateTime)lector["fecha"],
                                nombreCliente = lector["nombreCliente"].ToString(),
                                empleadoResponsable = lector["empleadoResponsable"].ToString(),
                                subtotal = Convert.ToDecimal(lector["subtotal"]),
                                montoDescuento = Convert.ToDecimal(lector["montoDescuento"]),
                                iva = Convert.ToDecimal(lector["iva"]),
                                total = Convert.ToDecimal(lector["total"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Anular(string numero)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        //Buscar el estado
                        string estadoActual = "";
                        string queryEstado = "SELECT estado FROM Facturas WHERE numeroFactura = @numero";

                        using (var cmdEstado = new SqlCommand(queryEstado, conexion, transaccion))
                        {
                            cmdEstado.Parameters.AddWithValue("@numero", numero.Trim());
                            var res = cmdEstado.ExecuteScalar();

                            if (res == null) throw new Exception("La factura no existe en la base de datos.");
                            estadoActual = res.ToString().Trim();
                        }

                        //Validar que no este anulada
                        if (estadoActual == "Anulada") throw new Exception("La factura ya se encuentra anulada.");

                        //Devolver el inventario si estaba pagada
                        if (estadoActual == "Pagada")
                        {
                            var ajustes = new List<(Guid id, int cant)>();
                            string queryStock = @"SELECT d.ProductoId, d.cantidad FROM DetallesFactura d WHERE d.numeroFactura = @numero";

                            using (var cmdStock = new SqlCommand(queryStock, conexion, transaccion))
                            {
                                cmdStock.Parameters.AddWithValue("@numero", numero.Trim());
                                using (var lector = cmdStock.ExecuteReader())
                                {
                                    while (lector.Read())
                                    {
                                        ajustes.Add(((Guid)lector["ProductoId"], Convert.ToInt32(lector["cantidad"])));
                                    }
                                }
                            }

                            foreach (var ajuste in ajustes)
                            {
                                string querySumar = "UPDATE Productos SET existenciaActual = existenciaActual + @cantidad WHERE Id = @id";
                                using (var cmdSumar = new SqlCommand(querySumar, conexion, transaccion))
                                {
                                    cmdSumar.Parameters.AddWithValue("@cantidad", ajuste.cant);
                                    cmdSumar.Parameters.AddWithValue("@id", ajuste.id);
                                    cmdSumar.ExecuteNonQuery();
                                }
                            }
                        }

                        //Actualizar el estado de la factura a "Anulada"
                        string queryUpdate = "UPDATE Facturas SET estado = 'Anulada' WHERE numeroFactura = @numero";
                        using (var cmdUpdate = new SqlCommand(queryUpdate, conexion, transaccion))
                        {
                            cmdUpdate.Parameters.AddWithValue("@numero", numero.Trim());
                            cmdUpdate.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}