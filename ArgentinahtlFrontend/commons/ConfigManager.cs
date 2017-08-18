using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace CheckArgentina.Commons
{
    public class ConfigManager
    {
        #region Settings
        public static string ReadSetting(string name)
        {
            return ReadSetting<string>(name, null);
        }

        public static T ReadSetting<T>(string name)
        {
            return ReadSetting<T>(name, default(T));
        }

        public static T ReadSetting<T>(string name, T defaultValue)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (String.IsNullOrEmpty(value)) return defaultValue;
            try
            {
                Type ttype = typeof(T);

                if (ttype.IsEnum)
                {
                    return (T)Enum.Parse(ttype, value, true);
                }
                else if (ttype.IsPrimitive || ttype == typeof(string))
                {
                    return (T)Convert.ChangeType(value, ttype);
                }
            }
            catch
            {
            }
            return defaultValue;
        }
        #endregion

        public static SmtpClient GetClientSmtp()
        {
            var smtp = new SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = ReadSetting("MailUseSSL", false);
            smtp.Host = ReadSetting("MailServer");
            smtp.Port = ReadSetting("MailPort", 25);

            string user = ReadSetting("MailUser");

            if (!String.IsNullOrEmpty(user))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(ReadSetting("MailUser"),
                        ReadSetting("MailPassword"));
            }
            return smtp;
        }
    }
}