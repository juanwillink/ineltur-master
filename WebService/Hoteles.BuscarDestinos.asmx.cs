using System;
using System.Linq;
using System.Web.Services;
using Ineltur.WebService.Clases;

namespace Ineltur.WebService
{
    partial class Hoteles
    {
        [WebMethod(CacheDuration = 3600)]
        //[SoapDocumentMethod(Binding = "BindingHotelesSoap", Action = "http://www.ineltur.com/BuscarDestinos")]
        public RespuestaBuscarDestinos BuscarDestinos(PeticionBuscarDestinos peticion)
        {
            var operationNumber = IncreaseOperationNumber().ToString().PadLeft(8, '0');

            Tracker.WriteTrace(operationNumber, "Inicio BuscarDestinos");
            Tracker.WriteTrace(operationNumber, "IP Solicitante: " + this.Context.Request.UserHostAddress);
            Tracker.WriteTrace(operationNumber, "Petición: " + peticion.ToStringWithProperties());

            string destino = peticion.Destino;

            if (destino.Length < ParametrosBasicos.LongitudMinimaCadenaBusqueda)
            {
                return new RespuestaBuscarDestinos()
                {
                    Estado = EstadoRespuesta.ErrorParametro,
                    MensajeEstado = "Destino"
                };
            }

            try
            {
                using (var dc = NuevoDataContext())
                {
                    var idUsuario = ValidarUsuarioClave(dc, peticion);

                    if (idUsuario == null)
                    {
                        var respuestaError = new RespuestaBuscarDestinos()
                        {
                            Estado = EstadoRespuesta.CredencialesNoValidas
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarDestinos: " + respuestaError.ToStringWithProperties(), traceType: Tracker.TraceType.Fatal);

                    }
                    var provincias = dc.Provincias.Where(p => p.Nombre.Contains(destino)).ToList();
                    var destinos = provincias.SelectMany(p => p.Ciudades)
                        .Concat(dc.Ciudades.Where(p => p.Nombre.Contains(destino)))
                        .Distinct().Select(d => new InfoDestino()
                        {
                            TipoDestino = TipoDestino.Ciudad,
                            IdDestino = d.IdCiudad,
                            NombreDestino = String.Concat(d.Nombre, ", ", d.Provincia.Nombre),
                        }).Concat(provincias.Select(p => new InfoDestino()
                        {
                            TipoDestino = TipoDestino.Provincia,
                            IdDestino = p.IdProvincia,
                            NombreDestino = p.Nombre
                        })).ToArray();

                    if (destinos.Length == 0)
                    {
                        return new RespuestaBuscarDestinos()
                        {
                            Estado = EstadoRespuesta.NoEncontrado
                        };
                    }
                    else
                    {
                        var respuesta = new RespuestaBuscarDestinos()
                        {
                            Estado = EstadoRespuesta.Ok,
                            Destinos = destinos
                        };

                        Tracker.WriteTrace(operationNumber, "Fin BuscarDestinos: Éxito");
                        Tracker.WriteTrace(operationNumber, "Respuesta: " + respuesta.ToStringWithProperties());

                        return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                var respuestaError = new RespuestaBuscarDestinos()
                {
                    Estado = EstadoRespuesta.ErrorInterno,
                    MensajeEstado = string.Empty //ex.ToString()
                };

                Tracker.WriteTrace(operationNumber, "Fin BuscarDestinos: " + respuestaError.ToStringWithProperties() + ex.Message, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Stack Trace: " + ex.StackTrace, traceType: Tracker.TraceType.Fatal);
                Tracker.WriteTrace(operationNumber, "Inner Exception: " + ex.InnerException.Message, traceType: Tracker.TraceType.Fatal);

                return respuestaError;
            }
        }
    }
}