using System;

namespace Ineltur.WebService
{
    public class PeticionInfoCuposAlojamiento : PeticionInfoAlojamiento
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Nacionalidad { get; set; }
    }
}