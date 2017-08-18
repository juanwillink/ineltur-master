namespace Ineltur.WebService
{
    public abstract class RespuestaBase
    {
        public EstadoRespuesta Estado { get; set; }
        public string MensajeEstado { get; set; }
    }
}