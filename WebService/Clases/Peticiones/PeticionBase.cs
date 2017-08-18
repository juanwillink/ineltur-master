using System.Linq;

namespace Ineltur.WebService
{
    public abstract class PeticionBase
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }

        public string IdiomaDeseado { get; set; }
    }
}