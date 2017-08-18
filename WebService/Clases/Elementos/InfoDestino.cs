using System;

namespace Ineltur.WebService
{
    public enum TipoDestino
    {
        Ciudad,
        Provincia,
        Region,
        NoEspecificado
    }

    public class InfoDestino
    {
        public Guid IdDestino { get; set; }

        public TipoDestino TipoDestino { get; set; }

        public string NombreDestino { get; set; }
    }
}