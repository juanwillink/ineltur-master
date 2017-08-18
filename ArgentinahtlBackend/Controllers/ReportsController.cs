using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArgentinahtlMVC.Models;
using ArgentinahtlMVC.Models.Services;

namespace ArgentinahtlMVC.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/

        public ActionResult Transactions(Guid? userId)
        {
            var model = new TransactionsModel
            {
                UserId = userId ?? SessionData.UserId,
                Provinces = DbAccess.GetProvinces(),
                Statuses = Enum.GetValues(typeof(ReservationStatus)).Cast<ReservationStatus>().ToList(),
                Agencys = DbAccess.GetAgencys().Where(a => a.Enabled).ToList()
            };

            return View(model);
        }
        //
        // GET: /Reports/

        public ActionResult TransactionList(Guid? userId)
        {
            var model = new TransactionListModel
            {
                Transactions = DbAccess.GetTransactions(userId ?? SessionData.UserId)
            };

            return View(model);
        }

        public ActionResult TransactionListFiltered(TransactionsModel filterModel)
        {
            var model = new TransactionListModel
            {
                Transactions = DbAccess.GetTransactions(filterModel)
            };

            return View("TransactionList", model);
        }

        [HttpGet]
        public ActionResult TransactionDetails(long reservationCode)
        {
            var model = new ReservationListModel
            {
                Reservations = DbAccess.GetReservationsByReservationCode(reservationCode),
                ReservationCode = reservationCode
            };

            return PartialView(model);
        }
    }
}
