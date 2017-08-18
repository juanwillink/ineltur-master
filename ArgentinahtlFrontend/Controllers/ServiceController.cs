using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CheckArgentina.Models;
using CheckArgentina.Managers;
using CheckArgentina.Commons;

namespace CheckArgentina.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lodging(LodgingModel lodging)
        {
            return View(lodging);
        }

        public bool CompleteReservation()
        {
            try
            {
                var manager = new Manager(SessionData.SearchType);

                var reservation = new ReservationModel();

                if (!string.IsNullOrEmpty(SessionData.Reservation.SecondaryUserName) || !string.IsNullOrEmpty(SessionData.Reservation.SecondaryUserPass))
                {
                    Credential secondaryUserCredential = new Credential()
                    {
                        Username = SessionData.Reservation.SecondaryUserName,
                        Password = SessionData.Reservation.SecondaryUserPass,
                        Language = "es"
                    };
                    reservation = manager.Reserve(SessionData.Reservation, secondaryUserCredential);
                }
                else
                {
                    reservation = manager.Reserve(SessionData.Reservation, SessionData.UserCredential);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
