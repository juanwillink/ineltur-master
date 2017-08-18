using System.Web.Mvc;
using System.Web.Routing;

namespace ArgentinahtlMVC.Models.Services
{
    public class UserProfileAttribute : ActionFilterAttribute
    {
        private static readonly RouteValueDictionary LoginResultRoute;
        private static readonly RouteValueDictionary MenuResultRoute;

        static UserProfileAttribute()
        {
            LoginResultRoute = new RouteValueDictionary()
            {
                { "controller", "Users" },
                { "action", "LogIn" }
            };
            MenuResultRoute = new RouteValueDictionary()
            {
                { "controller", "Management" },
                { "action", "Menu" }
            };
        }

        private UserProfile _profile;

        public UserProfileAttribute()
            : this(UserProfile.Operator)
        {
        }

        public UserProfileAttribute(UserProfile minimumProfile)
        {
            _profile = minimumProfile;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SessionData.UserProfile == UserProfile.None)
            {
                filterContext.Result = new RedirectToRouteResult(LoginResultRoute);
            }
            else if (SessionData.UserProfile < _profile)
            {
                filterContext.Result = new RedirectToRouteResult(MenuResultRoute);
            }
        }
    }
}