using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CheckArgentina
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region National
            routes.MapRoute(
                "NationalSearchDestinations", // Route name
                "National/SearchDestinations/{destinationName}/{parent}", // URL with parameters
                new { controller = "National", action = "SearchDestinations", destinationName = UrlParameter.Optional, parent = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "NationalSearchResults", // Route name
                "National/NationalSearchResults/{destinationId}/{lodgingName}/{checkin}/{checkout}/" +
                "{adults}/{children}/{userKey}", // URL with parameters
                new
                {
                    controller = "National",
                    action = "NationalSearchResults",
                    destinationId = UrlParameter.Optional,
                    lodgingName = UrlParameter.Optional,
                    checkin = UrlParameter.Optional,
                    checkout = UrlParameter.Optional,
                    adults = UrlParameter.Optional,
                    children = UrlParameter.Optional,
                    userKey = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                "NationalSearchOnlyUserKey", // Route name
                "National/NationalSearch/{userKey}", // URL with parameters
                new { controller = "National", action = "NationalSearch", userKey = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "NationalSearch", // Route name
                "National/NationalSearch/{searchModel}/{userKey}", // URL with parameters
                new { controller = "National", action = "NationalSearch", searchModel = UrlParameter.Optional, userKey = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "National", // Route name
                "National/{action}", // URL with parameters
                new { controller = "National", action = "NationalSearch" } // Parameter defaults
            );
            #endregion

            #region International
            routes.MapRoute(
                "InternationalSearchDestinations", // Route name
                "International/SearchDestinations/{destinationName}/{parent}", // URL with parameters
                new { controller = "International", action = "SearchDestinations", destinationName = UrlParameter.Optional, parent = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "InternationalSearchResults", // Route name
                "International/InternationalSearchResults/{destinationId}/{lodgingName}/{checkin}/{checkout}/" +
                "{adults}/{children}/{userKey}", // URL with parameters
                new
                {
                    controller = "International",
                    action = "InternationalSearchResults",
                    destinationId = UrlParameter.Optional,
                    lodgingName = UrlParameter.Optional,
                    checkin = UrlParameter.Optional,
                    checkout = UrlParameter.Optional,
                    adults = UrlParameter.Optional,
                    children = UrlParameter.Optional,
                    userKey = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                "International", // Route name
                "International/{action}", // URL with parameters
                new { controller = "International", action = "InternationalSearch" } // Parameter defaults
            );
            #endregion

            #region Aereal
            routes.MapRoute(
                "Aereal", // Route name
                "Aereal/{action}", // URL with parameters
                new { controller = "Aereal", action = "AerealSearch" } // Parameter defaults
            );

            routes.MapRoute(
                "searchA", // Route name
                "Aereal/searchA/{q}/{limit}/{timestamp}", // URL with parameters
                new { controller = "Aereal", action = "searchA", q = UrlParameter.Optional, limit = UrlParameter.Optional, timestamp = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "search2A", // Route name
                "Aereal/search2A/{q}/{limit}/{timestamp}", // URL with parameters
                new { controller = "Aereal", action = "search2A", q = UrlParameter.Optional, limit = UrlParameter.Optional, timestamp = UrlParameter.Optional } // Parameter defaults
            );
            #endregion

            #region Lodging
            routes.MapRoute(
                "Lodging", // Route name
                "Service/Lodging/{lodgingModel}", // URL with parameters
                new { controller = "National", action = "NationalSearchResults", lodgingModel = UrlParameter.Optional } // Parameter defaults
            );
            #endregion

            #region Payment
            routes.MapRoute(
                "Confirmation", // Route name
                "Payment/Confirmation/{vacancyModel}", // URL with parameters
                new { controller = "Payment", action = "Confirmation", vacancyModel = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "PaymentMethods", // Route name
                "Payment/PaymentMethods/{reservationModel}", // URL with parameters
                new { controller = "Payment", action = "PaymentMethods", reservationModel = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "ProcessPaymentNPS", // Route name
                "Payment/ProcessPaymentNPS/{reservationModel}", // URL with parameters
                new { controller = "Payment", action = "ProcessPaymentNPS", reservationModel = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "ProcessResultPaymentNPS", // Route name
                "Payment/ProcessResultPaymentNPS/{model}", // URL with parameters
                new { controller = "Payment", action = "ProcessResultPaymentNPS", model = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "PaymentSuccess", // Route name
                "Payment/PaymentSuccess", // URL with parameters
                new { controller = "Payment", action = "PaymentSuccess" } // Parameter defaults
            );

            routes.MapRoute(
                "PaymentError", // Route name
                "Payment/PaymentError", // URL with parameters
                new { controller = "Payment", action = "PaymentError" } // Parameter defaults
            );

            routes.MapRoute(
                "InternalPaymentSimulator", // Route name
                "Payment/InternalPaymentSimulator", // URL with parameters
                new { controller = "Payment", action = "InternalPaymentSimulator" } // Parameter defaults
            );

            routes.MapRoute(
                "ProcessPaymentSimulation", // Route name
                "Payment/ProcessPaymentSimulation/{processAction}/{clave}", // URL with parameters
                new { controller = "Payment", action = "ProcessPaymentSimulation", processAction = UrlParameter.Optional, clave = UrlParameter.Optional } // Parameter defaults
            );
            #endregion

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Home", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}