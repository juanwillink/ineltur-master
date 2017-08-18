using System.ComponentModel.DataAnnotations;

namespace Ineltur.CuentasCorrientes.Modelos
{
    public class ModeloLogin
    {
        [Required]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasenya { get; set; }

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }
    }
}