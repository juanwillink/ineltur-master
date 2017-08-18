using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CheckArgentina.Models
{
    public class SearchModel
    {
        public bool ExtendedSearch { get; set; }
        public string UserKey { get; set; }
        public string UserName { get; set; }

        //[Required(ErrorMessage = "Debe indicar el destino")]
        [Display(Name = "Destino")]
        [DataType(DataType.Text)]
        public string DestinationId { get; set; }

        [Display(Name = "Destino")]
        [DataType(DataType.Text)]
        public string DestinationName { get; set; }

        [Required(ErrorMessage = "Debe indicar el país")]
        [Display(Name = "País")]
        [DataType(DataType.Text)]
        public string DestinationParentId { get; set; }

        [Display(Name = "País")]
        [DataType(DataType.Text)]
        public string DestinationParentName { get; set; }
        
        [Display(Name = "Zona")]
        [DataType(DataType.Text)]
        public string Zone { get; set; }

        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string Lodging { get; set; }

        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string LodgingId { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de checkin")]
        [Display(Name = "Checkin")]
        [DataType(DataType.Date)]
        public string Checkin { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de checkout")]
        [Display(Name = "Checkout")]
        [DataType(DataType.Date)]
        public string Checkout { get; set; }

        [Display(Name = "Noches")]
        [DataType(DataType.Text)]
        public string NightsCount { get; set; }

        [Required(ErrorMessage = "Debe indicar la cantidad de ocupantes")]
        [Display(Name = "Adultos")]
        [DataType(DataType.Text)]
        public string Adults { get; set; }

        [Display(Name = "Cunas")]
        [DataType(DataType.Text)]
        public string Cradles { get; set; }

        [Display(Name = "Niños")]
        [DataType(DataType.Text)]
        public string Children { get; set; }

        [Display(Name = "Edad 1")]
        [DataType(DataType.Text)]
        public string Age1 { get; set; }

        [Display(Name = "Edad 2")]
        [DataType(DataType.Text)]
        public string Age2 { get; set; }

        [Display(Name = "Tipo")]
        [DataType(DataType.Text)]
        public string Type { get; set; }

        [Display(Name = "Orden")]
        [DataType(DataType.Text)]
        public string Order { get; set; }

        [Display(Name = "Nacionalidad")]
        [DataType(DataType.Text)]
        public string Nationality { get; set; }

        [Display(Name = "Visualizacion")]
        [DataType(DataType.Text)]
        public string DisplayType { get; set; }



    }

    public class SearchLodgingModel
    {
        public RoomType RoomType { get; set; }
        public string RoomTypeCode { get; set; }
        public int Count { get; set; }
        public int Children { get; set; }
        public IEnumerable<int> ChildrenAges { get; set; }
    }

    public class SearchLodgingRequestModel
    {
        public string DestinationParentId { get; set; }
        public string DestinationId { get; set; }
        public string DestinationType { get; set; }
        public string LodgingId { get; set; }
        public string LodgingName { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public string UserKey { get; set; }
        public string Order { get; set; }
        public string Nationality { get; set; }
        public string DisplayType { get; set; }
        public int Breakfast { get; set; }
        public int Tarifa { get; set; }

        public IEnumerable<SearchLodgingModel> Rooms { get; set; }
    }
}