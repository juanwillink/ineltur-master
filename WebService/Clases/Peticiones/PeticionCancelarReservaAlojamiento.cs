using System;

namespace Ineltur.WebService
{
    public class PeticionCancelarReservaAlojamiento : PeticionBase
    {
        public Guid IdReserva { get; set; }
        public int CodigoReserva { get; set; }
    }
}