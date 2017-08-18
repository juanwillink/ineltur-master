using System;
using System.Data;
using System.IO;

namespace Ineltur.CuentasCorrientes.Modelos.Servicios
{
    public static class MsExcel
    {
        private const string StartExcel = "<?xml version=\"1.0\"?>\r\n" +
            "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
            " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n" +
            " xmlns:x=\"urn:schemas-microsoft-com:office:excel\"\r\n" +
            " xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">\r\n" +
            "<Styles>\r\n" +
            "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n" +
            "<Alignment ss:Vertical=\"Bottom\"/>\r\n" +
            "<Borders/>\r\n" +
            "<Font/>\r\n" +
            "<Interior/>\r\n" +
            "<NumberFormat/>\r\n" +
            "<Protection/>\r\n" +
            "</Style>\r\n" +
            "<Style ss:ID=\"BoldColumn\">\r\n" +
            "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n" +
            "<ss:Interior ss:Color=\"#E6EEEE\" ss:Pattern=\"Solid\"/>\r\n" +
            "</Style>\r\n" +
            "<Style ss:ID=\"StringLiteral\">\r\n" +
            "<NumberFormat ss:Format=\"@\"/>\r\n" +
            "</Style>\r\n" +
            "<Style ss:ID=\"Decimal\">\r\n" +
            "<NumberFormat ss:Format=\"#,##0.00\"/>\r\n" +
            "</Style>\r\n" +
            "<Style ss:ID=\"Integer\">\r\n" +
            "<NumberFormat ss:Format=\"0\"/>\r\n" +
            "</Style>\r\n" +
            "<Style ss:ID=\"DateLiteral\">\r\n" +
            "<NumberFormat ss:Format=\"dd/mm/yyyy hh:mm:ss;@\"/>\r\n" +
            "</Style>\r\n" +
            "</Styles>";
        private const string EndExcel = "\r\n</Workbook>";
        private const string StartWorksheet = "\r\n<Worksheet ss:Name=\"Sheet{0}\">";
        private const string EndWorksheet = "\r\n</Worksheet>";
        private const string StartTable = "\r\n<Table>";
        private const string EndTable = "\r\n</Table>";
        private const string StartRow = "\r\n<Row>";
        private const string EndRow = "</Row>";
        private const string Column = "\r\n<Column ss:AutoFitWidth=\"1\" ss:Width=\"{0}\"/>";

        private const string ColumnHeader = "<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">{0}</Data></Cell>";
        private const string StringCell = "<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">{0}</Data></Cell>";
        private const string DateTimeCell = "<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">{0}</Data></Cell>";
        private const string IntegerCell = "<Cell ss:StyleID=\"Integer\"><Data ss:Type=\"Number\">{0}</Data></Cell>";
        private const string DecimalCell = "<Cell ss:StyleID=\"Decimal\"><Data ss:Type=\"Number\">{0}</Data></Cell>";

        public static byte[] ExportToExcel(DataTable source)
        {
            return ExportToExcel(source, null);
        }

        public static byte[] ExportToExcel(DataTable source, int[] columnWidths)
        {
            var ms = new MemoryStream();

            using (var excelDoc = new StreamWriter(ms))
            {
                int rowCount = 0;
                int colCount = source.Columns.Count;
                int sheetCount = 0;

                if (columnWidths != null && columnWidths.Length != colCount) columnWidths = null;
                if (columnWidths == null)
                {
                    columnWidths = new int[colCount];
                    for (int i = 0; i < colCount; ++i)
                    {
                        columnWidths[i] = Math.Max(100, source.Columns[i].ColumnName.Length * 8);
                    }
                }
                excelDoc.Write(StartExcel);
                foreach (DataRow x in source.Rows)
                {
                    // New worksheet for large row counts or first row to be written

                    if (sheetCount == 0 || ++rowCount == 64000)
                    {
                        rowCount = 0;

                        if (sheetCount > 0)
                        {
                            excelDoc.Write(EndTable);
                            excelDoc.Write(EndWorksheet);
                        }
                        excelDoc.Write(StartWorksheet, ++sheetCount);
                        excelDoc.Write(StartTable);

                        // Write column headers

                        for (int i = 0; i < colCount; ++i)
                        {
                            excelDoc.Write(Column, columnWidths[i]);
                        }

                        // Write header row

                        excelDoc.Write(StartRow);
                        foreach (DataColumn c in source.Columns)
                        {
                            excelDoc.Write(ColumnHeader, c.ColumnName);
                        }
                        excelDoc.Write(EndRow);
                    }

                    excelDoc.Write(StartRow);
                    for (int y = 0; y < colCount; y++)
                    {
                        var obj = x[y];
                        var objType = obj.GetType();

                        if (objType.IsEnum) obj = obj.ToString();
                        switch (Type.GetTypeCode(objType))
                        {
                            case TypeCode.String:
                                string str = obj.ToString();

                                str = str.Trim().Replace("&", "&amp;").Replace(">", "&gt;");
                                str = str.Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;");

                                excelDoc.Write(StringCell, str);
                                break;

                            case TypeCode.DateTime:
                                string dateTime = ((DateTime)obj).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff");

                                excelDoc.Write(DateTimeCell, dateTime);
                                break;

                            case TypeCode.Boolean:
                                excelDoc.Write(StringCell, obj.ToString());
                                break;

                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Byte:
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                                excelDoc.Write(IntegerCell, obj.ToString());
                                break;

                            case TypeCode.Decimal:
                            case TypeCode.Single:
                            case TypeCode.Double:
                                excelDoc.Write(DecimalCell, obj.ToString());
                                break;

                            case TypeCode.DBNull:
                                excelDoc.Write(StringCell, String.Empty);
                                break;

                            default:
                                throw new Exception(objType.ToString() + " not handled.");
                        }
                    }
                    excelDoc.Write(EndRow);
                }
                excelDoc.Write(EndTable);
                excelDoc.Write(EndWorksheet);
                excelDoc.Write(EndExcel);
                excelDoc.Flush();
            }

            return ms.GetBuffer();
        }
    }
}