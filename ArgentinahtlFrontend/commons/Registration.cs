using System;
using System.Web.Services;

using System.Drawing.Imaging;
using System.IO;

namespace CheckArgentina.Commons
{
    public class Registration
    {
        public static string CreateCaptcha(Stream stream)
        {
            // Create a random code and store it in the Session object.
            var captchaImageText = GenerateRandomCode();
            // Create a CAPTCHA image using the text stored in the Session object.
            RandomImage ci = new RandomImage(captchaImageText, 300, 75);
            // Write the image to the response stream in JPEG format.
            ci.Image.Save(stream, ImageFormat.Jpeg);
            // Dispose of the CAPTCHA image object.
            ci.Dispose();

            return captchaImageText;
        }

        private static string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }

        [WebMethod()]
        public static bool RegisterEmail(string email)
        {
            

            using (var smtp = ConfigManager.GetClientSmtp())
            {
                FluentEmail.Email
                                .From(ConfigManager.ReadSetting("MailAccountFrom"))
                                .Subject(string.Format("Registrando Email"))
                                .To(email)//"reservas@argentinahtl.com")
                                .Body("Prueba de registro", false)
                                .UsingClient(smtp)
                                .Send();
            }

            return true;
        }
    }
}