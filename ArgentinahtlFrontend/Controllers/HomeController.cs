using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using CheckArgentina.Models;
using CheckArgentina.Managers;
using System.Web;
using System.Web.UI.HtmlControls;
using System.IO;
using CheckArgentina.Commons;
using System;

namespace CheckArgentina.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Manager/

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult ShowRegister()
        {
            return View("Registration");
        }

        public ActionResult ShowCompany()
        {
            return View("Company");
        }

        public ActionResult ShowServices()
        {
            return View("Services");
        }

        public ActionResult ShowFaq()
        {
            return View("FaQ");
        }

        public ActionResult ShowNewsletter()
        {
            return View("Newsletter");
        }

        public ActionResult SendNewsletter(string email)
        {
            using (var smtp = ObtenerClienteSmtp())
            {
                FluentEmail.Email
                .From(email)
                .Subject("Subscripcion a Newsletter")
                .To("mjjara@ineltur.com")
                .Body("Se ha solicitado una subscripcion al newsletter con el siguiente correo " + email)
                .UsingClient(smtp)
                .Send();
            }
            return View("Home");
        }

        private static SmtpClient ObtenerClienteSmtp()
        {
            var smtp = new SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = Config.LeerSetting("MailUseSSL", false);
            smtp.Host = Config.LeerSetting("MailServer");
            smtp.Port = Config.LeerSetting("MailPort", 25);

            string user = Config.LeerSetting("MailUser");

            if (!string.IsNullOrEmpty(user))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Config.LeerSetting("MailUser"),
                        Config.LeerSetting("MailPassword"));
            }
            return smtp;
        }

        public ActionResult SearchUsers(string username)
        {
            return Json(ServiceManager.GetUsers(username, Session["userkey"].ToString()).Users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchHotels(string hotelName, string userKey)
        {
            return Json(ServiceManager.GetHotels(hotelName, Session["userkey"].ToString()).Lodgings, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendDepositConfirmation(HttpPostedFileBase file, string codigoReserva, string nombreAlojamiento)
        {
            try
            {
                
                Attachment attachment = null;

                if (file != null && file.ContentLength > 0 || !Path.GetExtension(file.FileName).Equals(".pdf", System.StringComparison.OrdinalIgnoreCase))
                {
                    attachment = new Attachment(file.InputStream, file.FileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
                }
                using (var smtp = ObtenerClienteSmtp())
                {
                    if (attachment != null)
                    {
                        FluentEmail.Email
                        .From("reservas@argentinahtl.com")
                        .Subject("Comprobante de Deposito")
                        .To("juanwillink@gmail.com")
                        .Body("El usuario: " + Session["username"].ToString() + " ha enviado como adjunto el comprobante de deposito de la reserva con codigo " + codigoReserva + " en " + nombreAlojamiento)
                        .Attach(attachment)
                        .UsingClient(smtp)
                        .Send();
                    }
                    else
                    {
                        FluentEmail.Email
                        .From("reservas@argentinahtl.com")
                        .Subject("Comprobante de Deposito")
                        .To("juanwillink@gmail.com")
                        .Body("El usuario: " + Session["username"].ToString() + " ha cometido un error al seleccionar el comprobante de deposito de la reserva con codigo " + codigoReserva + " en " + nombreAlojamiento)
                        .UsingClient(smtp)
                        .Send();
                    }
                    
                }
                return RedirectToAction("BuscarMisReservas", "Login");
            }
            catch (System.Exception ex)
            {
                return View("Error");
            }
        }
    }
}
