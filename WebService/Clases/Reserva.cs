using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ineltur.WebService
{
    public class Reserva
    {
        public int CodigoReserva { get; set; }
        public string Descripcion { get; set; }
        public int? EstadoPago { get; set; }
        public int EstadoReserva { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public Guid? IdAlojamiento { get; set; }
        public Guid? IdFormaDePago { get; set; }
        public Guid IdMoneda { get; set; }
        public int? IdPu { get; set; }
        public Guid IdSitioOrigen { get; set; }
        public Guid IdTipoFormaDePago { get; set; }
        public Guid IdTransaccion { get; set; }
        public Guid? IdUsuario { get; set; }
        public float? MontoTotalConDescuento { get; set; }
        public float? MontoTotalSinDescuento { get; set; }
        public string NombreAlojamiento { get; set; }
        public string NombreFormaDePago { get; set; }
        public string NombrePasajero { get; set; }
        public int TipoTransaccion { get; set; }
        public InfoUnidadReservada[] Unidades { get; set; }
    }
}