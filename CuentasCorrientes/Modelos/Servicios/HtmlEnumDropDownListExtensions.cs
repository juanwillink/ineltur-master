using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Ineltur.CuentasCorrientes.Modelos.Servicios
{
    public static class HtmlEnumDropDownListExtensions
    {
        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = String.Empty, Value = String.Empty } };

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            var realModelType = modelMetadata.ModelType;
            var underlyingType = Nullable.GetUnderlyingType(realModelType);

            if (underlyingType != null) realModelType = underlyingType;
            return realModelType;
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var enumType = GetNonNullableModelType(metadata);
            var converter = TypeDescriptor.GetConverter(enumType);

            var items = Enum.GetValues(enumType).Cast<TEnum>().Select(value => new SelectListItem()
            {
                Text = converter.ConvertToString(value), 
                Value = value.ToString(),
                Selected = value.Equals(metadata.Model)
            });

            if (metadata.IsNullableValueType)
            {
                items = SingleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(expression, items);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<TProperty> values)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var propType = GetNonNullableModelType(metadata);
            var converter = TypeDescriptor.GetConverter(propType);

            var items = values.Where(v => v != null).Select(value => new SelectListItem()
            {
                Text = converter.ConvertToString(value),
                Value = value.ToString(),
                Selected = value.Equals(metadata.Model)
            });

            if (metadata.IsNullableValueType || metadata.ModelType.IsClass)
            {
                items = SingleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(expression, items);
        }
    }
}