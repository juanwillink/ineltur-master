using System;
using Ineltur.Datos;

namespace Ineltur.WebService
{
    public class InfoAlojamientoDisponible
    {
        public InfoDestino Destino { get; set; }

        public TipoAlojamiento? TipoAlojamiento { get; set; }
        public Guid? IdAlojamiento { get; set; }
        public string Nombre { get; set; }
        public Moneda? Moneda { get; set; }

        public decimal? Tarifa1 { get; set; }
        public decimal? Tarifa2 { get; set; }
        public decimal? Tarifa3 { get; set; }
        public decimal? Tarifa4 { get; set; }
        public decimal? Tarifa5 { get; set; }
        public decimal? Tarifa6 { get; set; }

        public int? Cupo1 { get; set; }
        public int? Cupo2 { get; set; }
        public int? Cupo3 { get; set; }
        public int? Cupo4 { get; set; }
        public int? Cupo5 { get; set; }
        public int? Cupo6 { get; set; }
    }
}