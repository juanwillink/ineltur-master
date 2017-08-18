using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CheckArgentina.Models
{
    public class NPSPaymentModel
    {
        public string psp_TransactionId { get; set; }
        public string psp_MerchTxRef { get; set; }
    }

    public class NPSRedirectionModel
    {
        public string FrontPSP_URL { get; set; }
    }
}