using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ArgentinahtlMVC.Models
{
    public class SeasonTypeModel
    {
        [Display(Name = "ID Tipo Temporada")]
        public Guid? SeasonTypeId { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string SeasonTypeName { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
    }

    public class TarifasTypeModel
    {
        [Display(Name = "ID Tipo Moneda")]
        public Guid? TarifaTypeId { get; set; }

        [Required]
        [Display(Name = "Simbolo")]
        public string TarifaTypeSimbolo { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
    }

    public class PromocionesTypeModel
    {
        [Display(Name = "ID Tipo Promocion")]
        public int PromocionTypeId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Maximo De Dias")]
        public int? DiasMax { get; set; }

        [Display(Name = "Minimo De Dias")]
        public int? DiasMin { get; set; }

        [Display(Name = "Codigo")]
        public int? Codigo { get; set; }
    }

    public class SeasonModel
    {
        public List<SeasonTypeModel> SeasonTypes { get; set; }

        [Display(Name = "ID Temporada")]
        public Guid? SeasonId { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string SeasonName { get; set; }

        [Display(Name = "ID Alojamiento")]
        public Guid? LodgingId { get; set; }

        [Required]
        [Display(Name = "Habilitada")]
        public bool Enabled { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Fecha Alta")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOfRegistration { get; set; }

        [Required]
        [Display(Name = "ID Tipo Temporada")]
        public Guid? SeasonTypeId { get; set; }

        [Required]
        [Display(Name = "Deadline")]
        public int Deadline { get; set; }
    }

    public class TarifaModel
    {
        public List<TarifasTypeModel> TarifasTypes { get; set; }

        [Display(Name = "ID Tarifa")]
        public long? TarifaId { get; set; }

        [Display(Name = "ID Alojamiento")]
        public Guid? LodgingId { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "ID Tipo Tarifa")]
        public Guid TarifaTypeId { get; set; }

        [Required]
        [Display(Name = "ID Nacionalidad")]
        public string NationalityId { get; set; }
    }

    public class PromocionModel
    {
        public List<PromocionesTypeModel> PromocionesType { get; set; }

        public LodgingModel Lodging { get; set; }

        [Display(Name = "ID Promocion")]
        public Guid PromocionId { get; set; }

        [Display(Name = "ID Alojamiento")]
        public Guid LodgingId { get; set; }

        [Display(Name = "Nombre Promocion")]
        public string Nombre { get; set; }

        [Display(Name = "Descripcion")]
        public string Descripcion1 { get; set; }

        [Display(Name = "Descripcion 2")]
        public string Descripcion2 { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; }

        [Required]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Dias Reservados")]
        public int? DiasReservados { get; set; }

        [Display(Name = "Dias A Cobrar")]
        public int? DiasACobrar { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [Display(Name = "IdUnidadPromo")]
        public Guid? IdUnidadPromo { get; set; }

        [Display(Name = "ID Tipo Promocion")]
        public int PromocionTypeId { get; set; }

        [Display(Name = "Slogan")]
        public string Slogan { get; set; }

        [Display(Name = "Minimo Noches")]
        public int? MinimoNoches { get; set; }

        [Display(Name = "Maximo Noches")]
        public int? MaximoNoches { get; set; }

        [Display(Name = "Descuento")]
        public float? Descuento { get; set; }
    }

    public class SeasonListModel
    {
        public SeasonListModel()
        {
            Lodging = new LodgingModel();
            Seasons = new List<SeasonModel>();
        }

        public List<SeasonTypeModel> SeasonTypes { get; set; }
        public List<SeasonModel> Seasons { get; set; }

        public long Count { get { return (Seasons != null ? Seasons.Count() : 0); } }

        public LodgingModel Lodging { get; set; }
    }

    public class TarifasListModel
    {
        public TarifasListModel()
        {
            Lodging = new LodgingModel();
            Tarifas = new List<TarifaModel>();
        }
        public List<TarifasTypeModel> TarifasTypes { get; set; }
        public List<TarifaModel> Tarifas { get; set; }
        public long Count  { get { return (Tarifas != null ? Tarifas.Count() : 0); } }
        public LodgingModel Lodging { get; set; }

    }

    public class PromocionesListModel
    {
        public PromocionesListModel()
        {
            Lodging = new LodgingModel();
            Promociones = new List<PromocionModel>();
        }
        public List<PromocionesTypeModel> PromocionesType { get; set; }
        public List<PromocionModel> Promociones { get; set; }
        public long Count { get { return (Promociones != null ? Promociones.Count() : 0); } }
        public LodgingModel Lodging { get; set; }
    }

    public class SeasonsModel
    {
        public List<DestinationModel> Provinces { get; set; }
        public List<DestinationModel> Cities { get; set; }
    }

    public class TarifasModel
    {
        public List<DestinationModel> Provinces { get; set; }
        public List<DestinationModel> Cities { get; set; }
    }

    public class PromocionesModel
    {
        public List<DestinationModel> Provinces { get; set; }
        public List<DestinationModel> Cities { get; set; }
    }
}