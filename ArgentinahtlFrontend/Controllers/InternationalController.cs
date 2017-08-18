using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CheckArgentina.Managers;
using CheckArgentina.Models;
using CheckArgentina.Commons;

namespace CheckArgentina.Controllers
{
    public class InternationalController : Controller
    {
        private Manager Manager { get { return new Managers.Manager(SearchType.International); } }

        //
        // GET: /International/

        public ActionResult InternationalSearch()
        {
            return InternationalSearch(null);
        }


        [HttpPost]
        public ActionResult InternationalSearch(SearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new SearchModel { ExtendedSearch = false, Checkin = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"), NightsCount = "1", Checkout = DateTime.Today.AddDays(4).ToString("yyyy-MM-dd") };

            return View(searchModel);
        }

        public ActionResult SearchDestinations(string destinationName, string destinationParentId, string destinationParentName, string destinationParentType, string userKey)
        {
#if DEBUG
            if (string.IsNullOrEmpty(userKey))
                userKey = "1BB43EC1-2DBE-4DD7-ABD8-17890AFC0E69";
#endif

            SessionData.UserCredential = Manager.GetCredential(userKey);

            if (destinationName == null)
                destinationName = string.Empty;

            DestinationModel parent = null;

            if (!(string.IsNullOrWhiteSpace(destinationParentId) || string.IsNullOrWhiteSpace(destinationParentName) || string.IsNullOrWhiteSpace(destinationParentType)))
            {
                parent = new DestinationModel
                {
                    DestinationId = destinationParentId,
                    DestinationName = destinationParentName,
                    DestinationType = (DestinationType)Enum.Parse(typeof(DestinationType), destinationParentType)
                };
            }

            return Json(Manager.SearchDestination(destinationName, SessionData.UserCredential, parent).Destinations, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ValidateInput(false)]
        public ActionResult InternationalSearchResults(string destinationId, string lodgingName, DateTime checkin, DateTime checkout,
            string adults, int children, string userKey)
        {
            return InternationalSearchResults(new SearchLodgingRequestModel
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
        public ActionResult InternationalSearchResults(SearchLodgingRequestModel searchLodgingRequest)
        {
            //SessionData.UserCredential = Manager.GetCredential(searchLodgingRequest.UserKey);

            //int room1 = 0, room2 = 0, room3 = 0, room4 = 0, room5 = 0, room6 = 0, adultCount = 0, bedCount = 0;

            if (searchLodgingRequest.Rooms == null || searchLodgingRequest.Rooms.Count() == 0)
            {
                searchLodgingRequest.Rooms = new List<SearchLodgingModel>{
                    new SearchLodgingModel{
                        Count = 1,
                        RoomTypeCode = "NMO.HTL.RMT.SGL"
                    }
                };
            }

            if(searchLodgingRequest.Rooms != null)
            {
                foreach (var room in searchLodgingRequest.Rooms)
                {
                    switch (room.RoomTypeCode)
                    {
                        case "NMO.HTL.RMT.SGL":
                            room.RoomType = RoomType.Single;
                            //room1 = 1;
                            //adultCount = 1;
                            //bedCount = 1;
                            break;
                        case "NMO.HTL.RMT.DBL.TSU":
                            room.RoomType = RoomType.DSU;
                            //room1 = 1;
                            //adultCount = 1;
                            //bedCount = 1;
                            break;
                        case "NMO.HTL.RMT.DBL":
                            room.RoomType = RoomType.Double;
                            //room2 = 1;
                            //adultCount = 2;
                            //bedCount = 1;
                            break;
                        case "NMO.HTL.RMT.DBL.TWN":
                            room.RoomType = RoomType.Twin;
                            //room2 = 1;
                            //adultCount = 2;
                            //bedCount = 2;
                            break;
                        case "NMO.HTL.RMT.TPL":
                            room.RoomType = RoomType.Triple;
                            //room3 = 1;
                            //adultCount = 3;
                            //bedCount = 3;
                            break;
                        case "NMO.HTL.RMT.QUA":
                            room.RoomType = RoomType.Quad;
                            //room4 = 1;
                            //adultCount = 4;
                            //bedCount = 4;
                            break;
                        case "NMO.HTL.RMT.PEN":
                            room.RoomType = RoomType.Quintuple;
                            //room5 = 1;
                            //adultCount = 5;
                            //bedCount = 5;
                            break;
                        case "NMO.HTL.RMT.HEX":
                            room.RoomType = RoomType.Sextuple;
                            //room6 = 1;
                            //adultCount = 6;
                            //bedCount = 6;
                            break;
                    }
                }
            }

            var results = Manager.SearchLodging(searchLodgingRequest, SessionData.UserCredential);

            var model = new LodgingListModel();
            //model.Lodgings = results.Lodgings.Where(l => l.Vacancies.Where(v => v.VacancyAdults == adultCount && v.VacancyBeds == bedCount).Count() > 0).ToList();
            //model.Lodgings.ForEach(l => l.Vacancies = l.Vacancies.Where(v => v.VacancyAdults == adultCount && v.VacancyBeds == bedCount).ToList());
            model.Lodgings = results.Lodgings;

            return View(model);
        }

    }
}
