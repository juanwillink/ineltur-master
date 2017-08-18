using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Ineltur.WebService
{
    public static class Utiles
    {
        public static int DistanciaLevenshtein(string s, string t)
        {
            int costo = 0;
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= m; d[i, 0] = i++) ;
            for (int j = 0; j <= n; d[0, j] = j++) ;

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    costo = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + costo);
                }
            }

            return d[m, n];
        }

        public static string Base36(long n)
        {
            if (n <= 0) return "0";

            char[] base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            string returnValue = String.Empty;

            while (n != 0)
            {
                returnValue = base36Chars[n % base36Chars.Length] + returnValue;
                n /= 36;
            }
            return returnValue;
        }

        #region By Roque Lucero
        /// <summary>
        /// Evalúa la existencia de un elemento dentro de una lista de elementos determinada.
        /// </summary>
        /// <typeparam name="T">Función Generic</typeparam>
        /// <param name="elemento">El elemento a evaluar</param>
        /// <param name="listaElementos">Lista de elementos del mismo tipo que el elemento a evaluar</param>
        /// <returns>true si el elemento se encuentra en la lista, false en caso contrario</returns>
        public static bool PerteneceALista<T>(T elemento, params T[] listaElementos)
        {
            foreach (T t in listaElementos)
            {
                if (elemento.Equals(t))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Evalúa la existencia del elemento dentro de una lista de elementos determinada.
        /// </summary>
        /// <param name="listaElementos">Lista de elementos del mismo tipo que el elemento a evaluar</param>
        /// <returns>true si el elemento se encuentra en la lista, false en caso contrario</returns>
        public static bool PerteneceA<T>(this T elemento, params T[] listaElementos)
        {
            return PerteneceALista(elemento, listaElementos);
        }

        public static string ToStringWithProperties(this object element)
        {
            var toString = string.Empty;

            try
            {
                if (element.GetType().IsValueType || element.GetType().IsPrimitive || element.GetType() == typeof(string))
                    toString = element.ToString();
                else
                {
                    foreach (var p in element.GetType().GetProperties())
                    {
                        if (p.GetType().IsValueType || p.GetType().IsPrimitive || p.PropertyType == typeof(string))
                            toString += string.Format("{0}: {1}\r\n", p.Name, p.GetValue(element, null));
                        else
                        {
                            if (p.GetIndexParameters().Length == 0 && !typeof(System.Collections.IEnumerable).PerteneceA(p.PropertyType.GetInterfaces()))
                                if (p.GetValue(element, null) != null)
                                    toString += string.Format("{0}: {1}\r\n", p.Name, p.GetValue(element, null).ToStringWithProperties());
                                else
                                    toString += string.Format("{0}: {1}\r\n", p.Name, "NULL");
                            else
                            {
                                if (p.GetValue(element, null) != null)
                                {
                                    var i = 0;
                                    foreach (var o in p.GetValue(element, null) as IEnumerable)
                                    {
                                        toString += string.Format("{0}[{1}]: {2}\r\n", p.Name, i, o.ToStringWithProperties());
                                        i++;
                                    }
                                }
                                else
                                    toString += string.Format("{0}: {1}\r\n", p.Name, "NULL");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }

            return toString;
        }
        #endregion
    }

    public class CipherUtility
    {
        public static string Encrypt<T>(string value, string password, string salt)
              where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public static string Decrypt<T>(string text, string password, string salt)
           where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

            using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }

    public class DateGreaterThanAttribute : RangeAttribute
    {
        public DateGreaterThanAttribute(string MinimumDate)
            : base(typeof(DateTime), MinimumDate, DateTime.Now.AddDays(-1).ToShortDateString()) { }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class VisibleFromWSDL : System.Attribute
    {
        private bool _visible;

        public bool Visible { get { return _visible; } }

        public VisibleFromWSDL(bool visible)
        {
            _visible = visible;
        }
    }
}