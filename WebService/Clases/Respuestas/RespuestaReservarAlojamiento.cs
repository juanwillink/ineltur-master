using System;
using Ineltur.Datos;

namespace Ineltur.WebService
{
    public class RespuestaReservarAlojamiento : RespuestaBase
    {
        public Guid? IdReserva { get; set; }
        public int? CodigoReserva { get; set; }

        public Moneda? Moneda { get; set; }
        public decimal? Total { get; set; }

        public DateTime? VencimientoReserva { get; set; }

        public DetalleReserva[] Detalles { get; set; }
    }
}