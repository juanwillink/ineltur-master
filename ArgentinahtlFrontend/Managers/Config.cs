using System;
using System.Configuration;

namespace CheckArgentina.Managers
{
    static class Config
    {
public static string LeerSetting(string nombre)
        {
            return LeerSetting<string>(nombre, null);
        }

        public static T LeerSetting<T>(string nombre)
        {
            return LeerSetting<T>(nombre, default(T));
        }

        public static T LeerSetting<T>(string nombre, T valorPorDefecto)
        {
            string value = ConfigurationManager.AppSettings[nombre];

            if (String.IsNullOrEmpty(value)) return valorPorDefecto;
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
            return valorPorDefecto;
        }
    }
}