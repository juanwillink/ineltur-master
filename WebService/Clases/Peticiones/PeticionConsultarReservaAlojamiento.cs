using System;

namespace Ineltur.WebService
{
    public class PeticionConsultarReservaAlojamiento : PeticionBase
    {
        public Guid IdReserva { get; set; }
        public int CodigoReserva { get; set; }
    }
}