using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using Ineltur.Datos;
using System.Net.Mail;
using Ineltur.WebService.Clases;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod]
        [VisibleFromWSDL(true)]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarDestinos")]
        public RespuestaReservarAlojamiento ReservarAlojamiento(PeticionReservarAlojamiento peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio ReservarAlojamiento");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            if (peticion.Titular == null || peticion.Unidades.Count(u => u.Pasajeros == null || u.Pasajeros.Count() == 0) > 0)
            {
                var respuestaError = new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = "Titular/Pasajeros"
                };

                Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties(), traceType:Tracker.TraceType.Error);

                return respuestaError;
            }

            var validate = ValidatePasajero(peticion.Titular, -1);
            if (validate != null)
            {
                Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + validate.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                return validate;
            }

            var pasajerosList = new List<InfoPasajero>();
            
            pasajerosList.Add(peticion.Titular);
            peticion.Unidades.ToList().ForEach(u => pasajerosList.AddRange(u.Pasajeros));

            foreach (var pasajero in pasajerosList)
            {
                if (pasajero.Nombre == null)
                {
                    pasajero.Nombre = peticion.Titular.Nombre;
                }
                if (pasajero.Apellido == null)
                {
                    pasajero.Apellido = peticion.Titular.Apellido;
                }
                if (pasajero.Pais == null)
                {
                    pasajero.Pais = peticion.Titular.Pais;
                }
                if (pasajero.Documento == null)
                {
                    pasajero.TipoDocumento = peticion.Titular.TipoDocumento;
                    pasajero.Documento = peticion.Titular.Documento;
                }
                if (pasajero.FechaNacimiento < new DateTime(1850, 1, 1))
                {
                    pasajero.FechaNacimiento = peticion.Titular.FechaNacimiento;
                }
            }

            foreach(var pasajero in pasajerosList)
            {
                validate = ValidatePasajero(pasajero, -1);
                if (validate != null)
                {
                    Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + validate.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                    return validate;
                }
            }

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    var usuario = dc.Usuarios.SingleOrDefault(u => u.IdUsuario == idUsuario.Value);
                    var alojamiento = dc.Alojamientos.SingleOrDefault(a => a.IdAlojamiento == peticion.IdAlojamiento);

                    if (alojamiento == null)
                    {
                        Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: Alojamiento no encontrado.", traceType: Tracker.TraceType.Error);

                        var respuestaError = new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };

                        Tracker.WriteTrace(operationNumber, "Respuesta: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    if (peticion.Unidades == null || peticion.Unidades.Length == 0)
                    {
                        var respuestaError = new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "Unidades"
                        };

                        Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    DateTime fechaInicio = peticion.Unidades.Min(u => u.FechaInicio.Date);
                    DateTime fechaFin = peticion.Unidades.Max(u => u.FechaFin.Date);

                    if (fechaInicio == null || fechaInicio < DateTime.Today)
                    {
                        return new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaInicio"
                        };
                    }

                    if (fechaFin == null || fechaInicio >= fechaFin)
                    {
                        return new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaFin"
                        };
                    }

                    string idioma = MapearIdioma(peticion.IdiomaDeseado);
                    Guid? idTransaccion = null;
                    int? error = 0;
                    string nombreUsuario;
                    Random rnd = new Random();
                    
                    var pasajerosIdPasajero = new Dictionary<InfoPasajero, Guid>();
                    
                    Guid? idPasajero = null;
                    Guid documento = Guid.Empty;
                    int intentos = 5;

                    Tracker.WriteTrace(operationNumber, "Agregar Pasajeros a la BD o cargar los existentes...");

                    foreach (var pax in pasajerosList)
                    {
                        var pais = dc.Paises.SingleOrDefault(p => p.Descripcion == pax.Pais);

                        if (pais == null)
                            pais = dc.Paises.SingleOrDefault(p => p.Descripcion == "AR");

                        switch (pax.TipoDocumento)
                        {
                            case TipoDocumento.CUIT: documento = CUIT; break;
                            case TipoDocumento.DNI: documento = DNI; break;
                            //case TipoDocumento.Pasaporte: documento = Pasaporte; break;
                            default: documento = Pasaporte; break;//AM
                        }

                        while (intentos-- > 0)
                        {
                            try
                            {
                                Tracker.WriteTrace(operationNumber, string.Format("Buscando Pasajero: {0}, Tipo Documento: {1}", pax.Documento, pax.TipoDocumento));
                                if (string.IsNullOrEmpty(pax.Documento))//AM
                                {
                                    pax.Documento = Guid.NewGuid().ToString();
                                    Tracker.WriteTrace(operationNumber, string.Format("NroDoc asignado aleatorio: {0}, Tipo Documento: {1}", pax.Documento, pax.TipoDocumento));
                                }

                                var pasajeros = dc.PASAJEROs.Where(p => p.NRODOCUMENTO.ToUpper().Trim().Equals(pax.Documento.ToUpper().Trim()) &&
                                    p.IDTIPODOCUMENTO == documento);

                                if (pasajeros != null && pasajeros.Count() > 0)
                                {
                                    var pasajero = pasajeros.First();
                                    idPasajero = pasajero.IDPASAJERO;
                                    pasajerosIdPasajero.Add(pax, idPasajero.Value);

                                    pasajero.NOMBRE = pax.Nombre;
                                    pasajero.APELLIDO = pax.Apellido;

                                    dc.SubmitChanges();

                                    Tracker.WriteTrace(operationNumber, string.Format("Pasajero encontrado con ID: ", idPasajero));

                                    break;
                                }
                                else
                                {
                                    Tracker.WriteTrace(operationNumber, "Pasajero no encontrado, guardando en BD.");

                                    nombreUsuario = String.Format("u{0}{1}", Utiles.Base36(
                                            (DateTime.UtcNow.Ticks - TicksOffset) / 10000L),
                                            rnd.Next(36 * 36 * 36));
                                    dc.AddPasajero(null, pais != null ?
                                            pais.IdPais : Guid.Empty, null, pax.Ciudad, pax.Nombre.Trim(),
                                            pax.Apellido.Trim(), pax.Direccion, pax.FechaNacimiento,
                                            pax.Telefono, null, string.IsNullOrWhiteSpace(pax.Email) ? "a@a.com" : pax.Email.ToLower().Trim(),
                                            nombreUsuario, pax.Sexo == Sexo.Femenino, nombreUsuario,
                                            false, documento, pax.Documento.Trim(),
                                            ref idPasajero);

                                    pasajerosIdPasajero.Add(pax, idPasajero.Value);

                                    Tracker.WriteTrace(operationNumber, string.Format("Pasajero guardado con ID: ", idPasajero.Value));

                                    if (error.GetValueOrDefault() == 0) break;
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        if (error.GetValueOrDefault() == 1 || !idPasajero.HasValue)
                        {
                            var respuestaError = new RespuestaReservarAlojamiento()
                            {
                                Estado = EstadoRespuesta.OperacionFallida
                            };

                            Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento (No pudo guardarse el pasajero): " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                            return respuestaError;
                        }
                    }

                    idPasajero = pasajerosIdPasajero[peticion.Titular];
                    var unidades = peticion.Unidades.Select(u => u.IdUnidad).Distinct().ToArray();
                    var disponibles = dc.GetCuposAlojamientoEnRangoFechaV3(1, 1, 1, 1, 1, 1,
                            fechaInicio, fechaFin, peticion.IdAlojamiento, "arg", peticion.Desayuno, peticion.TarifaReembolsable).Where(
                            d => unidades.Contains(d.IDUNIDAD_ALOJ)).ToList();
                    bool satisfecho = true;
                    decimal total = 0.0m;
                    var detalles = new List<DetalleReserva>();

                    //1.Cambiar markup teniendo en cuenta cotizaciones de ambas monedas
                    float cotizMonedaUsuario = ObtenerCotizacion(dc, usuario.IdMoneda); //Agregado por AM
                    float cotizMonedaAloj = ObtenerCotizacion(dc, (Guid)alojamiento.Moneda.IdMoneda); //Agregado por AM

                    var markupYCotiz = cotizMonedaAloj / ((1 - (usuario.MarkupAAgencia) / 100) * (1 - (usuario.MarkupAConsumidorFinal) / 100) * cotizMonedaUsuario);

                    Tracker.WriteTrace(operationNumber, "Guardando Transacción en BD...");

                    int diasCancelacionCargo = 0;
                    if (alojamiento.DiasCancelacionCargo != null)
                    {
                        diasCancelacionCargo = int.Parse(alojamiento.DiasCancelacionCargo.ToString());
                    }

                    //2.Modificar para que guarde como idMoneda y cotización las del usuario, no las del hotel
                    dc.AddTransaccion(peticion.IdAlojamiento, (peticion.IdFormaPago != Guid.Empty ? peticion.IdFormaPago : FormaPago), idUsuario, null,
                            peticion.Observaciones, null, null, null, null, -1, true, null, null,
                            cotizMonedaUsuario, usuario.IdMoneda, WebServiceSitio, 0,
                            CalcularFechaVencimiento(peticion.Unidades.First().FechaInicio, diasCancelacionCargo), peticion.Titular.Nombre, peticion.Titular.Apellido, false, idPasajero,
                            out idTransaccion);

                    Tracker.WriteTrace(operationNumber, string.Format("Transacción guardada con ID: {0}", idTransaccion));

                    foreach (var detalle in peticion.Unidades)
                    {
                        Tracker.WriteTrace(operationNumber, string.Format("Comprobando disponibilidad de Unidad con ID: {0}", detalle.IdUnidad));

                        DateTime inicio = detalle.FechaInicio.Date;
                        DateTime fin = detalle.FechaFin.Date;

                        //Determina la cantidad de unidades de ese tipo
                        int cantidad = detalle.Cantidad;
                            
                        for (DateTime fi = inicio, ff = fi + UnDia; fi < fin; fi = ff, ff += UnDia)
                        {
                            //Toma la disponibilidad de unidades para esa fecha
                            var disponible = disponibles.FirstOrDefault(d => d.FECHA == fi &&
                                    d.IDUNIDAD_ALOJ == detalle.IdUnidad);

                            //Si no hay disponibilidad o es menor que la solicitada, sale
                            if (disponible == null || (disponible.CUPODISPONIBLE ?? 0) < cantidad)
                                satisfecho = false;

                            if (!satisfecho) break;
                        }

                        if(satisfecho)
                        {
                            Tracker.WriteTrace(operationNumber, string.Format("Disponibilidad satisfecha", detalle.IdUnidad));

                            cantidad = detalle.Cantidad;
                            
                            var disponible = disponibles.FirstOrDefault(d => d.FECHA == inicio &&
                                    d.IDUNIDAD_ALOJ == detalle.IdUnidad);
                            
                            int dias = (fin - inicio).Days;

                            //Mientras todavía no se ha reservado la cantidad solicitada
                            while (cantidad-- > 0)
                            {
                                Guid? idReserva = Guid.Empty;

                                Tracker.WriteTrace(operationNumber, "Guardando Reserva de Unidad en BD...");

                                if (peticion.TienePromocion == true)
                                {
                                    dc.AddReservaUnidad(detalle.IdUnidad, idTransaccion, inicio, fin, (float)peticion.PrecioPromocional, true,
                                        ref error, ref idReserva);
                                } else
                                {
                                    dc.AddReservaUnidad(detalle.IdUnidad, idTransaccion, inicio, fin,
                                        disponible.MONTOPROMEDIOPORDIA * dias * markupYCotiz, true,
                                        ref error, ref idReserva);
                                }
                                Tracker.WriteTrace(operationNumber, string.Format("Reserva de Unidad guardada con ID: {0}", idReserva));

                                var count = 0;
                                foreach (var px in detalle.Pasajeros)
                                {
                                    //Soluciona error de no poder asociar dos pasajeros a una reserva. -Juan Willink 21/12/2016
                                    Tracker.WriteTrace(operationNumber, string.Format("Asociando Pasajero {0} a la Reserva de Unidad...", pasajerosIdPasajero[px]));
                                    if (count < 0)
                                    {
                                        dc.AddReservaUnidadPasajero(idReserva, pasajerosIdPasajero[px]);
                                    }
                                    count++;
                                }

                                if (error.GetValueOrDefault() != 0)
                                {
                                    satisfecho = false;
                                    break;
                                }
                            }

                            Tracker.WriteTrace(operationNumber, "Reservas de Unidad guardadas en BD");

                            decimal porDia = Decimal.Round((decimal)((disponible.MONTOPROMEDIOPORDIA ?? 0.0f) * markupYCotiz), 2);

                            var detalleUnidad = new DetalleReserva()
                            {
                                Descripcion = disponible.NOMBRE,
                                FechaInicial = inicio,
                                FechaFinal = fin,
                                Cantidad = detalle.Cantidad,
                                Dias = dias,
                                PorUnidad = porDia,
                                Subtotal = porDia * detalle.Cantidad * dias
                            };

                            detalles.Add(detalleUnidad);
                            total += detalleUnidad.Subtotal;

                            Tracker.WriteTrace(operationNumber, "Detalle de Unidad actualizado: " + detalleUnidad.ToStringWithProperties());
                        }
                    }

                    if (!satisfecho)
                    {
                        var respuestaError = new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.NoHayCupo
                        };

                        Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    var estadoReserva = (int)EstadoReserva.ReservaEfectiva;

                    if (peticion.IdFormaPago == Guid.Parse("b8b3354a-4cd1-47dc-8267-707fd80d3072"))
                    {
                        estadoReserva = (int)EstadoReserva.ReservaAConstatar;
                    }

                    // Si el alojamiento es de tipo Simple, la reserva será Bajo Petición
                    //if (alojamiento.IdTipoPerfil == Guid.Parse("DDEAB0FD-6515-4788-9034-40C1888459C4"))
                    //    // Se envía un mail a la administración para que procesen la reserva con el Alojamiento
                    //    estadoReserva = (int)EstadoReserva.ReservaAConstatar;

                    Guid? tmpGuid;
                    DateTime? tmpDateTime;

                    if (usuario.UsuarioWebService.FormaPago == Datos.FormaPago.Cuenta && peticion.IdFormaPago == Guid.Parse("7d5192ca-fe10-455e-b051-d1023a07ba75"))
                    {
                        Tracker.WriteTrace(operationNumber, "Confirmando la Transacción y Agregando Movimiento en Cta. Cte. en BD...");

                        dc.ConfirmarTransaccion(idTransaccion, idUsuario, estadoReserva, out error);
                        //3.Verificar si debo cambiar alojamiento.IdMoneda y alojamiento.Cotizacion por usuario.IdMoneda y usuario.Cotizacion
                        if (peticion.TienePromocion)
                        {
                            total = peticion.PrecioPromocional;
                        }
                        dc.AddMovimientoCtaCte(idUsuario, idTransaccion, usuario.IdMoneda,
                                (float?)total, 0.0f, cotizMonedaUsuario, out tmpGuid, out tmpDateTime);

                        Tracker.WriteTrace(operationNumber, string.Format("Movimiento en Cta. Cte. guardado con ID: {0}", tmpGuid));
                    }
                    else if (peticion.IdFormaPago == Guid.Parse("b8b3354a-4cd1-47dc-8267-707fd80d3072"))
                    {
                        Tracker.WriteTrace(operationNumber, "Confirmando la Transacción...");
                        dc.ConfirmarTransaccion(idTransaccion, idUsuario, estadoReserva, out error);
                    }
                    dc.SubmitChanges();

                    if (error != 0)
                    {
                        var respuestaError = new RespuestaReservarAlojamiento()
                        {
                            Estado = EstadoRespuesta.ErrorInterno,
                            MensajeEstado = string.Format("Código: ", error)
                        };

                        Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    var transaccion = dc.Transacciones.Single(t => t.IdTransaccion == idTransaccion.Value);
                    var respuesta = new RespuestaReservarAlojamiento()
                    {
                        Estado = EstadoRespuesta.Ok,
                        IdReserva = idTransaccion.GetValueOrDefault(),
                        CodigoReserva = transaccion.CodigoReserva,

                        //4.Cambiar por usuario.IdMoneda
                        Moneda = ConvertirMoneda(usuario.IdMoneda),
                        Total = total,

                        VencimientoReserva = usuario.UsuarioWebService.FormaPago == Datos.FormaPago.Cuenta ? (DateTime?)null :
                                DateTime.Now.Date.AddDays(dc.Consultoras.First().TopeDiasDeposito.GetValueOrDefault(0)),

                        Detalles = detalles.ToArray()
                    };


                    using (var smtp = ObtenerClienteSmtp())
                    {
                        var idTransaccionString = transaccion.IdTransaccion.ToString().Trim('{', '}').ToUpper();

                        var model = new
                        {
                            NombreCliente = String.Format("{0} {1}", peticion.Titular.Nombre, peticion.Titular.Apellido),
                            CodigoReserva = transaccion.CodigoReserva,
                            IdTransaccion = idTransaccionString,
                            CodigoConfirmacion = idTransaccionString.Substring(idTransaccionString.Length - 4, 4),
                            Agencia = String.Format("{0} {1}", usuario.Nombre, usuario.Apellido),
                            Alojamiento = alojamiento,
                            LogoAgencia = usuario.UrlLogo ?? string.Empty,
                            Observaciones = transaccion.Descripcion ?? string.Empty,
                            PoliticasCancelacion = alojamiento.PoliticasCancelacion,
                            IncurreGastos = peticion.IncurreGastos,
                            Moneda = respuesta.Moneda,
                            Total = respuesta.Total,
                            Detalles = detalles.ToArray(),
                            CantidadPasajeros = peticion.Unidades.Select(u => u.Pasajeros.Count()).ToArray(),
                            Pasajeros = peticion.Unidades.Select(u => u.Pasajeros).ToArray()
                        };

                        
                        
                        //Tracker.WriteTrace(operationNumber, "Preparando datos para enviar mails: " + model.ToStringWithProperties());

                        if (estadoReserva == (int)EstadoReserva.ReservaEfectiva)
                        {
                            Tracker.WriteTrace(operationNumber, "Enviando mail a agencia...");

                            FluentEmail.Email
                                .From(Config.LeerSetting("MailAccountFrom"))
                                .Subject(idioma == "es" ? string.Format("Reserva en {0}", alojamiento.Nombre) : string.Format("Reserve at {0}", alojamiento.Nombre))
                                .To(usuario.Email.Split(new char[] {';', ','}, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/Reserva.{0}.cshtml", idioma), model)
                                .UsingClient(smtp)
                                .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail a administración...");

                            FluentEmail.Email
                                .From(Config.LeerSetting("MailAccountFrom"))
                                .Subject(idioma == "es" ? string.Format("Reserva en {0}", alojamiento.Nombre) : string.Format("Reserve at {0}", alojamiento.Nombre))
                                .To(new List<MailAddress> 
                                    {
                                        new MailAddress(Config.LeerSetting("MailReservas")), 
                                        new MailAddress(Config.LeerSetting("MailReservas2"))
                                    })
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/Reserva.{0}.cshtml", idioma), model)
                                .UsingClient(smtp)
                                .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail al hotel...");

                            FluentEmail.Email
                                .From(Config.LeerSetting("MailAccountFrom"))
                                .Subject(idioma == "es" ? string.Format("Reserva en {0}", alojamiento.Nombre) : string.Format("Reserve at {0}", alojamiento.Nombre))
                                .To(transaccion.Alojamiento.Email.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/ReservaParaAlojamiento.{0}.cshtml", idioma), model)
                                .UsingClient(smtp)
                                .Send();
                        }
                        else if (estadoReserva == (int)EstadoReserva.ReservaAConstatar)
                        {
                            Tracker.WriteTrace(operationNumber, "Enviando mail a agencia...");

                            FluentEmail.Email
                                .From(Config.LeerSetting("MailAccountFrom"))
                                .Subject(idioma == "es" ? string.Format("Para Agencia - Reserva en {0}", alojamiento.Nombre) : string.Format("Reserve at {0}", alojamiento.Nombre))
                                .To(usuario.Email.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/ReservaAConstatar.cshtml", idioma), model)
                                .UsingClient(smtp)
                                .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail a administración...");

                            FluentEmail.Email
                                .From(Config.LeerSetting("MailAccountFrom"))
                                .Subject(idioma == "es" ? string.Format("Para Administracion - Reserva en {0}", alojamiento.Nombre) : string.Format("Reserve at {0}", alojamiento.Nombre))
                                .To(new List<MailAddress>
                                    {
                                        new MailAddress(Config.LeerSetting("MailReservas")),
                                        new MailAddress(Config.LeerSetting("MailReservas2"))
                                    })
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/ReservaAConstatar.cshtml"), model)
                                .UsingClient(smtp)
                                .Send();

                            Tracker.WriteTrace(operationNumber, "Enviando mail al hotel...");

                            FluentEmail.Email
                                .From(Config.LeerSetting("MailAccountFrom"))
                                .Subject(idioma == "es" ? string.Format("Para Hotel - Reserva en {0}", alojamiento.Nombre) : string.Format("Reserve at {0}", alojamiento.Nombre))
                                .To(transaccion.Alojamiento.Email.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(a => new MailAddress(a)).ToList())
                                .UsingTemplateFromFile(String.Format("~/PlantillasMails/ReservaParaAlojamiento.{0}.cshtml", idioma), model)
                                .UsingClient(smtp)
                                .Send();
                        }
                        else
                        {
                            var respuestaError = new RespuestaReservarAlojamiento()
                            {
                                Estado = EstadoRespuesta.ErrorInterno,
                                MensajeEstado = string.Format("Estado: {0}", (int)estadoReserva)
                            };

                            Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);
                            return respuestaError;
                        }
                    }

                    Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: Éxito");
                    Tracker.WriteTrace(operationNumber, "Respuesta: " + respuesta.ToStringWithProperties());

                    return respuesta;
                }

            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin ReservarAlojamiento: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }

        private RespuestaReservarAlojamiento ValidatePasajero(InfoPasajero pax, int i)
        {
            RespuestaReservarAlojamiento resultado = null;
            //pax.Pais = (pax.Pais ?? String.Empty).ToUpperInvariant();

            if (String.IsNullOrWhiteSpace(pax.Nombre))
            {
                return new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = string.Format("Pasajeros[{0}].Nombre", i)
                };
            }

            if (String.IsNullOrWhiteSpace(pax.Apellido))
            {
                return new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = string.Format("Pasajeros[{0}].Apellido", i)
                };
            }

            if (pax.FechaNacimiento != null && pax.FechaNacimiento < new DateTime(1850, 1, 1))
            {
                return new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = string.Format("Pasajeros[{0}].FechaNacimiento", i)
                };
            }

            //if (pax.TipoDocumento != TipoDocumento.Pasaporte && pax.Pais != "AR")
            //{
            //    return new RespuestaReservarAlojamiento()
            //    {
            //        Estado = EstadoRespuesta.ErrorParametro,
            //        MensajeEstado = string.Format("Pasajeros[{0}].TipoDocumento", i)
            //    };
            //}

            if (pax.TipoDocumento == null && pax.Pais == "AR")
            {
                return new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = string.Format("Pasajeros[{0}].TipoDocumento", i)
                };
            }

            if (String.IsNullOrWhiteSpace(pax.Documento) && pax.Pais == "AR")
            {
                return new RespuestaReservarAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = string.Format("Pasajeros[{0}].Documento", i)
                };
            }

            return resultado;
        }

        private DateTime CalcularFechaVencimiento(DateTime checkin, int diasCancelacionCargo)
        {
            var result = DateTime.Today.Date;
            var substract = -2 - diasCancelacionCargo;
            if (checkin.AddDays(substract) <= DateTime.Now.Date)
            {
                result = checkin.AddDays(-2);
            }
            else
            {
                result = checkin.AddDays(substract);
            }
            return result;
        }
    }
}