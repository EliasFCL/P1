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
            using (var conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                //Si algo falla se cancela todo
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        //Buscar id
                        Guid clienteId = Guid.Empty;
                        using (var cmdC = new SqlCommand("SELECT TOP 1 Id FROM Clientes WHERE nombre = @nombre", conexion, transaccion))
                        {
                            cmdC.Parameters.AddWithValue("@nombre", factura.clienteId);
                            var res = cmdC.ExecuteScalar();
                            if (res != null) clienteId = (Guid)res;
                        }

                        Guid empleadoId = Guid.Empty;
                        using (var cmdE = new SqlCommand("SELECT TOP 1 Id FROM Empleados WHERE nombre = @nombre", conexion, transaccion))
                        {
                            cmdE.Parameters.AddWithValue("@nombre", factura.empleadoId);
                            var res = cmdE.ExecuteScalar();
                            if (res != null) empleadoId = (Guid)res;
                        }

                        //Insertar la cabecera
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

                        //Insertar detalles
                        string queryDetalle = @"INSERT INTO DetallesFactura (Id, numeroFactura, ProductoId, cantidad, precioUnitario, subtotal) 
                                                VALUES (@Id, @numero, @producto, @cantidad, @precio, @subtotal)";

                        foreach (var detalle in factura.detalles)
                        {
                            // Buscar el Guid del producto por su nombre
                            Guid productoId = Guid.Empty;
                            using (var cmdP = new SqlCommand("SELECT TOP 1 Id FROM Productos WHERE nombre = @nombre", conexion, transaccion))
                            {
                                cmdP.Parameters.AddWithValue("@nombre", detalle.producto);
                                var res = cmdP.ExecuteScalar();
                                if (res != null) productoId = (Guid)res;
                            }

                            using (var cmdD = new SqlCommand(queryDetalle, conexion, transaccion))
                            {
                                cmdD.Parameters.AddWithValue("@Id", Guid.NewGuid()); // Nuevo Guid para el detalle
                                cmdD.Parameters.AddWithValue("@numero", factura.numeroFactura);
                                cmdD.Parameters.AddWithValue("@producto", productoId);
                                cmdD.Parameters.AddWithValue("@cantidad", detalle.cantidad);
                                cmdD.Parameters.AddWithValue("@precio", detalle.precioUnitario);
                                cmdD.Parameters.AddWithValue("@subtotal", detalle.subtotal);

                                cmdD.ExecuteNonQuery();
                            }
                        }

                        //Confirmar cambios
                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        //Cancelar todo si hay error
                        transaccion.Rollback();
                        return false;
                    }
                }
            }
        }


        public List<Factura> Listar()
        {
            var lista = new List<Factura>();
            using (var conexion = new SqlConnection(conexionString))
            {
                // JOIN para traer el nombre del cliente y del empleado
                string consulta = @"
                    SELECT f.numeroFactura, f.fecha, c.nombre AS nombreCliente, e.nombre AS empleadoResponsable, 
                           f.metodoPago, f.descuento, f.estado, f.subtotal, f.montoDescuento, f.baseGravable, f.iva, f.total
                    FROM Facturas f
                    INNER JOIN Clientes c ON f.ClienteId = c.Id
                    INNER JOIN Empleados e ON f.EmpleadoId = e.Id";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Factura
                            {
                                numeroFactura = lector["numeroFactura"].ToString(),
                                fecha = (DateTime)lector["fecha"],
                                clienteId = (Guid)lector["clienteId"],
                                empleadoId = (Guid)lector["empleadoId"],
                                metodoPago = lector["metodoPago"].ToString(),
                                descuento = Convert.ToInt32(lector["descuento"]),
                                estado = lector["estado"].ToString(),
                                subtotal = Convert.ToDecimal(lector["subtotal"]),
                                montoDescuento = Convert.ToDecimal(lector["montoDescuento"]),
                                baseGravable = Convert.ToDecimal(lector["baseGravable"]),
                                iva = Convert.ToDecimal(lector["iva"]),
                                total = Convert.ToDecimal(lector["total"])
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}