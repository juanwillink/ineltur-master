using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ArgentinahtlMVC.Models
{
    public class CurrencyModel
    {
        [Display(Name = "ID")]
        public Guid CurrencyId { get; set; }

        [Display(Name="Nombre")]
        public string CurrencyName { get; set; }

        [Display(Name = "Símbolo")]
        public string CurrencySymbol { get; set; }

        [Display(Name = "Cotización")]
        public float CurrencyValue { get; set; }
    }

    public class DestinationModel
    {
        public List<LodgingModel> Lodgings { get; set; }

        [Display(Name = "ID Destino")]
        public Guid DestinationId { get; set; }

        [Display(Name = "Nombre")]
        public string DestinationName { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }
    }

    public class DestinationListModel
    {
        public DestinationListModel()
        {
            Destinations = new List<DestinationModel>();
        }

        public List<DestinationModel> Destinations { get; set; }
    }
}