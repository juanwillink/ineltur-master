using System;
using System.Linq;
using System.Web.Services;
using Ineltur.Datos;
using Ineltur.WebService.Clases;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarDestinos")]
        public RespuestaInfoCuposAlojamiento InfoCuposAlojamiento(PeticionInfoCuposAlojamiento peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio InfoCuposAlojamiento");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaInfoCuposAlojamiento()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin InfoCuposAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    var usuario = dc.Usuarios.SingleOrDefault(u => u.IdUsuario == idUsuario);

                    var alojamiento = dc.Alojamientos.SingleOrDefault(
                        a => a.IdAlojamiento == peticion.IdAlojamiento);

                    if (alojamiento == null)
                    {
                        var respuestaError = new RespuestaInfoCuposAlojamiento()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    if (peticion.FechaInicio == null || peticion.FechaInicio < DateTime.Today)
                    {
                        var respuestaError = new RespuestaInfoCuposAlojamiento()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaInicio"
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    if (peticion.FechaFin == null || peticion.FechaInicio >= peticion.FechaFin)
                    {
                        var respuestaError = new RespuestaInfoCuposAlojamiento()
                        {
                            Estado = EstadoRespuesta.ErrorParametro,
                            MensajeEstado = "FechaFin"
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarAlojamientosEInfo: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Error);

                        return respuestaError;
                    }

                    var disponible = dc.GetCuposAlojamientoEnRangoFechas(1, 1, 1, 1, 1, 1,
                            peticion.FechaInicio, peticion.FechaFin, peticion.IdAlojamiento, peticion.Nacionalidad).OrderBy(
                            d => d.MONTOPROMEDIOPORDIA).Where(d => d.MONTOPROMEDIOPORDIA > 0 && d.FECHA >= DateTime.Now.Date.AddDays((double) 3)).ToList();

                    //Moneda moneda = ConvertirMoneda(alojamiento.IdMoneda.GetValueOrDefault());
                    Moneda moneda = ConvertirMoneda(usuario.IdMoneda);
                    var cotizMonedaUsuario = ObtenerCotizacion(dc, usuario.IdMoneda);//AM
                    var cotizMonedaAloj = ObtenerCotizacion(dc, (Guid) alojamiento.IdMoneda);//AM

                    var markup = 1 / ((1 - (usuario.MarkupAAgencia) / 100) * (1 - (usuario.MarkupAConsumidorFinal) / 100));

                    var info = new RespuestaInfoCuposAlojamiento()
                    {
                        Estado = EstadoRespuesta.Ok,

                        Moneda = moneda,
                        Alojamiento = new InfoAlojamiento(),

                        Unidades = disponible.Where(d => d.CUPODISPONIBLE.GetValueOrDefault() > 0).Select(d => new InfoUnidad()
                        {
                            IdUnidad = d.IDUNIDAD_ALOJ,
                            Fecha = d.FECHA,

                            NombreUnidad = d.NOMBRE,
                            Personas = d.CANTPERSONAS.GetValueOrDefault(),
                            Camas = d.CANTCAMAS.GetValueOrDefault(),
                            
                            Disponibles = d.CUPODISPONIBLE.GetValueOrDefault(),
                            //MontoPorUnidad = Decimal.Round((decimal)(d.MontoPromedioPorDia.GetValueOrDefault() * markup), 2),
                            MontoPorUnidad = Decimal.Round((decimal)((d.MONTOPROMEDIOPORDIA.GetValueOrDefault() * markup) * (cotizMonedaAloj / cotizMonedaUsuario)), 2),
                        }).ToArray()
                    };

                    CargarAmenidadesRegimenAlojamiento(alojamiento, info.Alojamiento);
                    CargarInfoAlojamiento(alojamiento, info.Alojamiento, usuario.IdMoneda);

                    Tracker.WriteTrace(operationNumber, "Fin InfoCuposAlojamiento: Éxito");
                    Tracker.WriteTrace(operationNumber, "Respuesta: " + info.ToStringWithProperties());

                    return info;
                }         
            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaInfoCuposAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin InfoCuposAlojamiento: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }
    }
}