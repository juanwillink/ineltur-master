using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckArgentina.Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserLogo { get; set; }
        public bool UsesPaymentInterface { get; set; }
        public Guid? LodgingId { get; set; }
        public string LodgingName { get; set; }
        public Guid? DestinationId { get; set; }
        public string DestinationName { get; set; }
    }
}