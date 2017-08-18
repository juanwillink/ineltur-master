using Ineltur.Datos;

namespace Ineltur.WebService
{
    public class RespuestaInfoCuposAlojamiento : RespuestaInfoAlojamiento
    {
        public Moneda Moneda { get; set; }

        public InfoUnidad[] Unidades { get; set; }
    }
}