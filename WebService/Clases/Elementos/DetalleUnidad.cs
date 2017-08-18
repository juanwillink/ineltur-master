using System;

namespace Ineltur.WebService
{
    public class DetalleUnidad
    {
        public Guid IdUnidad { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Cantidad { get; set; }
        public InfoPasajero[] Pasajeros { get; set; }
    }
}