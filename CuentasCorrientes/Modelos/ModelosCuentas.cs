using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ineltur.CuentasCorrientes.Modelos.Servicios;
using Ineltur.Datos;

namespace Ineltur.CuentasCorrientes.Modelos
{
    public class ModeloCuenta
    {
        public Guid IdUsuario { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Nombre Usuario")]
        public string NombreUsuario { get; set; }

        [Display(Name = "Clave")]
        public string Contrasenya { get; set; }

        [Display(Name = "Saldo Cuenta")]
        public decimal? Saldo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [Display(Name = "Forma de Pago")]
        public FormaPago? FormaPago { get; set; }

        [Display(Name = "Margen (%)")]
        public double? Margen { get; set; }
    }

    public class ModeloListaCuentas
    {
        public ModeloCuenta[] Cuentas { get; set; }
    }

    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum TipoMovimiento
    {
        Reserva,
        ReservaAnulada,
        Pago
    }

    public class ModeloDetalleCuenta
    {
        public DateTime Fecha { get; set; }
        public TipoMovimiento Tipo { get; set; }
        public Moneda Moneda { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoPesos { get; set; }
        public string Cliente { get; set; }
    }

    public class ModeloListaDetallesCuentas
    {
        [Display(Name = "Fecha Desde")]
        public DateTime? Desde { get; set; }

        [Display(Name = "Fecha Hasta")]
        public DateTime? Hasta { get; set; }

        public int Pagina { get; set; }
        public int UltimaPagina { get; set; }

        public Guid IdUsuario { get; set; }

        public string Nombre { get; set; }

        public ModeloDetalleCuenta[] Detalles { get; set; }
    }

    public class ModeloPago
    {
        public Guid IdUsuario { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        
        [Display(Name = "Saldo de la Cuenta")]
        public decimal Saldo { get; set; }

        [Display(Name = "Fecha del Pago")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaPago { get; set; }

        [Display(Name = "Monto Pagado")]
        [Required]
        [Range(0.0, Double.MaxValue)]
        public decimal MontoPagado { get; set; }

        [Display(Name = "Moneda")]
        [Required]
        public Moneda? Moneda { get; set; }

        [Display(Name = "Tipo de Cambio")]
        [Required]
        [Range(0.0, Double.MaxValue)]
        public decimal TipoCambio { get; set; }
    }
}