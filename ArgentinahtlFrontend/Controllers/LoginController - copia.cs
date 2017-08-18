using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CheckArgentina.Models;
using CheckArgentina.Managers;
using System.Text;
using CheckArgentina.Commons;
using System.IO;
using CheckArgentina.NationalService;
using System.Net.Mail;
using System.Net;


namespace CheckArgentina.Controllers
{
    public class LoginController : Controller
    {
        private LocalManager Manager { get { return new LocalManager(); } }

        public ActionResult LoginAttemp(LoginModel loginModel)
        {
            var petitionResults = Manager.LoginAttemp(loginModel);

            if (petitionResults.Estado == (int)EstadoRespuesta.Ok)
            {
                loginModel.Apellido = petitionResults.Apellido;
                loginModel.Nombre = petitionResults.Nombre;
                loginModel.UID = petitionResults.UID;
            }
            else
            {
                loginModel.UID = null;
            }
                
              

            return RedirectToAction("NationalSearch", "National", new { userKey = loginModel.UID, userName = loginModel.Nombre });

        }

        public ActionResult Registration(RegistrationModel model)
        {
            using (var smtp = ObtenerClienteSmtp())
            {
                FluentEmail.Email
                .From(Config.LeerSetting("MailAccountFrom"))
                .Subject(string.Format("Alta de usuario {0}", model.BuissnessName))
                .To("mjjara@argentinahtl.com")
                .Body("Datos del Alta: <br>" +
                    "Nombre de la Empresa: " + model.BuissnessName + "<br>" +
                    "Razon Social: " + model.RazonSocial + "<br>" +
                    "Tipo de factura: " + model.BillType + "<br>" +
                    "CUIT: " + model.Cuit + "<br>" +
                    "Direccion: " + model.Adress + "<br>" +
                    "Codigo Postal: " + model.ZipCode + "<br>" +
                    "Telefono: " + model.Phone + "<br>" +
                    "Mail de confirmacion de reserva: " + model.ReservationConfirmationEmail + "<br>" +
                    "Persona a cargo: " + model.PersonInChargeName + "<br>" +
                    "Email de la persona a cargo: " + model.PersonInChargeEmail + "<br>" +
                    "Newsletter: " + model.Newsletter + "<br>" +
                    "Como llego a nosotros: " + model.HowDidYouFindUs)
                .UsingClient(smtp)
                .Send();
            }
            return View("RegistrationEmailSent");
        }
        private static SmtpClient ObtenerClienteSmtp()
        {
            var smtp = new SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = Config.LeerSetting("MailUseSSL", false);
            smtp.Host = Config.LeerSetting("MailServer");
            smtp.Port = Config.LeerSetting("MailPort", 25);

            string user = Config.LeerSetting("MailUser");

            if (!String.IsNullOrEmpty(user))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Config.LeerSetting("MailUser"),
                        Config.LeerSetting("MailPassword"));
            }
            return smtp;
        }
    }
}