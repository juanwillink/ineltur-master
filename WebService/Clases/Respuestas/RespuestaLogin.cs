namespace Ineltur.WebService
{
    public class RespuestaLogin : RespuestaBase
    {
        public string User { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UID { get; set; }
    }
}