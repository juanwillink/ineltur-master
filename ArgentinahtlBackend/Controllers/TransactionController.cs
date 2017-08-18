using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArgentinahtlMVC.Models.Services;
using NPSWSClientCOM;
using ArgentinahtlMVC.Models;

namespace ArgentinahtlMVC.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/

        [HttpGet]
        public ActionResult CancelTransaction(long reservationCode)
        {
            return View(DbAccess.GetTransaction(reservationCode));
        }

        [HttpGet]
        public ActionResult ProcessTransactionCancellation(long reservationCode)
        {
            return View();
        }


        public ActionResult ChangePaymentMethod(long reservationCode)
        {
            //var client = new NPSWSClient();

            //using (var dc = new TurismoDataContext())
            //{
            //    int? attemptNumber = -1, error = 1;
            //    Guid? npsTransactionId = null;

            //    var transaction = dc.Transaccions.Single(t => t.CODIGO_RESERVA == reservationCode);

            //    dc.addTransaccionNPS(transaction.IDTRANSACCION, reservationCode.ToString(), "", "", ref npsTransactionId, ref attemptNumber);
            //    var npsTransaction = dc.TransaccionNPS.SingleOrDefault(tn => tn.IDTRANSACCION_NPS == npsTransactionId);

            //    var paymentMethod = dc.FormaPagos.SingleOrDefault(fp => fp.IDFORMAPAGO == Guid.Parse(reservation.PaymentMethodId));
            //    var currencyDb = dc.MonedaDBs.SingleOrDefault(m => m.DESCRIPCION == MapCurrencyFromNPS(SessionData.Reservation.LodgingCurrencyCode));

            //    var response = client.Authorize_3p(
            //            "2.2", "ineltur", "WEB", npsTransaction.REF_INELTUR + "-" + attemptNumber, npsTransaction.REF_INELTUR,
            //            Url.Action("ProcessResultPaymentNPS", "Payment", null, Request.Url.Scheme), "es_AR", Request.UrlReferrer.AbsoluteUri,
            //            (float)SessionData.Reservation.TotalAmount * currencyDb.COTIZACION, 1, 0, null,
            //            "032", "ARG", paymentMethod.DESCRIPCION, SessionData.Reservation.ReservationOwner.TravelerEmail, "amanzur@itiers.com",
            //            "Reserva en " + SessionData.Reservation.LodgingName, 3, "", DateTime.Now
            //        );

            //    dc.updateTransaccionNPS(npsTransactionId, npsTransaction.IDTRANSACCION, npsTransaction.NROINTENTO, response.MerchTxRef,
            //        response.TransactionId.ToString(), string.Format("{0}: {1} / {2}", response.ResponseCod, response.ResponseMsg, response.ResponseExtended), ref error);

            //    if (response.ResponseCod == 1)
            //    {
            //        SessionData.Reservation.PaymentMethodId = reservation.PaymentMethodId;
            //        return View(new NPSRedirectionModel { FrontPSP_URL = response.FrontPSP_URL });
            //    }
            //    else
            //        return Redirect(Request.UrlReferrer.AbsoluteUri);
            //}

            return View();
        }
    }
}
