using System;

namespace Ineltur.WebService
{
    public class PeticionReservarAlojamiento : PeticionBase
    {
        public Guid IdAlojamiento { get; set; }
        
        public Guid IdFormaPago { get; set; }

        public InfoPasajero Titular { get; set; }

        public DetalleUnidad[] Unidades { get; set; }

        public string Nacionalidad { get; set; }

        public int Desayuno { get; set; }

        public int TarifaReembolsable { get; set; }

        public string Observaciones { get; set; }
        public EstadoReserva EstadoReserva { get; set; }

        public bool IncurreGastos { get; set; }

        public bool TienePromocion { get; set; }

        public decimal PrecioPromocional { get; set; }
    }
}