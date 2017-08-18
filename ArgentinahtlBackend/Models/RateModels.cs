using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace ArgentinahtlMVC.Models
{
    public class RateModel
    {
        public RateModel() { UnidadAlojamiento = new RoomModel(); }

        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [Display(Name = "RoomId")]
        public Guid RoomId { 
            get{return UnidadAlojamiento.RoomId;} 
            set{UnidadAlojamiento.RoomId = value;} 
        }

        [Display(Name = "Unidad Alojamiento")]
        public RoomModel UnidadAlojamiento { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Cupo Máximo")]
        public int CupoMaximo { get; set; }

        [Display(Name = "Cupo Reservado")]
        public int CupoReservado { get; set; }

        [Display(Name = "Monto RA CD TR")]
        public float MontoRACDTR { get; set; }

        [Display(Name = "Monto EXT CD TR")]
        public float? MontoEXTCDTR { get; set; }

        [Display(Name = "Monto MER CD TR")]
        public float? MontoMERCDTR { get; set; }

        [Display(Name = "Monto RA SD TR")]
        public float? MontoRASDTR { get; set; }

        [Display(Name = "Monto EXT SD TR")]
        public float? MontoEXTSDTR { get; set; }

        [Display(Name = "Monto MER SD TR")]
        public float? MontoMERSDTR { get; set; }

        [Display(Name = "Monto RA CD TNR")]
        public float? MontoRACDTNR { get; set; }

        [Display(Name = "Monto EXT CD TNR")]
        public float? MontoEXTCDTNR { get; set; }

        [Display(Name = "Monto MER CD TNR")]
        public float? MontoMERCDTNR { get; set; }

        [Display(Name = "Monto RA SD TNR")]
        public float? MontoRASDTNR { get; set; }

        [Display(Name = "Monto EXT SD TNR")]
        public float? MontoEXTSDTNR { get; set; }

        [Display(Name = "Monto MER SD TNR")]
        public float? MontoMERSDTNR { get; set; }

        [Required]
        [Display(Name = "Fecha Alta")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAlta { get; set; }

        [Required]
        [Display(Name = "Fecha Desde")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Fecha Hasta")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }


        public bool Lun { get; set; }
        public bool Mar { get; set; }
        public bool Mie { get; set; }
        public bool Jue { get; set; }
        public bool Vie { get; set; }
        public bool Sab { get; set; }
        public bool Dom { get; set; }
    }

    public class RateListModel
    {
        public RateListModel()
        {
            Lodging = new LodgingModel();
            Rates = new List<RateModel>();
        }

        //public List<SeasonTypeModel> SeasonTypes { get; set; }
        public List<RateModel> Rates { get; set; }

        public long Count { get { return (Rates != null ? Rates.Count() : 0); } }

        public LodgingModel Lodging { get; set; }

        public string TablaCupos { get; set; } 
    }
}