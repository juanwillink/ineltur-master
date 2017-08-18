using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CheckArgentina.Models
{
    public enum ReservationStatus
    { 
        Reserved, Cancelled, Error, Undefined, InProcess
    }

    public enum IdType
    {
        DNI, LE, LC, CUIT, Pasaporte
    }

    public class ReservationModel
    {
        public ReservationModel()
        {
            Vacancies = new List<VacancyModel>();
        }

        public Dictionary<string, string> Countries { get; set; }
        public Dictionary<string, string> PaymentMethods { get; set; }
        public List<VacancyModel> Vacancies { get; set; }

        public string SecondaryUserName { get; set; }
        public string SecondaryUserPass { get; set; }

        public decimal TotalAmount
        {
            get
            {
                return Vacancies.Sum(v => Decimal.Round(v.VacancyReserved, 0) * Decimal.Round(v.VacancyPrice, 0) * Convert.ToInt16((v.VacancyCheckout - v.VacancyCheckin).TotalDays));
            }
            set { }
            
        }

        public decimal PromotionPrice { get; set; }

        [Required]
        [Display(Name = "ID. Hotel")]
        [DataType(DataType.Text)]
        public string LodgingId { get; set; }

        [Required]
        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string LodgingName { get; set; }

        [Required]
        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string LodgingCurrency { get; set; }

        [Required]
        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string LodgingCurrencyCode { get; set; }

        [Required]
        [Display(Name = "ID Proveedor")]
        [DataType(DataType.Text)]
        public string LodgingSupplierId { get; set; }

        [Required]
        [Display(Name = "ID Destino")]
        [DataType(DataType.Text)]
        public string DestinationId { get; set; }

        [Required]
        [Display(Name = "Estado")]
        [DataType(DataType.Text)]
        public ReservationStatus ReservationStatus { get; set; }

        [Required]
        [Display(Name = "Código de Confirmación")]
        [DataType(DataType.Text)]
        public string ReservationId { get; set; }

        [Required]
        [Display(Name = "Código de reserva")]
        [DataType(DataType.Text)]
        public string ReservationCode { get; set; }

        [Required(ErrorMessage = "Debe indicar la forma de pago.")]
        [Display(Name = "Forma de pago")]
        [DataType(DataType.Text)]
        public string PaymentMethodId { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.Text)]
        public string Observations { get; set; }

        public TravelerModel ReservationOwner { get; set; }

        public int DiasCancelacionCargo { get; set; }
        public bool TienePromocion { get; set; }
    }

    public class ReservationListModel
    {
        public List<ReservationModel> Reservations { get; set; }
    }

    public class TravelerModel
    {
        [Required(ErrorMessage = "Debe indicar la nacionalidad.")]
        [Display(Name = "País")]
        [DataType(DataType.Text)]
        public string TravelerCountry { get; set; }

        [Required(ErrorMessage = "Debe indicar el nombre.")]
        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        public string TravelerFirstName { get; set; }

        [Required(ErrorMessage = "Debe indicar el apellido.")]
        [Display(Name = "Apellido")]
        [DataType(DataType.Text)]
        public string TravelerLastName { get; set; }

        [Display(Name = "Tipo de identificación")]
        [DataType(DataType.Text)]
        public IdType TravelerIdType { get; set; }


        [Display(Name = "Nro. de identificación")]
        [DataType(DataType.Text)]
        public string TravelerId { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Debe indicar una dirección de email válida.")]
        [Display(Name = "Email")]
        [DataType(DataType.Text)]
        public string TravelerEmail { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public string TravelerBornDate { get; set; }
    }

    public class TravelerListModel
    {
        public List<TravelerModel> Travelers { get; set; }
    }

    public class MyReservationListModel
    {
        public List<MyReservationModel> MyReservations { get; set; }
    }

    public class MyReservationModel
    {
        public int CodigoReserva { get; set; }
        public string Descripcion { get; set; }
        public int? EstadoPago { get; set; }
        public string EstadoReserva { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public Guid? IdAlojamiento { get; set; }
        public Guid? IdFormaDePago { get; set; }
        public Guid IdMoneda { get; set; }
        public int? IdPu { get; set; }
        public Guid IdSitioOrigen { get; set; }
        public Guid IdTipoFormaDePago { get; set; }
        public Guid IdTransaccion { get; set; }
        public Guid? IdUsuario { get; set; }
        public float? MontoTotalConDescuento { get; set; }
        public float? MontoTotalSinDescuento { get; set; }
        public string NombreAlojamiento { get; set; }
        public string NombreFormaDePago { get; set; }
        public string NombrePasajero { get; set; }
        public int TipoTransaccion { get; set; }
        public VacancyListModel Unidades { get; set; }
    }
}