using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArgentinahtlMVC.Models.Services;
using ArgentinahtlMVC.Models;
using System.Web.Security;

namespace ArgentinahtlMVC.Controllers
{
    public class ManagementController : Controller
    {
        // **************************************
        // URL: /Management/Menu
        // **************************************

        [UserProfile]
        public ActionResult Menu()
        {
            return View();
        }

        #region User management

        // **************************************
        // URL: /Management/Users
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult Users()
        {
            return View("Users", new UserListModel()
            {
                Users = DbAccess.GetUsers()
            });
        }

        // **************************************
        // URL: /Management/NewUser
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult NewUser()
        {
            ViewBag.PasswordLength = UsersController.GetMinRequiredPasswordLength();
            return View("User", new UserModel());
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult NewUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Profile == UserProfile.None || model.Profile > SessionData.UserProfile)
                {
                    ModelState.AddModelError("Profile", "No puede crearse un usuario con ese perfil.");
                }
                else
                {
                    string password = model.UserName;
                    MembershipCreateStatus createStatus = UsersController.CreateUser(model.UserName, model.UserDescription,
                            password, model.Email, model.Profile, -1);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        ViewBag.Message = "Usuario Creado";
                        return Users();
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, AccountValidation.ErrorCodeToString(createStatus));
                    }
                }
            }

            ViewBag.PasswordLength = UsersController.GetMinRequiredPasswordLength();

            return View("User", model);
        }

        // **************************************
        // URL: /Management/EnableUser
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult EnableUser(string id)
        {
            if (UsersController.ChangeUserStatus(id, true))
            {
                ViewBag.Message = "Usuario habilitado";
            }
            else
            {
                ViewBag.Message = "El estado del usuario no pudo cambiarse";
            }
            return Users();
        }

        // **************************************
        // URL: /Management/DisableUser
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult DisableUser(string id)
        {
            if (UsersController.ChangeUserStatus(id, false))
            {
                ViewBag.Message = "Usuario deshabilitado";
            }
            else
            {
                ViewBag.Message = "El estado del usuario no pudo cambiarse";
            }
            return Users();
        }

        // **************************************
        // URL: /Management/DeleteUser
        // **************************************

        [UserProfile(UserProfile.Superadmin)]
        public ActionResult DeleteUser(string id)
        {
            if (DbAccess.DeleteUser(id))
            {
                ViewBag.Message = "Usuario eliminado";
            }
            else
            {
                ViewBag.Message = "El usuario no pudo eliminarse";
            }
            return Users();
        }

        // **************************************
        // URL: /Management/ResetUserPassword
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult ResetUserPassword(string id)
        {
            if (UsersController.ResetUserPassword(id))
            {
                ViewBag.Message = "Contraseña reseteada";
            }
            else
            {
                ViewBag.Message = "La contraseña no pudo resetearse";
            }
            return Users();
        }

        #endregion

        #region Platform management

        public ActionResult ClearCache()
        {
            CacheData.Clear();
            return PartialView();
        }

        public ActionResult Error()
        {
            return View();
        }

        #endregion
    }
}
