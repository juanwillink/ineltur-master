using System;
using System.Web.Mvc;
using System.Web.Security;
using ArgentinahtlMVC.Models;
using ArgentinahtlMVC.Models.Services;

namespace ArgentinahtlMVC.Controllers
{
    public class UsersController : Controller
    {
        #region User management

        private static readonly object UserCreationLock = new object();

        public static MembershipCreateStatus CreateUser(string userName, string userDescription, string password,
            string email, UserProfile profile, int clientCode)
        {
            lock (UserCreationLock)
            {
                try
                {
                    if (DbAccess.CheckUsedUserName(userName)) return MembershipCreateStatus.DuplicateUserName;
                    if (DbAccess.CheckUsedEmail(email)) return MembershipCreateStatus.DuplicateEmail;
                    if (DbAccess.CreateUser(userName, userDescription, password, email, profile, clientCode)) return MembershipCreateStatus.Success;
                }
                catch
                {
                }
                return MembershipCreateStatus.ProviderError;
            }
        }

        public static bool ChangeUserStatus(string userName, bool enabled)
        {
            return DbAccess.ChangeUserStatus(userName, enabled, SessionData.UserProfile);
        }

        public static bool ResetUserPassword(string userName)
        {
            return DbAccess.ResetUserPassword(userName, SessionData.UserProfile);
        }

        public static void SignInUser(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public static void SignOutUser()
        {
            FormsAuthentication.SignOut();
        }

        public static int GetMinRequiredPasswordLength()
        {
            return Membership.Provider.MinRequiredPasswordLength;
        }

        #endregion

        // **************************************
        // URL: /Users/LogIn
        // **************************************

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LogInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = DbAccess.GetUser(model.UserName, model.Password);

                if (user != null)
                {
                    SignInUser(model.UserName, model.RememberMe);
                    SessionData.UserProfile = user.Profile;
                    SessionData.UserPermissions = DbAccess.GetUserPermission(user.UserName);
                    SessionData.UserId = user.UserId.GetValueOrDefault();

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Menu", "Management");
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "The user name or password provided is incorrect.");
                }
            }

            return View(model);
        }

        // **************************************
        // URL: /Users/LogOut
        // **************************************

        public ActionResult LogOut()
        {
            SignOutUser();
            SessionData.Clear();
            return RedirectToAction("Login", "Users");
        }

        // **************************************
        // URL: /Users/ChangePassword
        // **************************************

        [UserProfile]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = GetMinRequiredPasswordLength();
            return View();
        }

        [HttpPost]
        [UserProfile]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.ChangeUserPassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "The current password is incorrect or the new password is invalid.");
                }
            }

            ViewBag.PasswordLength = GetMinRequiredPasswordLength();
            return View(model);
        }

        // **************************************
        // URL: /Users/ChangePasswordSuccess
        // **************************************

        [UserProfile]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}