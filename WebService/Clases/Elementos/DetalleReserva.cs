using System;

namespace Ineltur.WebService
{
    public class DetalleReserva
    {
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public int Dias { get; set; }
        public decimal PorUnidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}