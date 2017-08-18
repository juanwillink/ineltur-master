using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ArgentinahtlBackend
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Users",
                "Users/{action}",
                new { controller = "Users", action = "LogIn" }
            );

            routes.MapRoute(
                "ReportsTransactionList",
                "Reports/TransactionList/{userId}",
                new { controller = "Reports", action = "TransactionList", userId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ReportsTransactionListFiltered",
                "Reports/TransactionListFiltered/{filterModel}",
                new { controller = "Reports", action = "TransactionListFiltered", filterModel = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ReportsDefault",
                "Reports/{action}/{reservationCode}",
                new { controller = "Reports", action = "TransactionDetails", reservationCode = UrlParameter.Optional }
            );

            routes.MapRoute(
                "TransactionDefault",
                "Transaction/{action}/{reservationCode}",
                new { controller = "Transaction", action = "CancelTransaction", reservationCode = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingGetCities",
                "Lodging/GetCities/{provinceId}",
                new { controller = "Lodging", action = "GetCities", provinceId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingGetLodgings",
                "Lodging/GetLodgings/{cityId}",
                new { controller = "Lodging", action = "GetLodgings", cityId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingSearchLodgings",
                "Lodging/SearchLodgings/{lodgingName}",
                new { controller = "Lodging", action = "SearchLodgings", lodgingName = UrlParameter.Optional }
            );

            routes.MapRoute(
               "RateGetRooms",
               "Rate/GetRooms/{lodgingId}",
               new { controller = "Rate", action = "GetRooms", lodgingId = UrlParameter.Optional }
           );

            routes.MapRoute(
               "RateCreateRate",
               "Rate/CreateRate/{roomId}",
               new { controller = "Rate", action = "CreateRate", roomId = UrlParameter.Optional }
           );

            routes.MapRoute(
               "RateEditRate",
               "Rate/EditRate/{rateId}",
               new { controller = "Rate", action = "EditRate", rateId = UrlParameter.Optional }
           );

            routes.MapRoute(
                "LodgingCreateSeason",
                "Lodging/CreateSeason/{lodgingId}",
                new { controller = "Lodging", action = "CreateSeason", seasonId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingCreatePromocion",
                "Lodging/CreatePromocion/{lodgingId}",
                new { controller = "Lodging", action = "CreatePromocion", lodgingId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingCreateTarifa",
                "Lodging/CreateTarifa/{lodgingId}",
                new { controller = "Lodging", action = "CreateTarifa", lodgingId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingSeasonsDefault",
                "Lodging/{action}/{seasonId}",
                new { controller = "Lodging", action = "EditSeason", seasonId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingTarifaDefault",
                "Lodging/EditTarifa/{tarifaId}",
                new { controller = "Lodging", action = "EditTarifa", tarifaId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "LodgingDefault",
                "Lodging/{action}/{lodgingId}",
                new { controller = "Lodging", action = "Menu", lodgingId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Management", action = "Menu", id = UrlParameter.Optional }
            );
        }
    }
}
