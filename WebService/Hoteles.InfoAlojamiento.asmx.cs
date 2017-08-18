using System;
using System.Linq;
using System.Web.Services;
using Ineltur.WebService.Clases;
using Ineltur.Datos.Entidades;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod(CacheDuration = 3600)]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarDestinos")]
        public RespuestaInfoAlojamiento InfoAlojamiento(PeticionInfoAlojamiento peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio InfoAlojamiento");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaInfoAlojamiento()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin InfoAlojamiento: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                        return respuestaError;
                    }

                    var alojamiento = dc.Alojamientos.SingleOrDefault(a => a.IdAlojamiento == peticion.IdAlojamiento);

                    if (alojamiento != null)
                    {
                        var info = new RespuestaInfoAlojamiento()
                        {
                            Estado = EstadoRespuesta.Ok,
                            
                            Alojamiento = ObtenerInfoAlojamiento(alojamiento, ObtenerMonedaUsuario(dc, (Guid) idUsuario))
                        };

                        Tracker.WriteTrace(operationNumber, "Fin InfoAlojamiento: Éxito");
                        Tracker.WriteTrace(operationNumber, "Respuesta: " + info.ToStringWithProperties());

                        return info;
                    }
                    else
                    {
                        return new RespuestaInfoAlojamiento()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };
                    }
                }         
            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaInfoAlojamiento()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin InfoAlojamiento: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }

        private InfoAlojamiento ObtenerInfoAlojamiento(Alojamiento alojamiento, Guid idMonedaUsuario)
        {
            var info = new InfoAlojamiento();

            CargarAmenidadesRegimenAlojamiento(alojamiento, info);
            CargarInfoAlojamiento(alojamiento, info, idMonedaUsuario); 

            return info;
        }
    }
}