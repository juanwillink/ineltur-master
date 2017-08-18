using Ineltur.Datos;

namespace Ineltur.WebService
{
    public class RespuestaConsultarReservaAlojamiento : RespuestaBase
    {
        public InfoPasajero Pasajero { get; set; }

        public InfoAlojamiento Alojamiento { get; set; }

        public EstadoReserva? EstadoReserva { get; set; }

        public Moneda? Moneda { get; set; }
        public decimal? Total { get; set; }
        public DetalleReserva[] Detalles { get; set; }
    }
}