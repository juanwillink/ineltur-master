using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CheckArgentina.Models;
using CheckArgentina.Managers;
using System.Text;
using CheckArgentina.Commons;
using System.IO;
using CheckArgentina.NationalService;
using System.Net.Mail;
using System.Net;
using System.Globalization;

namespace CheckArgentina.Controllers
{
    public class LoginController : Controller
    {
        private LocalManager Manager { get { return new LocalManager(); } }

        public ActionResult LoginAttemp(LoginModel loginModel)
        {
            var petitionResults = Manager.LoginAttemp(loginModel);

            if (petitionResults.Estado == (int)EstadoRespuesta.Ok)
            {
                Session["username"] = loginModel.Nombre;
                Session["userkey"] = petitionResults.UID;
            }
            else
            {
                loginModel.UID = null;
            }
                
              

            return RedirectToAction("Home", "Home");

        }

        public ActionResult BuscarMisReservas(string reservationCode, string searchParameter)
        {
            if (Session["userkey"] != null)
            {
                if (string.IsNullOrEmpty(searchParameter))
                {
                    searchParameter = "busqNuevas";
                }
                var userKeyGuid = Guid.Parse(Session["userkey"].ToString());
                int reservationCodeInt = 0;
                if (!string.IsNullOrEmpty(reservationCode))
                {
                    reservationCodeInt = int.Parse(reservationCode);
                }
                var petitionResults = Manager.SearchMyReservations(userKeyGuid, reservationCodeInt, searchParameter);
                return View("MyReservations", petitionResults);
            }
            else
            {
                return View("NotLoggedIn");
            }
        }

        public ActionResult Registration(RegistrationModel model)
        {
            using (var smtp = ObtenerClienteSmtp())
            {
                FluentEmail.Email
                .From(Config.LeerSetting("MailAccountFrom"))
                .Subject(string.Format("Alta de usuario {0}", model.BuissnessName))
                .To("mjjara@argentinahtl.com")
                .Body("Datos del Alta: <br>" +
                    "Nombre de la Empresa: " + model.BuissnessName + "<br>" +
                    "Razon Social: " + model.RazonSocial + "<br>" +
                    "Tipo de factura: " + model.BillType + "<br>" +
                    "CUIT: " + model.Cuit + "<br>" +
                    "Direccion: " + model.Adress + "<br>" +
                    "Codigo Postal: " + model.ZipCode + "<br>" +
                    "Telefono: " + model.Phone + "<br>" +
                    "Mail de confirmacion de reserva: " + model.ReservationConfirmationEmail + "<br>" +
                    "Persona a cargo: " + model.PersonInChargeName + "<br>" +
                    "Email de la persona a cargo: " + model.PersonInChargeEmail + "<br>" +
                    "Newsletter: " + model.Newsletter + "<br>" +
                    "Como llego a nosotros: " + model.HowDidYouFindUs)
                .UsingClient(smtp)
                .Send();
            }
            return View("RegistrationEmailSent");
        }

        

        private static SmtpClient ObtenerClienteSmtp()
        {
            var smtp = new SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = Config.LeerSetting("MailUseSSL", false);
            smtp.Host = Config.LeerSetting("MailServer");
            smtp.Port = Config.LeerSetting("MailPort", 25);

            string user = Config.LeerSetting("MailUser");

            if (!String.IsNullOrEmpty(user))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Config.LeerSetting("MailUser"),
                        Config.LeerSetting("MailPassword"));
            }
            return smtp;
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Home", "Home");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult SearchLodgingWeeklyPrices(Guid lodgingId, string dateString)
        {
            DateTime date = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(dateString))
            {
                var values = dateString.Split('/');
                date = new DateTime(int.Parse(values[2]), int.Parse(values[1]), int.Parse(values[0]));
            }

            SessionData.UserCredential = Manager.GetCredential(Session["userkey"].ToString());
            if (SessionData.User.UserId == Guid.Empty)
            {
                SessionData.User = ServiceManager.GetUser(Session["userkey"].ToString());
                
            }
            return Json(Manager.GetLodgingWeeklyPrices(lodgingId, date, SessionData.UserCredential), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult SearchHotels(SearchLodgingRequestModel searchLodgingRequestModel)
        {
            SessionData.UserCredential = Manager.GetCredential(Session["userkey"].ToString());
            if (SessionData.UserCredential.Username == null)
            {
                return View("Home");
            }
            searchLodgingRequestModel.DestinationType = string.IsNullOrEmpty(searchLodgingRequestModel.DestinationId) ? "NoEspecificado" : "Ciudad";
            if (searchLodgingRequestModel.Rooms != null)
            {
                foreach (var room in searchLodgingRequestModel.Rooms)
                {
                    switch (room.RoomTypeCode)
                    {
                        case "single":
                            room.RoomType = RoomType.Single;
                            break;
                        case "doble":
                            room.RoomType = RoomType.Double;
                            break;
                        case "triple":
                            room.RoomType = RoomType.Triple;
                            break;
                        case "cuadruple":
                            room.RoomType = RoomType.Quad;
                            break;
                        case "":
                            break;
                    }
                }
            } 
            var results = Manager.SearchHotels(searchLodgingRequestModel, SessionData.UserCredential);
            if (results != null)
            {
                var model = new LodgingListModel();
                model.Lodgings = results.Lodgings;
                if (searchLodgingRequestModel.DisplayType == "tar")
                {
                    model.DisplayType = "tar";
                }
                else
                {
                    model.DisplayType = "det";
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }
    }
}