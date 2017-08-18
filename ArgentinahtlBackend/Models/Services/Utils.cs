using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Globalization;

namespace ArgentinahtlMVC.Models.Services
{
    public static class Utils
    {
        public static DateTime ExcelSerialDateToDateTime(double nFechaExcel)
        {
            if (nFechaExcel == 0) return new DateTime(1900, 1, 1);
            if (nFechaExcel == 60) return new DateTime(1900, 3, 1);
            if (nFechaExcel < 60) nFechaExcel++;
            nFechaExcel -= 2;
            DateTime oFechaOrigen = new DateTime(1900, 1, 1);
            return oFechaOrigen.AddDays(nFechaExcel);
        }

        public static string GenerarTablaHTML(List<RateModel> m, DateTime fechaDesde, string tipoTabla)
        {
            string html = string.Empty;
            //Definir estilo
            TableItemStyle tableStyle = new TableItemStyle();
            tableStyle.HorizontalAlign = HorizontalAlign.Center;
            tableStyle.VerticalAlign = VerticalAlign.Middle;
            tableStyle.Width = Unit.Pixel(30);
            tableStyle.Height = Unit.Pixel(30);
            tableStyle.BorderWidth = Unit.Pixel(1);
            tableStyle.BorderColor = Color.White;

            Table t = new Table();
            t.CssClass = "tablesorter";

            // Create more rows for the table.
            for (int r = 1; r <= 12; r++)
            {

                TableRow tempRow = new TableRow();

                //1er columna: Nombre del mes
                TableCell tempCell = new TableCell();
                tempCell.ID = r.ToString("D2");
                tempRow.Cells.Add(tempCell);
                tempCell.BackColor = Color.Gray;
                tempCell.ForeColor = Color.White;
                tempCell.Font.Bold = true;
                tempCell.Controls.Add(new Label() { Text = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(r) });

                for (int c = 1; c <= 31; c++)
                {
                    tempCell = new TableCell();
                    tempCell.ID = r.ToString("D2") + c.ToString("D2");
                    //tempCell.Text = String.Format("({0},{1})", r, c);
                    //tempCell.Controls.Add(new CheckBox() { ID = "chk" + r.ToString("D2") + c.ToString("D2") });
                    //tempCell.Controls.Add(new Label() { Text = r.ToString("D2") + c.ToString("D2") });
                    tempCell = FormatearSegunCupos(tempCell, m, r, c, fechaDesde.Year, tipoTabla);
                    tempRow.Cells.Add(tempCell);
                }
                t.Rows.Add(tempRow);
            }

            // Apply the TableItemStyle to all rows in the table.
            foreach (TableRow rw in t.Rows)
                foreach (TableCell cel in rw.Cells)
                    cel.ApplyStyle(tableStyle);

            AddHeader(t);

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                t.RenderControl(new System.Web.UI.HtmlTextWriter(sw));

                html = sw.ToString();
            }



            return html;
        }

        private static TableCell FormatearSegunCupos(TableCell tempCell, List<RateModel> m, int mes, int dia, int anio, string tipoTabla)
        {

            var rate = m.SingleOrDefault(t => t.Fecha.Month == tempCell.ID.Substring(0, 2).ToInt() &&
                                            t.Fecha.Day == tempCell.ID.Substring(2, 2).ToInt());
            if (rate != null)
            {
                tempCell.Controls.Add(new CheckBox() { ID = rate.Id.ToString() });
                tempCell.Controls.Add(new Label() { Text = "<br>D " + (rate.CupoMaximo - rate.CupoReservado).ToString() });
                tempCell.Controls.Add(new Label() { Text = "<br>R " + rate.CupoReservado.ToString() });

                if (rate.CupoMaximo > rate.CupoReservado)
                {
                    tempCell.BackColor = Color.LightGreen;
                }
                else
                {
                    tempCell.BackColor = Color.OrangeRed;
                }
            }
            else
            {
                tempCell.BackColor = Color.LightGray;

                DateTime d;
                if (DateTime.TryParse(mes.ToString("D2") + "/" + dia.ToString("D2") + "/" + anio.ToString(), out d) && tipoTabla.Contains("Cupos"))//validar que fecha exista
                    tempCell.Controls.Add(new CheckBox() { ID = mes.ToString("D2") + dia.ToString("D2") });
            }

            return tempCell;
        }

        private static Table AddHeader(Table t)
        {
            TableRow headerRow = new TableRow();
            TableHeaderCell tempCell = new TableHeaderCell();

            tempCell.BackColor = Color.Gray;
            tempCell.Controls.Add(new Label() { Text = "" });
            headerRow.Cells.Add(tempCell);
            headerRow.TableSection = TableRowSection.TableHeader;

            for (int c = 1; c <= 31; c++)
            {
                tempCell = new TableHeaderCell();
                tempCell.RowSpan = 1;
                tempCell.BackColor = Color.Gray;
                tempCell.ForeColor = Color.White;
                tempCell.Font.Bold = true;
                tempCell.HorizontalAlign = HorizontalAlign.Center;
                tempCell.VerticalAlign = VerticalAlign.Middle;

                tempCell.Text = String.Format("{0}", c);
                //tempCell.Controls.Add(new CheckBox() { ID = r.ToString("D2") + c.ToString("D2") });
                headerRow.Cells.Add(tempCell);
            }

            // Add the header row to the table.
            t.Rows.AddAt(0, headerRow);
            return t;
        }
    }
}