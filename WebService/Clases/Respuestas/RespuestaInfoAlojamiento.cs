using System;

namespace Ineltur.WebService
{
    public class RespuestaInfoAlojamiento : RespuestaBase
    {
        public InfoAlojamiento Alojamiento { get; set; }
    }

    public class RespuestaInfoCuposSemanalesAlojamiento : RespuestaBase
    {
        public InfoCuposAlojamiento Alojamiento { get; set; }
    }
}