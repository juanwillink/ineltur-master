using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ArgentinahtlMVC.Models.Services;
using System;

namespace ArgentinahtlMVC.Models
{
    public static class SessionData
    {
        private static HttpSessionState Session
        {
            get { return Context.Session; }
        }

        private static HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        public static string UserName
        {
            get { return Context.User.Identity.Name; }
        }

        public static UserProfile UserProfile
        {
            get
            {
                var p = Session["UserProfile"];

                if (p == null) return default(UserProfile);
                return (UserProfile)p;
            }

            set
            {
                Session["UserProfile"] = value;
            }
        }

        public static UserPermissionModel UserPermissions
        {
            get
            {
                var userPermissions = Session["UserPermissions"] as UserPermissionModel;

                if (userPermissions == null)
                {
                    Session["UserPermissions"] = userPermissions = DbAccess.GetUserPermission(UserName);
                }
                return userPermissions;
            }
            set
            {
                Session["UserPermissions"] = value;
            }
        }

        public static Guid UserId
        {
            get
            {
                var u = Session["UserId"];

                if (u == null) return default(Guid);
                return (Guid)u;
            }

            set
            {
                Session["UserId"] = value;
            }
        }

        public static void Clear()
        {
            Session.Clear();
        }
    }
}