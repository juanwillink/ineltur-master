using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;

namespace CheckArgentina.Models
{
    public enum DestinationType
    {
        City,
        State,
        Region,
        Airport
    }

    public enum RoomType
    {
        Single,
        Double,
        DSU,
        Twin,
        Triple,
        Quad,
        Quintuple,
        Sextuple,
        Septuple,
        Octuple,
        Nonuple
    }

    public class DestinationModel
    {
        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        public string DestinationId { get; set; }

        [Required]
        [Display(Name = "Destino")]
        [DataType(DataType.Text)]
        public string DestinationName { get; set; }
        
        [Required]
        [Display(Name = "Tipo de Destino")]
        [DataType(DataType.Text)]
        public DestinationType DestinationType { get; set; }
    }

    public class DestinationListModel
    {
        public List<DestinationModel> Destinations { get; set; }
    }

    public class LodgingModel
    {
        public LodgingModel()
        {
            Vacancies = new List<VacancyModel>();
        }

        public List<VacancyModel> Vacancies { get; set; }

        /// <summary>
        /// Get the image which corresponds to the specified lodging category
        /// </summary>
        /// <param name="lodgingCategory"></param>
        /// <returns></returns>
        public static int GetCategoryImage(int lodgingCategory)
        {
            // Si es menor a 1 lo deja en 1, si es mayor de 5 lo deja en 5
            lodgingCategory = (lodgingCategory < 1 ? 1 : (lodgingCategory > 5 ? 5 : lodgingCategory));

            return lodgingCategory;
        }

        public static string GetServiceGlyphicon(string service)
        {
            string glyphicon = "";
            switch (service)
            {
                case "Actividades recreativas para niños":
                    glyphicon = "glyphicon glyphicon-tree-deciduous";
                    break;
                case "Agencia de Viajes":
                    glyphicon = "glyphicon glyphicon-plane";
                    break;
                case "Aire acondicionado":
                    glyphicon = "glyphicon glyphicon-leaf";
                    break;
                case "Alquiler de Autos":
                    glyphicon = "glyphicon glyphicon-dashboard";
                    break;
                case "Ascensor":
                    glyphicon = "glyphicon glyphicon-resize-vertical";
                    break;
                case "Bar":
                    glyphicon = "glyphicon glyphicon-glass";
                    break;
                case "Biblioteca":
                    glyphicon = "glyphicon glyphicon-book";
                    break;
                case "Bicicletas":
                    glyphicon = "glyphicon glyphicon-tree-conifer";
                    break;
                case "Calefacción":
                    glyphicon = "glyphicon glyphicon-fire";
                    break;
                case "Copa de Bienvenida":
                    glyphicon = "glyphicon glyphicon-glass";
                    break;
                case "Internet":
                    glyphicon = "glyphicon glyphicon-signal";
                    break;
                case "Música":
                    glyphicon = "glyphicon glyphicon-music";
                    break;
                case "Peluqueria":
                    glyphicon = "glyphicon glyphicon-scissors";
                    break;
                case "Restaurante":
                    glyphicon = "glyphicon glyphicon-cutlery";
                    break;
                case "Teléfono Directo":
                    glyphicon = "glyphicon glyphicon-earphone";
                    break;
                case "Wi Fi":
                    glyphicon = "glyphicon glyphicon-signal";
                    break;
                default:
                    glyphicon = "glyphicon glyphicon-asterisk";
                    break;
            }
            return glyphicon;
        }

        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        public string LodgingId { get; set; }

        [Required]
        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string LodgingName { get; set; }

        [Required]
        [Display(Name = "Direccion")]
        [DataType(DataType.Text)]
        public string LodgingLocation { get; set; }

        [Required]
        [Display(Name = "Ciudad")]
        [DataType(DataType.Text)]
        public string LodgingCity { get; set; }

        [Required]
        [Display(Name = "Telefono")]
        [DataType(DataType.Text)]
        public string LodgingPhone { get; set; }

