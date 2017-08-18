using System;
using System.Linq;
using System.Web.Services;
using Ineltur.WebService.Clases;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarAlojamientos")]
        public RespuestaBuscarAlojamientosEInfo BuscarAlojamientosEInfo(PeticionBuscarAlojamientos peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio BuscarAlojamientosEInfo");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            Guid? ciudad = null;
            Guid? provincia = null;

            switch (peticion.TipoDestino)
            {
                case TipoDestino.Ciudad:
                    ciudad = peticion.IdDestino;
                    break;

                case TipoDestino.Provincia:
                    provincia = peticion.IdDestino;
                    break;
            }

            //if (!ciudad.HasValue && !provincia.HasValue)
            //{
            //    return new RespuestaBuscarAlojamientosEInfo()
            //    {
            //        Estado = EstadoRespuesta.ErrorParametro,
            //        MensajeEstado = "TipoDestino"
            //    };
            //}

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaBuscarAlojamientosEInfo()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    if (peticion.FechaInicio == null || peticion.FechaInicio < DateTime.Today)
                    {
                        var respuestaError = new RespuestaBuscarAlojamientosEInfo()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaInicio"
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    if (peticion.FechaFin == null || peticion.FechaInicio >= peticion.FechaFin)
                    {
                        var respuestaError = new RespuestaBuscarAlojamientosEInfo()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaFin"
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    var usuario = dc.Usuarios.SingleOrDefault(u => u.IdUsuario == idUsuario);

                    var encontrados = dc.GetAlojamientosConDisponibilidadV2(
                        peticion.Habitacion1.GetValueOrDefault(), peticion.Habitacion2.GetValueOrDefault(),
                        peticion.Habitacion3.GetValueOrDefault(), peticion.Habitacion4.GetValueOrDefault(),
                        peticion.Habitacion5.GetValueOrDefault(), peticion.Habitacion6.GetValueOrDefault(),
                        peticion.FechaInicio, peticion.FechaFin, ConvertirOrdenamiento(peticion.Orden),
                        ciudad, provincia, ConvertirTipoAlojamiento(peticion.TipoAlojamiento), peticion.Nacionalidad, peticion.NombreAlojamiento)
                            .Where(a => a.montoTotalEstimadoEnPesos > 0).ToArray();

                    if (encontrados.Length == 0)
                    {
                        var respuestaError = new RespuestaBuscarAlojamientosEInfo()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }
                    else
                    {
                        var markup = 1 / ((1 - (usuario.MarkupAAgencia) / 100) * (1 - (usuario.MarkupAConsumidorFinal) / 100));

                        var cotizMonedaUsuario = ObtenerCotizacion(dc, usuario.IdMoneda); //Agregado por AM

                        var alojamientosDisponibles = encontrados.Select(a => new InfoAlojamientoDisponibleCompleta()
                            {
                                Destino = new InfoDestino()
                                {
                                    TipoDestino = TipoDestino.Ciudad,
                                    IdDestino = a.idCiudad.GetValueOrDefault(),
                                    NombreDestino = String.Concat(a.nombreCiudad, ", ", a.nombreProvincia)
                                },

                                //Alojamiento = InfoAlojamiento(new PeticionInfoAlojamiento()
                                //    {
                                //        Usuario = peticion.Usuario,
                                //        Clave = peticion.Clave,
                                //        IdiomaDeseado = peticion.IdiomaDeseado,
                                //        IdAlojamiento = a.IdAlojamiento.GetValueOrDefault()
                                //    }).Alojamiento,

                                Alojamiento = ObtenerInfoAlojamiento(dc.Alojamientos.SingleOrDefault(aloj => aloj.IdAlojamiento == a.idAloj), usuario.IdMoneda),

                                TipoAlojamiento = ConvertirTipoAlojamiento(a.idTipoAloj),
                                IdAlojamiento = a.idAloj.GetValueOrDefault(),
                                Nombre = a.nombre,
                                Moneda = ConvertirMoneda(usuario.IdMoneda),
                                
                                Cupo1 = a.cupoTotalDisponibleSingle,
                                Cupo2 = a.cupoTotalDisponibleDoble,
                                Cupo3 = a.cupoTotalDisponibleTriple,
                                Cupo4 = a.cupoTotalDisponibleCuadruple,
                                Cupo5 = a.cupoTotalDisponible5Personas,
                                Cupo6 = a.cupoTotalDisponible6Personas,

                                //Tarifa1 = Decimal.Round((decimal)(a.MontoMasBaratoPorDia1.GetValueOrDefault() * markup), 2),
                                //Tarifa2 = Decimal.Round((decimal)(a.MontoMasBaratoPorDia2.GetValueOrDefault() * markup), 2),
                                //Tarifa3 = Decimal.Round((decimal)(a.MontoMasBaratoPorDia3.GetValueOrDefault() * markup), 2),
                                //Tarifa4 = Decimal.Round((decimal)(a.MontoMasBaratoPorDia4.GetValueOrDefault() * markup), 2),
                                //Tarifa5 = Decimal.Round((decimal)(a.MontoMasBaratoPorDia5.GetValueOrDefault() * markup), 2),
                                //Tarifa6 = Decimal.Round((decimal)(a.MontoMasBaratoPorDia6.GetValueOrDefault() * markup), 2)

                                Tarifa1 = Decimal.Round((decimal)((a.montoUnidadSingleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa2 = Decimal.Round((decimal)((a.montoUnidadDobleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa3 = Decimal.Round((decimal)((a.montoUnidadTripleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa4 = Decimal.Round((decimal)((a.montoUnidadCuadrupleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa5 = Decimal.Round((decimal)((a.montoUnidad5PersonasMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa6 = Decimal.Round((decimal)((a.montoUnidad6PersonasMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2)

                            }).ToArray();

                        foreach (var alojamientoDisponible in alojamientosDisponibles)
                        {
                            //var respuestaInfoCupos = InfoCuposAlojamiento(new PeticionInfoCuposAlojamiento
                            //    {
                            //        Usuario = peticion.Usuario,
                            //        Clave = peticion.Clave,
                            //        IdiomaDeseado = peticion.IdiomaDeseado,
                            //        IdAlojamiento = alojamientoDisponible.IdAlojamiento.GetValueOrDefault(),
                            //        FechaInicio = peticion.FechaInicio,
                            //        FechaFin = peticion.FechaFin
                            //    });

                            //if (respuestaInfoCupos.Estado != EstadoRespuesta.Ok)
                            //{
                            //    return new RespuestaBuscarAlojamientosEInfo()
                            //    {
                            //        Estado = EstadoRespuesta.ErrorInterno,
                            //        MensajeEstado = respuestaInfoCupos.MensajeEstado
                            //    };
                            //}

                            var disponible = dc.GetCuposAlojamientoEnRangoFechaV2(1, 1, 1, 1, 1, 1,
                            peticion.FechaInicio, peticion.FechaFin, alojamientoDisponible.IdAlojamiento, peticion.Nacionalidad).OrderBy(
                            d => d.MONTOPROMEDIOPORDIA).Where(d => d.MONTOPROMEDIOPORDIA > 0).ToList();

                            var monedaAloj = ObtenerMoneda(dc, (Guid) alojamientoDisponible.IdAlojamiento);
                            var cotizMonedaAloj = ObtenerCotizacion(dc, (Guid) monedaAloj);
                            
                            alojamientoDisponible.Alojamiento.Unidades = disponible.Where(d => d.CUPODISPONIBLE.GetValueOrDefault() > 0)
                                                                        .Select(d => new InfoUnidad()
                                {
                                    IdUnidad = d.IDUNIDAD_ALOJ,
                                    Fecha = d.FECHA,

                                    NombreUnidad = d.NOMBRE,
                                    Personas = d.CANTPERSONAS.GetValueOrDefault(),
                                    Camas = d.CANTCAMAS.GetValueOrDefault(),

                                    Disponibles = d.CUPODISPONIBLE.GetValueOrDefault(),
                                    MontoPorUnidad = Decimal.Round((decimal)((d.MONTOPROMEDIOPORDIA.GetValueOrDefault() * markup) * (cotizMonedaAloj / cotizMonedaUsuario)), 2),
                                                    //Decimal.Round((decimal)(d.MontoPromedioPorDia.GetValueOrDefault() * markup), 2),
                                }).ToArray();

                            //alojamientoDisponible.Alojamiento.Moneda = respuestaInfoCupos.Moneda;
                            //alojamientoDisponible.Alojamiento.Unidades = new List<InfoUnidad>(respuestaInfoCupos.Unidades).ToArray();
                        }

                        var respuesta = new RespuestaBuscarAlojamientosEInfo()
                        {
                            Estado = EstadoRespuesta.Ok,
                            AlojamientosDisponibles = alojamientosDisponibles
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: Éxito");
                        Tracker.WriteTrace(operationNumber, "Respuesta: " + respuesta.ToStringWithProperties());

                        return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaBuscarAlojamientosEInfo()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }
    }
}