using System;
using Ineltur.Datos;
using System.ComponentModel.DataAnnotations;

namespace Ineltur.WebService
{
    public class InfoPasajero
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public Sexo Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}