        [Required]
        [Display(Name = "Amenidades")]
        [DataType(DataType.Text)]
        public string[] LodgingServices { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [DataType(DataType.Text)]
        public string LodgingDescription { get; set; }

        [Display(Name = "Foto")]
        [DataType(DataType.ImageUrl)]
        public string LodgingPhoto { get; set; }

        [Required]
        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        public decimal LodgingPrice { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        [DataType(DataType.Text)]
        public int LodgingCategory { get; set; }

        [Required]
        [Display(Name = "Moneda")]
        [DataType(DataType.Text)]
        public string LodgingCurrency { get; set; }

        [Required]
        [Display(Name = "Moneda")]
        [DataType(DataType.Text)]
        public string LodgingCurrencyCode { get; set; }

        [Required]
        [Display(Name = "ID Destino")]
        [DataType(DataType.Text)]
        public string DestinationId { get; set; }

        [Required]
        [Display(Name = "ID Proveedor")]
        [DataType(DataType.Text)]
        public string LodgingSupplierId { get; set; }

        [Required]
        [Display(Name = "Bajo Peticion")]
        [DataType(DataType.Text)]
        public bool LodgingUnderPetition { get; set; }

        [Required]
        [Display(Name = "Politicas de Cancelacion")]
        [DataType(DataType.Text)]
        public string LodgingCancelationPolitic { get; set; }
        public float? Latitud { get; set; }
        public float? Longitud { get; set; }
        public int LodgingTarifa { get; set; }
        public int LodgingBreakfast { get; set; }
        public bool TienePromocion { get; set; }
    }

    public class LodgingListModel
    {
        public bool Vacancies { get { return Lodgings.Count() > 0; } }

        public string DisplayType { get; set; }

        public List<LodgingModel> Lodgings { get; set; }
    }

    public class VacancyModel
    {
        public VacancyModel()
        {
            VacancyDates = new List<DateTime>();
            Rooms = new List<RoomModel>();
        }

        public List<DateTime> VacancyDates { get; set; }
        public List<RoomModel> Rooms { get; set; }

        [Required]
        [Display(Name = "ID Hotel")]
        [DataType(DataType.Text)]
        public string LodgingId { get; set; }

        [Required]
        [Display(Name = "Hotel")]
        [DataType(DataType.Text)]
        public string LodgingName { get; set; }

        [Display(Name = "Moneda")]
        [DataType(DataType.Text)]
        public string LodgingCurrency { get; set; }

        [Display(Name = "Seleccionado")]
        [DataType(DataType.Text)]
        public bool VacancySelected { get; set; }

        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        public string VacancyId { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [DataType(DataType.Text)]
        public string VacancyName { get; set; }

        [Required]
        [Display(Name = "Descripción Larga")]
        [DataType(DataType.Text)]
        public string VacancyDescription { get; set; }

        [Required]
        [Display(Name = "Checkin")]
        [DataType(DataType.Text)]
        public DateTime VacancyCheckin { get; set; }

        [Required]
        [Display(Name = "Checkout")]
        [DataType(DataType.Text)]
        public DateTime VacancyCheckout { get; set; }

        [Required]
        [Display(Name = "Disponibles")]
        [DataType(DataType.Text)]
        public int VacancyCount { get; set; }

        [Required]
        [Display(Name = "Reservadas")]
        [DataType(DataType.Text)]
        public int VacancyReserved { get; set; }

        [Required]
        [Display(Name = "Adultos")]
        [DataType(DataType.Text)]
        public int VacancyAdults { get; set; }

        [Required]
        [Display(Name = "Camas")]
        [DataType(DataType.Text)]
        public int VacancyBeds { get; set; }

        [Required]
        [Display(Name = "Precio por Unidad")]
        [DataType(DataType.Currency)]
        public decimal VacancyPrice { get; set; }

        [Display(Name = "Precio por Unidad Confirmado")]
        [DataType(DataType.Currency)]
        public decimal ConfirmedVacancyPrice { get; set; }

        [Display(Name = "Disponible")]
        [DataType(DataType.Text)]
        public bool Available { get; set; }
        public int Breakfast { get; set; }
        public int Tarifa { get; set; }
        public bool TienePromocionNxM { get; set; }
        public bool TienePromocionMinimoMaximo { get; set; }
        public int? MinimoNoches { get; set; }
        public int? MaximoNoches { get; set; }
    }

    public class RoomModel
    {
        public RoomModel()
        {
            Travelers = new List<TravelerModel>();
            ChildrenAges = new List<int>();
        }

        public List<TravelerModel> Travelers { get; set; }

        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        public string RoomId { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [DataType(DataType.Text)]
        public string RoomName { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        [DataType(DataType.Text)]
        public RoomType RoomType { get; set; }

        [Required]
        [Display(Name = "Código Tipo")]
        [DataType(DataType.Text)]
        public string RoomTypeCode { get; set; }

        [Required]
        [Display(Name = "Adultos")]
        [DataType(DataType.Text)]
        public int RoomAdults { get; set; }

        [Required]
        [Display(Name = "Camas")]
        [DataType(DataType.Text)]
        public int RoomBeds { get; set; }

        [Display(Name = "Edades Niños")]
        [DataType(DataType.Text)]
        public IEnumerable<int> ChildrenAges { get; set; }

        [Required]
        [Display(Name = "Disponibles")]
        [DataType(DataType.Text)]
        public int RoomCount { get; set; }

    }

    public class RoomListModel
    {
        public List<RoomModel> Rooms { get; set; }
    }

    public class VacancyListModel
    {
        public List<VacancyModel> Vacancies { get; set; }
    }

    public class UnitListModel
    {
        public Unit[] Units { get; set; }
    }

    public class Unit
    {
        public Guid IdUnidad { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreUnidad { get; set; }
        public int Personas { get; set; }
        public int Disponibles { get; set; }
        public string Description { get; set; }
        public decimal MontoPorUnidad { get; set; }
        public Quota[] Quota { get; set; }
    }

    public class Quota
    {
        public bool Activo { get; set; }
        public bool BloqueadoPorPromo { get; set; }
        public int CupoMaximo { get; set; }
        public int CupoReservado { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public Guid IdCupoUnidad { get; set; }
        public Guid IdUnidadAloj { get; set; }
        public int? MarcaTemporada { get; set; }
        public decimal Monto { get; set; }
    }
}