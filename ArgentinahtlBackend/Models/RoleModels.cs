using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ArgentinahtlMVC.Models
{
    public class EntityModel
    {
        [Display(Name="Nombre")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime BornDate { get; set; }

        [Display(Name = "Activo")]
        public bool Enabled { get; set; }
    }

    public class LodgingModel : EntityModel
    {
        public LodgingModel()
        {
            Rooms = new List<RoomModel>();
        }

        public List<RoomModel> Rooms { get; set; }

        [Display(Name = "ID")]
        public Guid LodgingId { get; set; }
        
        public CurrencyModel Currency { get; set; }
    }

    public class RoomModel
    {
        [Display(Name = "ID")]
        public Guid RoomId { get; set; }

        [Display(Name = "Nombre")]
        public string RoomName { get; set; }

        [Display(Name = "Descripción")]
        public string RoomDescription { get; set; }

        [Display(Name = "Costo")]
        public float RoomCost { get; set; }

        [Display(Name = "Cupo")]
        public int RoomCupo { get; set; }

        [Display(Name = "Camas")]
        public int RoomCamas { get; set; }

        [Display(Name = "Personas")]
        public int RoomPersonas { get; set; }


    }

    public class AgencyModel : EntityModel
    {
        [Display(Name = "ID")]
        public Guid AgencyId { get; set; }

        public CurrencyModel Currency { get; set; }
    }

    public class PassengerModel : EntityModel
    {
        [Display(Name = "ID")]
        public Guid PassengerId { get; set; }
    }

    public class LodgingListModel
    {
        public LodgingListModel()
        {
            Lodgings = new List<LodgingModel>();
        }

        public List<LodgingModel> Lodgings { get; set; }
    }
}