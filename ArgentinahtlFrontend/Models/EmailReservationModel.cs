using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckArgentina.Models
{
    public class EmailReservationModel
    {
        public string LodgingName { get; set; }
        public string DestinationName { get; set; }
        public string TravelerName { get; set; }
        public string TravelerEmail { get; set; }
        public string TravelersCount { get; set; }
        public string Nationality { get; set; }
        public string TravelerId { get; set; }
        public string CheckinDate { get; set; }
        public string CheckoutDate { get; set; }
        public string RoomsCount { get; set; }
        public string RoomType { get; set; }
        public string Observations { get; set; }
    }
}