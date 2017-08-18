using System.Collections.Generic;
using System.Web.Mvc;
using System.Globalization;
using System.Linq;
using CheckArgentina.Models;
using CheckArgentina.Commons;
using CheckArgentina.Managers;
using System;
using System.Net;
using System.Net.Mail;
using NPSWSClientCOM;


namespace CheckArgentina.Controllers
{
    public class PaymentController : Controller
    {
        public ActionResult Confirmation(ReservationModel reservation)
        {
            var manager = new Manager(SessionData.SearchType);

            reservation.Vacancies = reservation.Vacancies;

            reservation = manager.ConfirmAvailability(reservation, SessionData.UserCredential);

            reservation.Countries = SessionData.Countries;

            reservation.PaymentMethods = SessionData.PaymentMethods;

            SessionData.Reservation = reservation;

            foreach (var vacancy in reservation.Vacancies)
            {
                if (vacancy.TienePromocionNxM)
                {
                    using (var dc = new TurismoDataContext())
                    {
                        var promocion = dc.Promociones_Alojamientos.SingleOrDefault(p => p.IDUNIDADPROMO.ToString() == vacancy.VacancyId && 
                                        p.DIASRESERVADOS == (reservation.Vacancies.FirstOrDefault().VacancyCheckout - reservation.Vacancies.FirstOrDefault().VacancyCheckin).TotalDays);
                        var nochesARestar = promocion.DIASRESERVADOS - promocion.DIASACOBRAR;
                        SessionData.Reservation.PromotionPrice = SessionData.Reservation.TotalAmount;
                        for (int i = 0; i < nochesARestar; i++)
                        {
                            SessionData.Reservation.PromotionPrice = SessionData.Reservation.PromotionPrice - vacancy.VacancyPrice;
                        }
                    }
                    
                }
                if (vacancy.TienePromocionMinimoMaximo)
                {

                }
            }

            ModelState.Clear();

            return Json(reservation, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BorrarHabitacion(string vacancyId)
        {
            int index = 0;
            int i = 0;
            foreach (var vacancy in SessionData.Reservation.Vacancies)
            {
                if (vacancy.VacancyId == vacancyId)
                {
                    index = i;
                }
                i++;
            }
            SessionData.Reservation.Vacancies.RemoveAt(index);
            return Json(SessionData.Reservation, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddPassenger(string travelerName, string travelerLastName, string travelerCountry, string travelerIdType, string travelerIdNumber, string vacancyId)
        {
            var traveler = new TravelerModel();
            if (string.IsNullOrEmpty(travelerIdNumber))
            {
                travelerIdNumber = "0";
            }
            if (SessionData.Reservation.ReservationOwner == null)
            {
                var reservationOwner = new TravelerModel()
                {
                    TravelerFirstName = travelerLastName,
                    TravelerLastName = travelerLastName,
                    TravelerCountry = travelerCountry,
                    TravelerIdType = IdType.DNI,
                    TravelerId = travelerIdNumber
                };
                SessionData.Reservation.ReservationOwner = reservationOwner;
            }
            foreach (var vacancy in SessionData.Reservation.Vacancies)
            {
                if (vacancy.VacancyId == vacancyId)
                {

                    traveler.TravelerFirstName = travelerName;
                    traveler.TravelerLastName = travelerLastName;
                    traveler.TravelerCountry = travelerCountry;
                    traveler.TravelerIdType = IdType.DNI;
                    traveler.TravelerId = travelerIdNumber;
                    vacancy.Rooms.Min().Travelers.Add(traveler);
                }
            }
            return Json(traveler, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarPasajero(string nombre, string apellido)
        {
            string nombrePasajero = nombre + "-" + apellido;
            int index = 0;
            foreach (var vacancy in SessionData.Reservation.Vacancies)
            {
                int i = 0;
                bool found = false;
                foreach (var traveler in vacancy.Rooms.Min().Travelers)
                {
                    string travelerName = traveler.TravelerFirstName + "-" + traveler.TravelerLastName;
                    if (travelerName == nombrePasajero)
                    {
                        index = i;
                        found = true;
                    }
                    i++;
                }
                if (found)
                {
                    vacancy.Rooms.Min().Travelers.RemoveAt(index);
                } 
            }
            return Json(SessionData.Reservation, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OpenConfirmation()
        {
            return View("~/Views/Shared/Reservation.cshtml", SessionData.Reservation);
        }

        public ActionResult AddSecondaryUser(string SecondaryUserName, string SecondaryUserPass)
        {
            if (!string.IsNullOrEmpty(SecondaryUserName) || !string.IsNullOrEmpty(SecondaryUserPass)) 
            {
                SessionData.Reservation.SecondaryUserName = SecondaryUserName;
                SessionData.Reservation.SecondaryUserPass = SecondaryUserPass;
            }
            return null;
        }

        public ActionResult PaymentMethods()
        {
            //if (SessionData.Reservation == null || reservationModel == null)
            //    RedirectToAction("PaymentError");

            //SessionData.Reservation.LodgingId = reservationModel.LodgingId;
            SessionData.Reservation.ReservationOwner = SessionData.Reservation.Vacancies.Min().Rooms.Min().Travelers.Min();
            //SessionData.Reservation.PaymentMethodId = reservationModel.PaymentMethodId;
            //SessionData.Reservation.Observations = reservationModel.Observations;
            //SessionData.Reservation.Vacancies = reservationModel.Vacancies;
            var manager = new Manager(SessionData.SearchType);
            SessionData.Reservation = manager.BlockVacancies(SessionData.Reservation, SessionData.UserCredential);
            SessionData.Reservation.DiasCancelacionCargo = manager.GetDiasCancelacionCargo(Guid.Parse(SessionData.Reservation.LodgingId));
            {

            }
            //return Redirect("http://200.85.184.11/SimuladorPagos/HSBC.aspx");
            //return RedirectToAction("InternalPaymentSimulator");

            ModelState.Clear();

            if (!SessionData.User.UsesPaymentInterface)
            {
                // Se utiliza la forma de pago por defecto del Web Service
                SessionData.Reservation.PaymentMethodId = Guid.Empty.ToString();
                return RedirectToAction("PaymentSuccess");
            }

            return View(SessionData.Reservation);
        }

        public ActionResult PaymentSuccess()
        {
            return View(SessionData.Reservation);
        }

        public ActionResult PaymentError()
        {
            return View();
        }

        public ActionResult InternalPaymentSimulator()
        {
            return View();
        }

        public ActionResult ProcessPaymentSimulation(string processAction, string clave)
        {
            if(processAction == "Accept" && clave == "ad123ar")
                return RedirectToAction("PaymentSuccess");
            else
                return RedirectToAction("PaymentError");
        }

        public ActionResult ProcessPaymentNPS(ReservationModel reservation)
        {
            var client = new NPSWSClient();
            
            using(var dc = new TurismoDataContext())
            {
                int? attemptNumber = -1, error = 1;
                Guid? npsTransactionId = null;

                //var reservationDb = dc.Transaccions.Single(t => t.IDTRANSACCION == Guid.Parse(reservation.ReservationId));

                dc.addTransaccionNPS(null, SessionData.Reservation.ReservationId, "", "", ref npsTransactionId, ref attemptNumber);
                var npsTransaction = dc.TransaccionNPS.SingleOrDefault(tn => tn.IDTRANSACCION_NPS == npsTransactionId);

                var paymentMethod = dc.FormaPagos.SingleOrDefault(fp => fp.IDFORMAPAGO == Guid.Parse(reservation.PaymentMethodId));
                var currencyDb = dc.MonedaDBs.SingleOrDefault(m => m.DESCRIPCION == MapCurrencyFromNPS(SessionData.Reservation.LodgingCurrencyCode));

                var response = client.Authorize_3p(
                        "2.2", "ineltur", "WEB", npsTransaction.REF_INELTUR + "-" + attemptNumber, npsTransaction.REF_INELTUR,
                        Url.Action("BridgeProcessResultPaymentNPS", "Payment", null, Request.Url.Scheme), "es_AR", Request.UrlReferrer.AbsoluteUri,
                        (float)SessionData.Reservation.TotalAmount * currencyDb.COTIZACION, 1, 0, null,
                        "032", "ARG", paymentMethod.DESCRIPCION, SessionData.Reservation.ReservationOwner.TravelerEmail, Config.LeerSetting("MailReservation"),
                        "Reserva en " + SessionData.Reservation.LodgingName, 3, "", DateTime.Now
                    );

                dc.updateTransaccionNPS(npsTransactionId, npsTransaction.IDTRANSACCION, npsTransaction.NROINTENTO, response.MerchTxRef,
                    response.TransactionId.ToString(), string.Format("{0}: {1} / {2}", response.ResponseCod, response.ResponseMsg, response.ResponseExtended), ref error);

                if (response.ResponseCod == 1)
                {
                    SessionData.Reservation.PaymentMethodId = reservation.PaymentMethodId;
                    return View(new NPSRedirectionModel { FrontPSP_URL = response.FrontPSP_URL });
                }
                else
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
        }

        public ActionResult BridgeProcessResultPaymentNPS(NPSPaymentModel model)
        {
            return View(model);
        }

        public ActionResult ProcessResultPaymentNPS(NPSPaymentModel model)
        {
            var client = new NPSWSClient();

            if(new ServiceController().CompleteReservation())
            {
                using (var dc = new TurismoDataContext())
                {
                    int? attemptNumber = -1;
                    Guid? npsTransactionId = null;
                    var npsTransaction = dc.TransaccionNPS.SingleOrDefault(tn => tn.REF_INELTUR == model.psp_MerchTxRef);
                    npsTransaction.IDTRANSACCION = Guid.Parse(SessionData.Reservation.ReservationId);

                    dc.SubmitChanges();

                    var response = client.Capture_3p("2.2", "ineltur", "WEB", SessionData.Reservation.ReservationCode + "-" + (npsTransaction.NROINTENTO + 1), Convert.ToInt64(model.psp_TransactionId),
                    (float)SessionData.Reservation.TotalAmount, "ineltur", DateTime.Now);

                    dc.addTransaccionNPS(npsTransaction.IDTRANSACCION, model.psp_MerchTxRef,
                        model.psp_TransactionId, string.Format("{0}: {1} / {2}", response.ResponseCod, response.ResponseMsg, response.ResponseExtended), ref npsTransactionId, ref attemptNumber);
                
                    if (response.ResponseCod == 0)
                        return RedirectToAction("PaymentSuccess");
                    else
                        return RedirectToAction("PaymentError");
                }
            }
            else
                return RedirectToAction("PaymentError");
        }

        public ActionResult SendEmailReservation(EmailReservationModel model)
        {
            model.TravelerId = !string.IsNullOrEmpty(model.TravelerId) ? model.TravelerId : "0"; 

            using (var smtp = ObtenerClienteSmtp())
            {
                FluentEmail.Email
                .From(model.TravelerEmail)
                .Subject(string.Format("Reserva en {0}", model.LodgingName))
                .To("mjjara@argentinahtl.com")
                .Body("Datos de la reserva: <br>"+
                    "Nombre del Hotel: " + model.LodgingName + "<br>" +
                    "Ciudad: " + model.DestinationName + "<br>" +
                    "Nombre del Pasajero: " + model.TravelerName + "<br>" +
                    "Nacionalidad del Pasajero: " + model.Nationality + "<br>" +
                    "Dni del Pasajero: " + model.TravelerId + "<br>" +
                    "Cantidad de Pasajeros: " + model.TravelersCount + "<br>" +
                    "Checkin: " + model.CheckinDate + "<br>" +
                    "Checkout: " + model.CheckoutDate + "<br>" +
                    "Tipo de Habitacion: " + model.RoomType + "<br>" +
                    "Cantidad de Habitaciones: " + model.RoomsCount + "<br>" +
                    "Observaciones: " + model.Observations)
                .UsingClient(smtp)
                .Send();
            }
            return View("ReservationEmailSent", model);
#if true

#endif
        }

        public ActionResult ProcessPaymentCtaCte()
        {
            SessionData.Reservation.PaymentMethodId = "7d5192ca-fe10-455e-b051-d1023a07ba75";
            if (new ServiceController().CompleteReservation())
                return RedirectToAction("PaymentSuccess");
            else
                return RedirectToAction("PaymentError");

        }

        public ActionResult ProcessPaymentDeposito()
        {
            SessionData.Reservation.PaymentMethodId = "b8b3354a-4cd1-47dc-8267-707fd80d3072";
            if (new ServiceController().CompleteReservation())
                return RedirectToAction("PaymentSuccess");
            else
                return RedirectToAction("PaymentError");
        }

        private string MapCurrencyFromNPS(string code)
        {
            string currency = "ARS";

            switch (code)
            {
                case "032": currency = "ARS"; break;
                case "978": currency = "EUR"; break;
                case "840": currency = "USD"; break;
            }

            return currency;
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
    }
}
