using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ArgentinahtlMVC.Models
{
    public enum ReservationStatus
    {
        ToCheck = 0,
        Effective = 1,
        Cancelled = 2,
        Refused = 3,
        UnfinishedWithoutAvailabilityMarked = -1,
        UnfinishedWithAvailabilityMarked = -2,
        CancellationPending = -3,
    }

    public class TransactionModel
    {
        public List<PassengerModel> Passengers 
        {
            get
            {
                var passengers = new List<PassengerModel>();
                Reservations.ForEach(r => passengers.AddRange(r.Passengers));

                return passengers;
            }
        }

        public List<ReservationModel> Reservations { get; set; }

        [Display(Name = "ID")]
        public Guid? TransactionId { get; set; }

        [Display(Name = "Cód. Reserva")]
        public long ReservationCode { get; set; }

        [Display(Name = "Monto")]
        public float Amount { get; set; }

        [Display(Name = "Estado")]
        public ReservationStatus Status { get; set; }

        [Display(Name = "Estado")]
        public string StatusDescription { get; set; }

        [Display(Name = "Forma de Pago")]
        public string PaymentMethod { get; set; }

        [Display(Name = "ID Forma de Pago")]
        public Guid PaymentMethodId { get; set; }

        [Display(Name = "Fecha de Alta")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Checkin")]
        public DateTime? Checkin { get; set; }

        [Display(Name = "Checkout")]
        public DateTime? Checkout { get; set; }

        public PassengerModel Owner { get; set; }
        public AgencyModel Agency { get; set; }
        public LodgingModel Lodging { get; set; }
        public CurrencyModel Currency { get; set; }
    }

    public class TransactionListModel
    {
        public TransactionListModel()
        {
            Transactions = new List<TransactionModel>();
        }

        public List<TransactionModel> Transactions { get; set; }

        public long Count { get { return (Transactions != null ? Transactions.Count : 0); } }
    }

    public class ReservationModel
    {
        public List<PassengerModel> Passengers { get; set; }

        [Display(Name = "ID")]
        public Guid ReservationId { get; set; }

        [Display(Name = "Transaction")]
        public Guid TransactionId { get; set; }

        [Display(Name = "Checkin")]
        public DateTime? Checkin { get; set; }

        [Display(Name = "Checkout")]
        public DateTime? Checkout { get; set; }

        [Display(Name = "Costo")]
        public float Cost { get; set; }

        [Display(Name = "Monto")]
        public float Price { get; set; }

        [Display(Name = "Cantidad")]
        public float Count { get; set; }

        public RoomModel Room { get; set; }
        public CurrencyModel Currency { get; set; }
    }

    public class ReservationListModel
    {
        public List<ReservationModel> Reservations { get; set; }

        public long Count { get { return (Reservations != null ? Reservations.Count : 0); } }

        [Display(Name = "Cód. Reserva")]
        public long ReservationCode { get; set; }
    }

    public class TransactionsModel
    {
        public List<DestinationModel> Provinces { get; set; }
        public List<DestinationModel> Cities { get; set; }
        public List<ReservationStatus> Statuses { get; set; }
        public List<AgencyModel> Agencys { get; set; }

        public TransactionsModel()
        {
            Provinces = new List<DestinationModel>();
            Cities = new List<DestinationModel>();
            Statuses = new List<ReservationStatus>();
            Agencys = new List<AgencyModel>();
        }

        [Required]
        public Guid UserId { get; set; }

        public Guid? ProvinceId { get; set; }

        public Guid? CityId { get; set; }

        public Guid? LodgingId { get; set; }

        public Guid? AgencyId { get; set; }

        public int? Status { get; set; }

        public DateTime? FechaAltaDesde { get; set; }

        public DateTime? FechaAltaHasta { get; set; }

        public DateTime? CheckinDesde { get; set; }

        public DateTime? CheckinHasta { get; set; }
    }
}