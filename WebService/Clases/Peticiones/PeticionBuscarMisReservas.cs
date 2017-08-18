using System;

namespace Ineltur.WebService
{
    public class PeticionBuscarMisReservas : PeticionBase
    {
        public Guid UserId { get; set; }
        public int ReservationCode { get; set; }
        public string SearchParameter { get; set; }
    }
}