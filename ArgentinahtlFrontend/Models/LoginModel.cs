using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CheckArgentina.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Debe ingresar un nombre de usuario")]
        [Display(Name = "Usuario")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UID { get; set; }
    }
}
