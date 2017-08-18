using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ArgentinahtlMVC.Models.Services
{
    public class PascalCaseWordSplittingEnumConverter : EnumConverter
    {
        private static readonly Regex Splitter = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.Compiled);

        public PascalCaseWordSplittingEnumConverter(Type type)
            : base(type)
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string)) throw new NotImplementedException();
            return base.ConvertFrom(context, culture, ((string)value).Replace(" ", String.Empty));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null && value.GetType().IsEnum)
            {
                return Splitter.Replace(value.ToString(), " ");
            }
            throw new NotImplementedException();
        }
    }
}