using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ArgentinahtlMVC.Models.Services
{
    public static class Extensions
    {
        public static string GetCorrectName(this TimeZoneInfo timeZone)
        {
            var offset = timeZone.GetUtcOffset(DateTime.UtcNow);
            string name = string.Format("(UTC{0}{1}) ", (offset.TotalMinutes < 0 ? "-" : "+"), offset.ToString(@"hh\:mm"));

            // Add the time zone to list according the daylight time.
            if (timeZone.IsDaylightSavingTime(DateTime.UtcNow))
                name += timeZone.DaylightName;
            else
                name = timeZone.DisplayName.Replace("GMT", "UTC");

            return name;
        }

        public static T Coalesce<T>(params T[] elements)
        { 
            if(elements != null && elements.Count() != 0)
            {
                T returnElement = elements[0];

                foreach (var element in elements)
                { 
                    returnElement = element;

                    if (returnElement != null)
                        return returnElement;
                }
            }

            return default(T);
        }

        public static T CoalesceAndDiscardFirst<T>(params T[] elements)
        {
            if (elements != null && elements.Count() > 1)
            {
                T discardElement = elements[0];
                T returnElement = elements[1];

                foreach (var element in elements)
                {
                    returnElement = element;

                    if (returnElement != null && !returnElement.Equals(discardElement))
                        return returnElement;
                }
            }

            return default(T);
        }

        public static List<T> AddAndReturn<T>(this List<T> list, T item)
        { 
            list.Add(item);
            return list;
        }

        public static List<T> InsertAndReturn<T>(this List<T> list, int index, T item)
        {
            list.Insert(index, item);
            return list;
        }

        public static Dictionary<T, V> AddAndReturn<T, V>(this Dictionary<T, V> dictionary, T key, V value)
        {
            dictionary.Add(key, value);
            return dictionary;
        }

        public static int? ToInt(this object o)
        {
            int? value = null;

            try
            {
                value = Convert.ToInt32(o);
            }
            catch (Exception) { }

            return value;
        }

        public static long? ToLong(this object o)
        {
            long? value = null;

            try
            {
                value = Convert.ToInt64(o);
            }
            catch (Exception) { }

            return value;
        }

        public static DateTime? ToDateTime(this object o)
        {
            DateTime? value = null;

            try
            {
                value = Convert.ToDateTime(o);
            }
            catch (Exception) { }

            return value;
        }

        public static string ToNullableString(this object o)
        {
            string value = null;

            try
            {
                value = Convert.ToString(o);
            }
            catch (Exception) { }

            return value;
        }

        public static int NonStrictComparison(this string s, string s2, bool deleteSpaces = false)
        {
            return s.PrepareNonStrictCompararison(deleteSpaces).CompareTo(s2.PrepareNonStrictCompararison(deleteSpaces));
        }

        public static string PrepareNonStrictCompararison(this string s, bool deleteSpaces = false)
        {
            Regex replace_a_Accents = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
            Regex replace_e_Accents = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
            Regex replace_i_Accents = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
            Regex replace_o_Accents = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
            Regex replace_u_Accents = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
            s = replace_a_Accents.Replace(s, "a");
            s = replace_e_Accents.Replace(s, "e");
            s = replace_i_Accents.Replace(s, "i");
            s = replace_o_Accents.Replace(s, "o");
            s = replace_u_Accents.Replace(s, "u");
            s = s.ToUpper();

            if (deleteSpaces)
                s = s.Replace(" ", "");

            return s;
        }

        public static Dictionary<string, string> AddAndReturn(this Dictionary<string, string> dictionary, string key, string value)
        {
            dictionary.Add(key, value);

            return dictionary;
        }

        public static MvcHtmlString AddAttributes(this MvcHtmlString element, Dictionary<string, string> attributes)
        {
            var encodedString = element.ToHtmlString();
            var encodedAttributes = new Dictionary<string, string>();

            foreach (var attribute in attributes.Keys)
            {
                if (!(string.IsNullOrEmpty(attribute.Trim()) || string.IsNullOrEmpty(attributes[attribute].Trim())))
                {
                    var regEx = new Regex(attribute.Trim() + "[ \t]*=");

                    if (!regEx.Match(encodedString).Success)
                    {
                        int index = (encodedString.IndexOf("/>") >= 0 ? encodedString.IndexOf("/>") : encodedString.IndexOf(">"));
                        if (index >= 0)
                            encodedString = encodedString.Insert(index, string.Format(" {0}=\"{1}\"", attribute, attributes[attribute].Trim()));
                    }
                    else
                    {
                        int index = (encodedString.IndexOf("\"", regEx.Match(encodedString).Index));
                        if (index >= 0)
                            encodedString = encodedString.Insert(index + 1, string.Format("{0} ", attributes[attribute].Trim()));
                    }
                }
            }

            return MvcHtmlString.Create(encodedString);
        }

        public static MvcHtmlString AddAttribute(this MvcHtmlString element, string attribute, string value)
        {
            return !(string.IsNullOrEmpty(attribute.Trim()) || string.IsNullOrEmpty(value.Trim())) ?
                AddAttributes(element, new Dictionary<string, string>().AddAndReturn(attribute, value)) : element;
        }

        public static MvcHtmlString AddId(this MvcHtmlString element, string id)
        {
            return AddAttribute(element, "id", id);
        }

        public static MvcHtmlString AddClasses(this MvcHtmlString element, string classes)
        {
            return AddAttribute(element, "class", classes);
        }

        public static MvcHtmlString AddInlineStyles(this MvcHtmlString element, string styles)
        {
            return AddAttribute(element, "style", styles);
        }
    }
}