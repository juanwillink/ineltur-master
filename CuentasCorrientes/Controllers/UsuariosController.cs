using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Ineltur.CuentasCorrientes.Modelos;

namespace Ineltur.CuentasCorrientes.Controllers
{
    public class UsuariosController : BaseController
    {
        // **************************************
        // URL: /Usuarios/LogIn
        // **************************************

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(ModeloLogin model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = DataContext.Usuarios.SingleOrDefault(u => u.NombreUsuario == model.NombreUsuario &&
                        u.Clave == model.Contrasenya && (u.IdTipoUsuario == PerfilesUsuario.PerfilAdministrador ||
                        u.IdTipoUsuario == PerfilesUsuario.PerfilAuditor ||
                        u.IdTipoUsuario == PerfilesUsuario.PerfilCliente) && u.Activo);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.NombreUsuario, model.Recordarme);
                    DatosSesion.IdUsuario = user.IdUsuario;

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Menu", "Administracion");
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Nombre de usuario o contraseña incorrectos.");
                }
            }

            return View(model);
        }

        // **************************************
        // URL: /Usuarios/Logout
        // **************************************

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Usuarios");
        }
    }
}