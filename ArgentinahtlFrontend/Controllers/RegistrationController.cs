using System.Web.Mvc;
using System.IO;
using CheckArgentina.Commons;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using System;

namespace CheckArgentina.Controllers
{
    public class RegistrationController : Controller
    {
        //
        // GET: /Captcha/
        public ActionResult GetCaptcha()
        {
            var stream = new MemoryStream();
            
            var captchaImageText = Registration.CreateCaptcha(stream);
            
            var image = Image.FromStream(stream);
            Session["captcha_hash"] = GetHash(captchaImageText);

            //return File(stream, "image/jpeg");
            return new ImageResult { Image = Image.FromStream(stream), ImageFormat = ImageFormat.Jpeg }; 
        }

        public ActionResult RegisterEmail(string reg_email, string text_captcha)
        {
            if (IsCaptchaValid(text_captcha) && Registration.RegisterEmail(reg_email))
                return Json(new { Result = "Success"});
            else
                return Json(new { Result = "Fail" });
        }

        public bool IsCaptchaValid(string captcha)
        {
            return GetHash(captcha).Equals((Session["captcha_hash"] ?? string.Empty));
        }

        private string GetHash(string input)
        { 
            var bytes = ASCIIEncoding.ASCII.GetBytes(input);
            return BitConverter.ToString(MD5.Create().ComputeHash(bytes));
        }
    }
}
