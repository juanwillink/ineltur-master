using System;
using System.Linq;
using System.Web.Services;
using System.Collections.Generic;
using System.Net.Mail;
using Ineltur.Datos.Entidades;
using Ineltur.WebService.Clases;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarDestinos")]
        public RespuestaCancelarReservaAlojamiento CancelarReservaAlojamiento(PeticionCancelarReservaAlojamiento peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio CancelarReservaAlojamiento");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaCancelarReservaAlojamiento()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    Transaccion reserva = null;

                    if (peticion.CodigoReserva != 0)
                        reserva = dc.Transacciones.SingleOrDefault(t => t.CodigoReserva == peticion.CodigoReserva && t.IdUsuario == idUsuario);
                    else
                        reserva = dc.Transacciones.SingleOrDefault(t => t.IdTransaccion == peticion.IdReserva && t.IdUsuario == idUsuario);

                    if (reserva.ReservaUnidades.Min(ru => ru.FechaInicial) < DateTime.Today.AddDays(1))
                    {
                        var respuestaError = new RespuestaCancelarReservaAlojamiento()
                        {
                            Estado = EstadoRespuesta.OperacionFallida
                        };

                        Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);
                        
                        if(reserva != null)
                            Tracker.WriteTrace(operationNumber, "Error CancelarReservaAlojamiento: Estado " + reserva.EstadoReserva.ToString(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    if (reserva == null || reserva.EstadoReserva.PerteneceA(2))
                    {
                        var respuestaError = new RespuestaCancelarReservaAlojamiento()
                        {
                            Estado = EstadoRespuesta.OperacionFallida
                        };

                        Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        if (reserva != null)
                            Tracker.WriteTrace(operationNumber, "Error CancelarReservaAlojamiento: Estado " + reserva.EstadoReserva.ToString(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    peticion.IdReserva = reserva.IdTransaccion;

                    int? error;
                   
                    if (reserva.IdFormaPago == Guid.Parse("7d5192ca-Fe10-455e-b051-d1023a07ba75") && reserva.EstadoReserva != -3)
                    {
                        error = null;
                        Tracker.WriteTrace("Actualizando estado de transaccion a 'A cancelar'", traceType: Tracker.TraceType.Info);
                        dc.updateTransaccion(
                            reserva.IdTransaccion,
                            reserva.IdFormaPago,
                            reserva.IdUsuario,
                            reserva.IdCliente,
                            reserva.Descripcion,
                            reserva.MontoTotalConDescuento,
                            -3,
                            reserva.Activo,
                            reserva.CodigoConfirmacion,
                            reserva.MontoTotalSinDescuento,
                            reserva.Cotizacion,
                            reserva.IdMoneda,
                            reserva.IdPU,
                            reserva.CantidadCuotas,
                            reserva.EstadoPago,
                            reserva.IdAlojamiento,
                            reserva.IdSitioOrigen,
                            CalcularFechaVencimiento(reserva.ReservaUnidades[0].FechaInicial, int.Parse(reserva.Alojamiento.DiasCancelacionCargo.ToString())),
                            reserva.PasajeroNombre,
                            reserva.PasajeroApellido,
                            false,
                            null,
                            ref error
                            );
                        if (error.GetValueOrDefault() != 0 && error.GetValueOrDefault() != 2)
                        {
                            var respuestaError = new RespuestaCancelarReservaAlojamiento()
                            {
                                Estado = EstadoRespuesta.OperacionFallida
                            };
                            Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);
                            Tracker.WriteTrace(operationNumber, "Error CancelarReservaAlojamiento: " + error.ToString(), traceType: Tracker.TraceType.Fatal);

                            return respuestaError;
                        }
                    }
                    else if (reserva.IdFormaPago == Guid.Parse("b8b3354a-4cd1-47dc-8267-707fd80d3072") && DateTime.Now.Date < reserva.ReservaUnidades.Min().FechaInicial.AddDays(-int.Parse(reserva.Alojamiento.DiasCancelacionCargo.ToString())))
                    {
                        Tracker.WriteTrace(operationNumber, "Cancelando Transacción...");
                        dc.CancelarTransaccion(reserva.IdTransaccion, out error);
                        if (error.GetValueOrDefault() != 0)
                        {
                            var respuestaError = new RespuestaCancelarReservaAlojamiento()
                            {
                                Estado = EstadoRespuesta.OperacionFallida
                            };

                            Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);
                            Tracker.WriteTrace(operationNumber, "Error CancelarReservaAlojamiento: " + error.ToString(), traceType: Tracker.TraceType.Fatal);

                            return respuestaError;
                        }
                    }
                    //Tracker.WriteTrace(operationNumber, "Obteniendo movimiento en Cuenta Corriente...");
                    //var mov = dc.MovimientosCuentaCorriente.FirstOrDefault(m => (m.IdTransaccion ?? Guid.Empty) == reserva.IdTransaccion);

                    //if (mov != null)
                    //{
                    //    Guid? tmpGuid;
                    //    DateTime? tmpDateTime;

                    //    //Tracker.WriteTrace(operationNumber, "Movimiento original: " + mov.ToStringWithProperties());

                    //    Tracker.WriteTrace(operationNumber, "Generando contra asiento en Cuenta Corriente...");
                    //    dc.AddMovimientoCtaCte(idUsuario, peticion.IdReserva, mov.IdMoneda, 0.0f,
                    //            mov.MontoDebe, mov.Cotizacion, out tmpGuid, out tmpDateTime);
                    //    Tracker.WriteTrace(operationNumber, "Asiento generado.");
                        
                    //    //dc.SubmitChanges();
                    //}

                    Tracker.WriteTrace(operationNumber, "Obteniendo Usuario...");
                    var usuario = dc.Usuarios.SingleOrDefault(u => u.IdUsuario == idUsuario.Value);
                    
                    var idioma = MapearIdioma(peticion.IdiomaDeseado);
                    
                    Tracker.WriteTrace(operationNumber, "Obteniendo Alojamiento...");
                    var alojamiento = dc.Alojamientos.SingleOrDefault(a => a.IdAlojamiento == reserva.IdAlojamiento.Value);
                    
                    Tracker.WriteTrace(operationNumber, "Obteniendo Pasajero...");
                    var pasajero = dc.PASAJEROs.SingleOrDefault(a => a.IDPASAJERO == (reserva.IdPasajero ?? Guid.Empty));

                    var nombrePasajero = reserva.PasajeroNombre ?? string.Empty;
                    var apellidoPasajero = reserva.PasajeroApellido ?? string.Empty;

                    if (pasajero != null)
                    {
                        //nombrePasajero = pasajero.NOMBRE;
                        //apellidoPasajero = pasajero.APELLIDO;
                    }

                    Tracker.WriteTrace(operationNumber, "Creando Cliente SMTP...");
                    using (var smtp = ObtenerClienteSmtp())
                    {
                        var from = Config.LeerSetting("MailAccountFrom");
                        var subject = idioma == "es" ? string.Format("Cancelación de Reserva en {0}", alojamiento.Nombre) : string.Format("Cancellation of Reserve at {0}", alojamiento.Nombre);
                        var idTransaccionString = reserva.IdTransaccion.ToString().Trim('{', '}').ToUpper();

                        var model = new
                            {
                                NombreCliente = String.Format("{0} {1}", nombrePasajero, apellidoPasajero),
                                CodigoReserva = reserva.CodigoReserva,
                                IdTransaccion = idTransaccionString,
                                CodigoConfirmacion = idTransaccionString.Substring(idTransaccionString.Length - 4, 4),
                                Agencia = String.Format("{0} {1}", usuario.Nombre, usuario.Apellido),
                                Alojamiento = alojamiento.Nombre,
                                LogoAgencia = usuario.UrlLogo ?? string.Empty,
                                Observaciones = reserva.Descripcion ?? string.Empty,
                                PoliticasCancelacion = alojamiento.PoliticasCancelacion,

                                Moneda = reserva.Moneda,
                                Total = reserva.MontoTotalSinDescuento,

                                Detalles = reserva.ReservaUnidades.GroupBy(u => u.IdUnidadAlojamiento).Select(g => new DetalleReserva()
                                {
                                    Descripcion = g.First().UnidadAlojamiento.Descripcion,
                                    FechaInicial = g.Min(u => u.FechaInicial),
                                    FechaFinal = g.Max(u => u.FechaFinal),
                                    PorUnidad = (decimal)g.Average(u => u.Monto) / (g.Max(u => u.FechaFinal) - g.Min(u => u.FechaInicial)).Days,
                                    Subtotal = (decimal)g.Sum(u => u.Monto),
                                    Cantidad = g.Count()
                                }).OrderBy(u => u.FechaInicial).ToArray(),
                                CantidadPasajeros = reserva.ReservaUnidades.Select(u => u.RESERVA_UNIDAD_PASAJEROs.Count()).ToArray()
                            };

                        //Tracker.WriteTrace(operationNumber, "Preparando datos para enviar mails: " + model.ToStringWithProperties());

                        if (reserva.IdFormaPago == Guid.Parse("7d5192ca-Fe10-455e-b051-d1023a07ba75"))
                        {
                            Tracker.WriteTrace(operationNumber, "Enviando mails notificando que la reserva 'Efectiva' DESEA ser cancelada");
                            Tracker.WriteTrace(operationNumber, "Enviando mail a agencia...");

                            FluentEmail.Email
                                .From(from)
                                .Subject(subject)
                                .To(usuario.Email.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/CancelacionReservaACancelar.cshtml"), model)
                                .UsingClient(smtp)
                                .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail a administración...");

                            FluentEmail.Email
                                .From(from)
                                .Subject(subject)
                                .To(new List<MailAddress>
                                    {
                                    new MailAddress(Config.LeerSetting("MailReservas")),
                                    new MailAddress(Config.LeerSetting("MailReservas2"))
                                    })
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/CancelacionReservaACancelar.cshtml"), model)
                                .UsingClient(smtp)
                                .Send();
                        }
                        else if (reserva.IdFormaPago == Guid.Parse("b8b3354a-4cd1-47dc-8267-707fd80d3072") && DateTime.Now.Date < reserva.ReservaUnidades.Min().FechaInicial.AddDays(-int.Parse(reserva.Alojamiento.DiasCancelacionCargo.ToString())))
                        {
                            Tracker.WriteTrace(operationNumber, "Enviando mail a agencia...");

                            FluentEmail.Email
                                .From(from)
                                .Subject(subject)
                                .To(usuario.Email.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/CancelacionReserva.cshtml"), model)
                                .UsingClient(smtp)
                                .Send();

                            //FluentEmail.Email
                            //    .From(from)
                            //    .Subject(subject)
                            //    .To(pasajero.EMAIL)
                            //    .UsingTemplateFromFile(String.Format("~/Views/MailTemplates/CancelacionReserva.{0}.cshtml", idioma), model)
                            //    .UsingClient(smtp)
                            //    .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail a administración...");

                            FluentEmail.Email
                                .From(from)
                                .Subject(subject)
                                .To(new List<MailAddress>
                                    {
                                    new MailAddress(Config.LeerSetting("MailReservas")),
                                    new MailAddress(Config.LeerSetting("MailReservas2"))
                                    })
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/CancelacionReserva.cshtml"), model)
                                .UsingClient(smtp)
                                .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail al hotel...");

                            FluentEmail.Email
                                .From(from)
                                .Subject(subject)
                                .To(reserva.Alojamiento.Email.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/CancelacionReservaParaAlojamiento.{0}.cshtml", idioma), model)
                                .UsingClient(smtp)
                                .Send();
                        }
                    }

                    var respuesta = new RespuestaCancelarReservaAlojamiento()
                    {
                        Estado = EstadoRespuesta.Ok
                    };

                    Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: Éxito");
                    Tracker.WriteTrace(operationNumber, "Respuesta: " + respuesta.ToStringWithProperties());

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaCancelarReservaAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin CancelarReservaAlojamiento: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }
    }
}