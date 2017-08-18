using System;
using System.Linq;
using System.Web.Services;
using Ineltur.Datos;
using Ineltur.Datos.Entidades;
using Ineltur.WebService.Clases;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarDestinos")]
        public RespuestaConsultarReservaAlojamiento ConsultarReservaAlojamiento(PeticionConsultarReservaAlojamiento peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio ConsultarReservaAlojamiento");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaConsultarReservaAlojamiento()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin ConsultarReservaAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    Transaccion reserva = null;
                    
                    if(peticion.CodigoReserva != 0)
                        reserva = dc.Transacciones.SingleOrDefault(t => t.CodigoReserva == peticion.CodigoReserva && t.IdUsuario == idUsuario);
                    else
                        reserva = dc.Transacciones.SingleOrDefault(t => t.IdTransaccion == peticion.IdReserva && t.IdUsuario == idUsuario);

                    // Si la reserva no está finalizada
                    if (reserva == null || reserva.EstadoReserva.PerteneceA(-1, -2))
                    {
                        return new RespuestaConsultarReservaAlojamiento()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };
                    }

                    peticion.IdReserva = reserva.IdTransaccion;

                    //var pax = reserva.Usuario;
                    var pax = dc.PASAJEROs.SingleOrDefault(p => p.IDPASAJERO == (reserva.IdPasajero ?? Guid.Empty));
                    TipoDocumento tipoDocumento;

                    if (pax != null)
                    {
                        if (pax.IDTIPODOCUMENTO == CUIT)
                        {
                            tipoDocumento = TipoDocumento.CUIT;
                        }
                        else if (pax.IDTIPODOCUMENTO == DNI)
                        {
                            tipoDocumento = TipoDocumento.DNI;
                        }
                        else if (pax.IDTIPODOCUMENTO == Pasaporte)
                        {
                            tipoDocumento = TipoDocumento.Pasaporte;
                        }
                        else
                        {
                            tipoDocumento = 0;
                        }

                        var respuesta = new RespuestaConsultarReservaAlojamiento()
                        {
                            Estado = EstadoRespuesta.Ok,

                            Pasajero = new InfoPasajero()
                            {
                                Nombre = reserva.PasajeroNombre,
                                Apellido = reserva.PasajeroApellido,
                                Sexo = pax.SEXO.GetValueOrDefault() ? Sexo.Femenino : Sexo.Masculino,
                                FechaNacimiento = pax.FECHA_NACIMIENTO,
                                TipoDocumento = tipoDocumento,
                                Documento = pax.NRODOCUMENTO,
                                Direccion = pax.DIRECCION,
                                Ciudad = pax.OTRACIUDAD,
                                Pais = pax.Pais.Descripcion,
                                Telefono = pax.TELEFONO,
                                Email = pax.EMAIL
                            },

                            Alojamiento = new InfoAlojamiento(),

                            // Sólo puede ser reserva a constatar, efectiva, rechazada o cancelada
                            EstadoReserva = (EstadoReserva)reserva.EstadoReserva,

                            Moneda = ConvertirMoneda(reserva.IdMoneda),
                            Total = reserva.MontoTotalSinDescuento.HasValue ?
                                Decimal.Round((decimal)reserva.MontoTotalSinDescuento.Value, 2) : (decimal?)null,

                            Detalles = reserva.ReservaUnidades.GroupBy(u => u.IdUnidadAlojamiento).Select(g => new DetalleReserva()
                            {
                                Descripcion = g.First().UnidadAlojamiento.Descripcion,
                                FechaInicial = g.Min(u => u.FechaInicial),
                                FechaFinal = g.Max(u => u.FechaFinal),
                                PorUnidad = (decimal)g.Average(u => u.Monto) / (g.Max(u => u.FechaFinal) - g.Min(u => u.FechaInicial)).Days,
                                Subtotal = (decimal)g.Sum(u => u.Monto),
                                Cantidad = g.Count()
                            }).OrderBy(u => u.FechaInicial).ToArray()
                        };

                        CargarInfoAlojamiento(reserva.Alojamiento, respuesta.Alojamiento, ObtenerMonedaUsuario(dc, (Guid) idUsuario));

                        Array.ForEach(respuesta.Detalles, d =>
                        {
                            d.Dias = (d.FechaFinal - d.FechaInicial).Days;
                            d.Cantidad /= d.Dias;
                        });

                        Tracker.WriteTrace(operationNumber, "Fin ConsultarReservaAlojamiento: Éxito");
                        Tracker.WriteTrace(operationNumber, "Respuesta: " + respuesta.ToStringWithProperties());

                        return respuesta;
                    }
                    else
                        throw new Exception("No se encontró el Pasajero.");
                }
            }
            catch (Exception ex)
            {
                Tracker.WriteTrace(operationNumber, "Fin ConsultarReservaAlojamiento: ErrorInterno\r\n" + ex.Message, true, Tracker.TraceType.Error);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return new RespuestaConsultarReservaAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };
            }
        }
    }
}