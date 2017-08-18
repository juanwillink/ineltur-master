using System;
using Ineltur.Datos;

namespace Ineltur.WebService
{
    public class PeticionBuscarAlojamientos : PeticionBase
    {
        public Guid IdDestino { get; set; }
        public TipoDestino TipoDestino { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TipoAlojamiento? TipoAlojamiento { get; set; }
        public int? Habitacion1 { get; set; }
        public int? Habitacion2 { get; set; }
        public int? Habitacion3 { get; set; }
        public int? Habitacion4 { get; set; }
        public int? Habitacion5 { get; set; }
        public int? Habitacion6 { get; set; }
        public OrdenAlojamientos? Orden { get; set; }
        public string Nacionalidad { get; set; }
        public string NombreAlojamiento { get; set; }
        public int desayuno { get; set; }
        public int tarifaReembolsable { get; set; }
    }
}