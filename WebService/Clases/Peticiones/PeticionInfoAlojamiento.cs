using System;

namespace Ineltur.WebService
{
    public class PeticionInfoAlojamiento : PeticionBase
    {
        public Guid IdAlojamiento { get; set; }
        public DateTime Fecha { get; set; }
    }
}