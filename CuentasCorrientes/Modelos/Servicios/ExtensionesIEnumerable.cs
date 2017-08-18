using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Ineltur.CuentasCorrientes.Modelos.Servicios
{
    public static class ExtensionesIEnumerable
    {
        public static DataTable DataTableDeEnumerable<T>(this IEnumerable<T> e)
        {
            var tabla = new DataTable();
            PropertyInfo[] props = null;

            foreach (var item in e)
            {
                if (props == null)
                {
                    props = ((Type)item.GetType()).GetProperties();
                    foreach (PropertyInfo pi in props)
                    {
                        var colType = pi.PropertyType;

                        if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        tabla.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = tabla.NewRow();

                foreach (var pi in props)
                {
                    var valor = pi.GetValue(item, null);

                    dr[pi.Name] = valor == null ? DBNull.Value : valor;
                }

                tabla.Rows.Add(dr);
            }

            return tabla;
        }
    }
}