using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CheckArgentina.Models;
using CheckArgentina.Managers;
using System.Text;
using CheckArgentina.Commons;

namespace CheckArgentina.Controllers
{
    public class NationalController : Controller
    {
        private Manager Manager { get { return new Managers.Manager(SearchType.National); } }
        
        //
        // GET: /National/

        public ActionResult NationalSearch()
        {
            return NationalSearch(null, null);
        }

        [HttpGet]
        public ActionResult NationalSearch(string userKey, string userName)
        {
            return NationalSearch(null, userKey, userName);
        }

        [HttpPost]
        public ActionResult NationalSearch(SearchModel searchModel, string userKey, string userName)
        {
            if (searchModel == null)
                searchModel = new SearchModel { ExtendedSearch = false, Checkin = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"), NightsCount = "1", Checkout = DateTime.Today.AddDays(4).ToString("yyyy-MM-dd") };

            if (Session["username"] != null)
            {
                SessionData.UserCredential = Manager.GetCredential(userKey);
                SessionData.User = ServiceManager.GetUser(userKey);
                searchModel.UserKey = Session["userkey"].ToString();
                searchModel.UserName = userName;
                searchModel.DestinationId = SessionData.User.DestinationId.ToString();
                searchModel.DestinationName = SessionData.User.DestinationName;
                searchModel.LodgingId = SessionData.User.LodgingId.ToString();
                searchModel.Lodging = SessionData.User.LodgingName;
                return View(searchModel);
            }

            LoginModel loginModel = new LoginModel();
            return PartialView("Login", loginModel);

        }

        public ActionResult SearchDestinations(string destinationName, string userKey)
        {
#if DEBUG
            if (string.IsNullOrEmpty(userKey))
                userKey = "1BB43EC1-2DBE-4DD7-ABD8-17890AFC0E69";
#endif

            SessionData.UserCredential = Manager.GetCredential(userKey);
            if (SessionData.User.UserId == Guid.Empty)
                SessionData.User = ServiceManager.GetUser(userKey);

            if (destinationName == null)
                destinationName = string.Empty;

            return Json(Manager.SearchDestination(destinationName, SessionData.UserCredential).Destinations, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ValidateInput(false)]
        public ActionResult NationalSearchResults(string destinationId, string lodgingName, DateTime checkin, DateTime checkout,
            string adults, int children, string userKey, string order, string nationality)
        {
            return NationalSearchResults(new SearchLodgingRequestModel
            {
                DestinationId = destinationId,
                LodgingName = lodgingName,
                Checkin = checkin,
                Checkout = checkout,
                UserKey = userKey,
                Rooms = new List<SearchLodgingModel>()
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult NationalSearchResults(SearchLodgingRequestModel searchLodgingRequestModel)
        {
            SessionData.UserCredential = Manager.GetCredential(searchLodgingRequestModel.UserKey);
            if (SessionData.User.UserId == Guid.Empty)
                SessionData.User = ServiceManager.GetUser(searchLodgingRequestModel.UserKey);

            int adultCount = 0, bedCount = 0;

            if (searchLodgingRequestModel.Rooms == null || searchLodgingRequestModel.Rooms.Count() == 0)
            {


                searchLodgingRequestModel.Rooms = new List<SearchLodgingModel>{
                    //new SearchLodgingModel{
                    //    Count = 1,
                    //    RoomTypeCode = "1 - Single"
                    //}
                    //new SearchLodgingModel{
                    //    Count = 1,
                    //    RoomTypeCode = "1 - DSU"
                    //},
                    new SearchLodgingModel{
                        Count = 1,
                        RoomTypeCode = "2 - Double"
                    }
                    //new SearchLodgingModel{
                    //    Count = 1,
                    //    RoomTypeCode = "1 - Twin"
                    //},
                    //new SearchLodgingModel{
                    //    Count = 1,
                    //    RoomTypeCode = "1 - Triple"
                    //},
                    //new SearchLodgingModel{
                    //    Count = 1,
                    //    RoomTypeCode = "1 - Quad"
                    //}
                };
                
            }

            foreach (var room in searchLodgingRequestModel.Rooms)
            {
                switch (room.RoomTypeCode)
                {
                    case "1 - Single":
                        room.RoomType = RoomType.Single;
                        adultCount += 1;
                        bedCount += 1;
                        break;
                    case "1 - DSU":
                        room.RoomType = RoomType.DSU;
                        adultCount += 1;
                        bedCount += 1;
                        break;
                    case "2 - Double":
                        room.RoomType = RoomType.Double;
                        adultCount += 2;
                        bedCount += 1;
                        break;
                    case "2 - Twin":
                        room.RoomType = RoomType.Twin;
                        adultCount += 2;
                        bedCount += 2;
                        break;
                    case "3 - Triple":
                        room.RoomType = RoomType.Triple;
                        adultCount += 3;
                        bedCount += 3;
                        break;
                    case "4 - Quad":
                        room.RoomType = RoomType.Quad;
                        adultCount += 4;
                        bedCount += 4;
                        break;
                }
            }

            searchLodgingRequestModel.DestinationType = string.IsNullOrEmpty(searchLodgingRequestModel.DestinationId)  ? "NoEspecificado" : "Ciudad";

            var results = Manager.SearchLodging(searchLodgingRequestModel, SessionData.UserCredential);
            
            var model = new LodgingListModel();
            model.Lodgings = results.Lodgings.Where(l => l.Vacancies.Where(v => v.VacancyAdults == adultCount && v.VacancyBeds == bedCount).Count() > 0).ToList();
            model.Lodgings.ForEach(l => l.Vacancies = l.Vacancies.Where(v => v.VacancyAdults == adultCount && v.VacancyBeds == bedCount).ToList());

            if (searchLodgingRequestModel.DisplayType == "tar")
            {
                model.DisplayType = "tar";
            }
            else if (searchLodgingRequestModel.DisplayType == "det")
            {
                model.DisplayType = "det";
            }
            ModelState.Clear();

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult SearchLodgingInfo(SearchLodgingRequestModel searchLodgingRequestModel)
        {
            SessionData.UserCredential = Manager.GetCredential(searchLodgingRequestModel.UserKey);
            if (SessionData.User.UserId == Guid.Empty)
                SessionData.User = ServiceManager.GetUser(searchLodgingRequestModel.UserKey);

            var model = Manager.SearchLodgingInfo(searchLodgingRequestModel, SessionData.UserCredential);

            ModelState.Clear();

            return View(model);
        }

        public ActionResult SearchLodgingInfoJson(string lodgingId)
        {
            SessionData.UserCredential = Manager.GetCredential(Session["userkey"].ToString());
            if (SessionData.User.UserId == Guid.Empty)
                SessionData.User = ServiceManager.GetUser(Session["userkey"].ToString());
            SearchLodgingRequestModel searchLodgingRequestModel = new SearchLodgingRequestModel()
            {
                LodgingId = lodgingId
            };
            var model = Manager.SearchLodgingInfo(searchLodgingRequestModel, SessionData.UserCredential);

            ModelState.Clear();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelReservation(string reservationCode)
        {
            SessionData.UserCredential = Manager.GetCredential(Session["userkey"].ToString());
            if (SessionData.User.UserId == Guid.Empty)
                SessionData.User = ServiceManager.GetUser(Session["userkey"].ToString());
            var respuesta = Manager.CancelReservation(reservationCode, SessionData.UserCredential);
            return RedirectToAction("BuscarMisReservas", "Login");
        }

        

    }
}
