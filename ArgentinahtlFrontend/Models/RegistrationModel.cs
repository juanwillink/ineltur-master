using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CheckArgentina.Models
{
    public class RegistrationModel
    {
        public string BuissnessName { get; set; }

        public string RazonSocial { get; set; }

        public string ActivityStartDate { get; set; }

        public string BillType { get; set; }

        public string Cuit { get; set; }

        public string Adress { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string ReservationConfirmationEmail { get; set; }

        public string PersonInChargeName { get; set; }

        public string PersonInChargeEmail { get; set; }

        public bool Newsletter { get; set; }

        public string HowDidYouFindUs { get; set; }


    }
}