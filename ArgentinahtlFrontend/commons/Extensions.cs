using System.Collections.Generic;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System;

namespace CheckArgentina.Commons
{
    public static class Extensions
    {
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

        public static string CleanString(this string strIn)
        {
            return strIn.CleanString(@"[^\w\._-]");
        }

        public static string CleanString(this string strIn, string pattern)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, pattern, "", RegexOptions.None);
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (Exception)
            {
                return string.Empty;
            }

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
                        if(index >= 0)
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
            return ! (string.IsNullOrEmpty(attribute.Trim()) || string.IsNullOrEmpty(value.Trim())) ? 
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