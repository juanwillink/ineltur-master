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
        public RespuestaBuscarAlojamientos BuscarAlojamientos(PeticionBuscarAlojamientos peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio BuscarAlojamientos");
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

            if (!ciudad.HasValue && !provincia.HasValue)
            {
                return new RespuestaBuscarAlojamientos()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = "TipoDestino"
                };
            }

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaBuscarAlojamientos()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientos: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    if (peticion.FechaInicio == null || peticion.FechaInicio < DateTime.Today)
                    {
                        return new RespuestaBuscarAlojamientos()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaInicio"
                        };
                    }

                    if (peticion.FechaFin == null || peticion.FechaInicio >= peticion.FechaFin)
                    {
                        return new RespuestaBuscarAlojamientos()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaFin"
                        };
                    }

                    var usuario = dc.Usuarios.SingleOrDefault(u => u.IdUsuario == idUsuario);
                   
                    var encontrados = dc.GetAlojamientosConDisponibilidad(
                        peticion.Habitacion1.GetValueOrDefault(), peticion.Habitacion2.GetValueOrDefault(),
                        peticion.Habitacion3.GetValueOrDefault(), peticion.Habitacion4.GetValueOrDefault(),
                        peticion.Habitacion5.GetValueOrDefault(), peticion.Habitacion6.GetValueOrDefault(),
                        peticion.FechaInicio, peticion.FechaFin, ConvertirOrdenamiento(peticion.Orden),
                        ciudad, provincia, ConvertirTipoAlojamiento(peticion.TipoAlojamiento), peticion.Nacionalidad)
                            .Where(a => a.montoTotalEstimadoEnPesos > 0).ToArray();
                      
                    if (encontrados.Length == 0)
                    {
                        return new RespuestaBuscarAlojamientos()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };
                    }
                    else
                    {
                        var markup = 1 / ((1 - (usuario.MarkupAAgencia) / 100) * (1 - (usuario.MarkupAConsumidorFinal) / 100));
                        //markupYConversion = monedaAloj.COTIZACION / ((1-usrOperaciones.MARKUPAAGENCIA/100) * (1-usrOperaciones.MARKUPACONSUMIDORFINAL/100) * moneda.COTIZACION)
                        var cotizMonedaUsuario = ObtenerCotizacion(dc, usuario.IdMoneda);
 
                        var respuesta = new RespuestaBuscarAlojamientos()
                        {
                            Estado = EstadoRespuesta.Ok,
                            AlojamientosDisponibles = encontrados.Select(a => new InfoAlojamientoDisponible()
                            {
                                Destino = new InfoDestino()
                                {
                                    TipoDestino = TipoDestino.Ciudad,
                                    IdDestino = a.idCiudad.GetValueOrDefault(),
                                    NombreDestino = String.Concat(a.nombreCiudad, ", ", a.nombreProvincia)
                                },

                                TipoAlojamiento = ConvertirTipoAlojamiento(a.idTipoAloj),
                                IdAlojamiento = a.idAloj.GetValueOrDefault(),
                                Nombre = a.nombre,
                                //Moneda = ConvertirMoneda(a.IdMoneda.GetValueOrDefault()),
                                Moneda = ConvertirMoneda(usuario.IdMoneda),
 
                                Cupo1 = a.cupoTotalDisponibleSingle,
                                Cupo2 = a.cupoTotalDisponibleDoble,
                                Cupo3 = a.cupoTotalDisponibleTriple,
                                Cupo4 = a.cupoTotalDisponibleCuadruple,
                                Cupo5 = a.cupoTotalDisponible5Personas,
                                Cupo6 = a.cupoTotalDisponible6Personas,

                                Tarifa1 = Decimal.Round((decimal)((a.montoUnidadSingleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa2 = Decimal.Round((decimal)((a.montoUnidadDobleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa3 = Decimal.Round((decimal)((a.montoUnidadTripleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa4 = Decimal.Round((decimal)((a.montoUnidadCuadrupleMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa5 = Decimal.Round((decimal)((a.montoUnidad5PersonasMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2),
                                Tarifa6 = Decimal.Round((decimal)((a.montoUnidad6PersonasMasBarataPorDia.GetValueOrDefault() * markup)* (ObtenerCotizacion(dc, (Guid) a.idMoneda) / cotizMonedaUsuario)), 2)
                                
                            }).ToArray()
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientos: Éxito");
                        Tracker.WriteTrace(operationNumber, "Respuesta: " + respuesta.ToStringWithProperties());

                        return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaBuscarAlojamientos()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientos: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }
    }
}