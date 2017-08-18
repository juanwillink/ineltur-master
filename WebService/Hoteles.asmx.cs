using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Services;
using Ineltur.Datos;
using Ineltur.Datos.Entidades;
using System.Web;
using System.IO;
using System.Configuration;

namespace Ineltur.WebService
{
    [WebService(Name = "HotelesSoap", Namespace = "http://www.ineltur.com/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1, Name = "BindingHotelesSoap", Location = "Hoteles.wsdl", Namespace = "http://www.ineltur.com/")]
    [System.ComponentModel.ToolboxItem(false)]
    public partial class Hoteles : System.Web.Services.WebService
    {
        private static object locker = new object();
        private static long _operationNumber = 0;

        public static long GetCurrentOperationNumber()
        {
            lock (locker)
            {
                return _operationNumber;
            }
        }

        public static long IncreaseOperationNumber()
        {
            lock (locker)
            {
                _operationNumber++;
                return _operationNumber;
            }
        }

        private static readonly Dictionary<Datos.TipoAlojamiento, Guid> TiposAlojamiento;
        private static readonly Dictionary<Guid, Datos.TipoAlojamiento> TiposAlojamiento2;
        private static readonly Dictionary<Guid, Datos.CategoriaAlojamiento> CategoriasAlojamiento;
        private static readonly Dictionary<Guid, Datos.Moneda> Monedas;

        private static readonly Guid FormaPago = Guid.Parse(Config.LeerSetting("FormaPago"));
        private static readonly Guid TipoUsuario = Guid.Parse(Config.LeerSetting("TipoUsuario"));
        private static readonly Guid Pasaporte = Guid.Parse(Config.LeerSetting("Pasaporte"));
        private static readonly Guid WebServiceSitio = Guid.Parse(Config.LeerSetting("WebServiceSitio"));
        private static readonly Guid WebServiceUsuario = Guid.Parse(Config.LeerSetting("WebServiceUsuario"));
        private static readonly Guid DNI = Guid.Parse(Config.LeerSetting("DNI"));
        private static readonly Guid CUIT = Guid.Parse(Config.LeerSetting("CUIT"));
        private static readonly long TicksOffset = new DateTime(2000, 1, 1).Ticks;
        private static readonly TimeSpan UnDia = TimeSpan.FromDays(1.0);

        static Hoteles()
        {
            using (var dc = NuevoDataContext())
            {
                string[] valoresEnum = Enum.GetNames(typeof(Datos.TipoAlojamiento));

                TiposAlojamiento = new Dictionary<Datos.TipoAlojamiento, Guid>();
                CategoriasAlojamiento = new Dictionary<Guid, Datos.CategoriaAlojamiento>();
                Monedas = new Dictionary<Guid, Datos.Moneda>();

                #region TiposAlojamiento

                foreach (var tipo in dc.TiposAlojamiento)
                {
                    var tipoAloj = Datos.TipoAlojamiento.SinTipoAlojamiento;

                    var tipoNombre = tipo.Nombre.Trim().ToLower();

                    tipoNombre = tipoNombre.Replace("á", "a");
                    tipoNombre = tipoNombre.Replace("é", "e");
                    tipoNombre = tipoNombre.Replace("í", "i");
                    tipoNombre = tipoNombre.Replace("ó", "o");
                    tipoNombre = tipoNombre.Replace("ú", "u");

                    if(tipoNombre.StartsWith("apart"))
                        tipoAloj = Datos.TipoAlojamiento.ApartHotel;
                    else if (tipoNombre.EndsWith("boutique"))
                        tipoAloj = Datos.TipoAlojamiento.HotelBoutique;
                    else if (tipoNombre.EndsWith("spa"))
                        tipoAloj = Datos.TipoAlojamiento.HotelSpa;
                    else if(tipoNombre.StartsWith("complejo"))
                        tipoAloj = Datos.TipoAlojamiento.ComplejoTuristico;
                    else if(tipoNombre.StartsWith("estancia"))
                        tipoAloj = Datos.TipoAlojamiento.EstanciaRanches;
                    else
                    {
                        switch(tipoNombre)
                        {
                            case "b&b": tipoAloj = Datos.TipoAlojamiento.BandB; break;
                            case "bodega": tipoAloj = Datos.TipoAlojamiento.Bodega; break;
                            case "bungalows": tipoAloj = Datos.TipoAlojamiento.Bungalows; break;
                            case "cabañas": tipoAloj = Datos.TipoAlojamiento.Cabanyas; break;
                            case "hospedaje": tipoAloj = Datos.TipoAlojamiento.Hospedaje; break;
                            case "hostal": tipoAloj = Datos.TipoAlojamiento.Hostal; break;
                            case "hosteria": tipoAloj = Datos.TipoAlojamiento.Hosteria; break;
                            case "hotel": tipoAloj = Datos.TipoAlojamiento.Hotel; break;
                            case "lodge": tipoAloj = Datos.TipoAlojamiento.Lodge; break;
                            case "motel": tipoAloj = Datos.TipoAlojamiento.Motel; break;
                            case "parador": tipoAloj = Datos.TipoAlojamiento.Parador; break;
                            case "posada": tipoAloj = Datos.TipoAlojamiento.Posada; break;
                            case "resort": tipoAloj = Datos.TipoAlojamiento.Resort; break;
                        }
                    }

                    if(!TiposAlojamiento.ContainsKey(tipoAloj))
                        TiposAlojamiento.Add(tipoAloj, tipo.IdTipoAlojamiento);
                }

                // Lo siguiente no funciona porque los nombres no pueden ser matchados correctamente
                //foreach (var tipo in dc.TiposAlojamiento)
                //{
                //    try
                //    {
                //        TiposAlojamiento.Add((Datos.TipoAlojamiento)Enum.Parse(typeof(Datos.TipoAlojamiento), tipo.Nombre.Trim(), true), tipo.IdTipoAlojamiento);
                //    }
                //    catch (Exception) { }
                //}
                #endregion

                TiposAlojamiento2 = TiposAlojamiento.ToDictionary(p => p.Value, p => p.Key);

                foreach (var categoriaAlojamiento in dc.TiposEstrellasAlojamiento)
                {
                    Datos.CategoriaAlojamiento cat = Datos.CategoriaAlojamiento.SinCategoria;

                    if (categoriaAlojamiento.Nombre.Trim().StartsWith("Otra"))
                    {
                        cat = Datos.CategoriaAlojamiento.OtraCategoria;
                    }
                    else
                    {
                        int estrellas;

                        if (Int32.TryParse(Regex.Match(categoriaAlojamiento.Nombre, @"\d+").Value,
                                out estrellas))
                        {
                            cat = (Datos.CategoriaAlojamiento)estrellas;
                        }
                    }

                    if (!CategoriasAlojamiento.ContainsKey(categoriaAlojamiento.IdTipoEstrellasAlojamiento))
                        CategoriasAlojamiento.Add(categoriaAlojamiento.IdTipoEstrellasAlojamiento, cat);
                }

                foreach (var moneda in dc.Monedas)
                {
                    try
                    {
                        Datos.Moneda m = (Datos.Moneda)Enum.Parse(typeof(Datos.Moneda), moneda.Descripcion);

                        Monedas.Add(moneda.IdMoneda, m);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static WebServiceDataContext NuevoDataContext()
        {
            var dc = new WebServiceDataContext(ConfigurationManager.ConnectionStrings["TurismoConnectionString"].ConnectionString);
            var dl = new DataLoadOptions();

            dl.LoadWith<Ciudad>(c => c.Provincia);

            dc.LoadOptions = dl;
            return dc;
        }

        private static float ObtenerCotizacionTarifaAlojamiento(WebServiceDataContext dc, Guid? idAloj, string idNacionalidad)
        {
            var dc2 = dc;

            try
            {

                if (dc2 == null) dc2 = NuevoDataContext();

                var idMoneda = dc2.Tarifas_Alojamientos.Where(t => t.IDALOJ == idAloj && t.IDNACIONALIDAD == idNacionalidad && t.FECHA_DESDE <= DateTime.Now && t.FECHA_HASTA == null).Select(t => t.IDMONEDA).SingleOrDefault();

                float cotizAlojamiento = dc2.Monedas.Where(m => m.IdMoneda == idMoneda).Select(m => m.Cotizacion).SingleOrDefault();

                return cotizAlojamiento;
            }
            finally
            {
                if (dc == null) dc2.Dispose();
            }

        }

        #region Conversi n de datos

        private static string ConvertirOrdenamiento(OrdenAlojamientos? orden)
        {
            switch (orden.GetValueOrDefault(ParametrosBasicos.OrdenamientoAlojamientosPorDefecto))
            {
                case OrdenAlojamientos.PorPrecio: return "precio";
                case OrdenAlojamientos.PorCategoria: return "estrellas";
                case OrdenAlojamientos.PorNombre: return "alfabetico";
            }
            return String.Empty;
        }

        private static Guid? ConvertirTipoAlojamiento(Datos.TipoAlojamiento? tipo)
        {
            if (!tipo.HasValue) return null;

            Guid ta;

            return TiposAlojamiento.TryGetValue(tipo.Value, out ta) ? ta : (Guid?)null;
        }

        private static Datos.TipoAlojamiento ConvertirTipoAlojamiento(Guid? tipo)
        {
            if (!tipo.HasValue) return Datos.TipoAlojamiento.SinTipoAlojamiento;

            Datos.TipoAlojamiento ta;

            return TiposAlojamiento2.TryGetValue(tipo.Value, out ta) ? ta : 0;
        }

        private static decimal? ConvertirPrecio(float? num)
        {
            if (!num.HasValue) return null;
            return Decimal.Round((decimal)num.Value, 2);
        }

        private static CategoriaAlojamiento ConvertirCategoria(Guid categoria)
        {
            CategoriaAlojamiento cat;

            if (CategoriasAlojamiento.TryGetValue(categoria, out cat)) return cat;
            return CategoriaAlojamiento.SinCategoria;
        }

        private static Datos.Moneda ConvertirMoneda(Guid moneda)
        {
            Datos.Moneda mon;

            if (Monedas.TryGetValue(moneda, out mon)) return mon;
            return 0;
        }

        #endregion

        #region Operaciones comunes

        private Guid? ValidarUsuarioClave(WebServiceDataContext dc, PeticionBase peticion)
        {
            var dc2 = dc;

            try
            {
                if (dc2 == null) dc2 = NuevoDataContext();

                Guid idUsuario = dc2.Usuarios.Where(u => u.NombreUsuario == peticion.Usuario &&
                        u.Clave == peticion.Clave).Select(u => u.IdUsuario).SingleOrDefault();

                return idUsuario == Guid.Empty ? (Guid?)null : idUsuario;
            }
            finally
            {
                if (dc == null) dc2.Dispose();
            }
        }

        private static float ObtenerCotizacion(WebServiceDataContext dc, Guid idMoneda)
        {
            var dc2 = dc;

            try
            {

                if (dc2 == null) dc2 = NuevoDataContext();

                float cotizAlojamiento = dc2.Monedas.Where(m => m.IdMoneda == idMoneda).Select(m => m.Cotizacion).SingleOrDefault();

                return cotizAlojamiento;
            }
            finally
            {
                if (dc == null) dc2.Dispose();
            }
           
        }

        private static Guid ObtenerMoneda(WebServiceDataContext dc, Guid idAlojamiento)
        {
            var dc2 = dc;

            try
            {

                if (dc2 == null) dc2 = NuevoDataContext();

                Guid idMoneda = (Guid) dc2.Alojamientos.Where(a => a.IdAlojamiento == idAlojamiento).Select(a => a.IdMoneda).SingleOrDefault();

                return idMoneda;
            }
            finally
            {
                if (dc == null) dc2.Dispose();
            }

        }

        private static Guid ObtenerMonedaUsuario(WebServiceDataContext dc, Guid idUsuario)
        {
            var dc2 = dc;

            try
            {

                if (dc2 == null) dc2 = NuevoDataContext();

                Guid idMoneda = (Guid)dc2.Usuarios.Where(u => u.IdUsuario == idUsuario).Select(u => u.IdMoneda).SingleOrDefault();

                return idMoneda;
            }
            finally
            {
                if (dc == null) dc2.Dispose();
            }

        }

        private static string MapearIdioma(string idiomaDeseado)
        {
            switch (idiomaDeseado == null ? String.Empty : idiomaDeseado.ToLowerInvariant())
            {
                case "en": return "en";
                case "es": return "es";
                default: return "en";
            }
        }

        private static void CargarAmenidadesRegimenAlojamiento(Alojamiento alojamiento, InfoAlojamiento info)
        {
            using (var dc = NuevoDataContext())
            {
                var amenidades = alojamiento.ServicioAlojamientos.Where(sa => sa.Fijo).Join(dc.Servicios, sa => sa.IdServ, s => s.IdServ, (sa, s) => new { IdServ = s.IdServ, NombreServ = s.Nombre});
                info.Amenidades = new List<string>();
                info.Regimen = RegimenAlojamiento.NoInformado;

                foreach (var amenidad in amenidades)
                {
                    switch (amenidad.IdServ.ToString())
                    {
                        case "8e88292e-f6fe-40a2-821c-4d0fbd5ef025": // Desayuno
                            info.Regimen = RegimenAlojamiento.Desayuno;
                            break;

                        case "7f32a94a-ca84-43b4-943f-eff30340d8b7": // Media Pensión
                            info.Regimen = RegimenAlojamiento.MediaPension;
                            break;

                        case "fb0bfbc2-2baf-4c17-8dec-ae2c8030abd8": // Pensión Completa
                            info.Regimen = RegimenAlojamiento.PensionCompleta;
                            break;

                        default:
                            info.Amenidades.Add(amenidad.NombreServ); break;
                    }
                }
            }            
        }

        private static void CargarInfoAlojamiento(Alojamiento alojamiento, InfoAlojamiento info, Guid idMonedaUsuario)
        {
            info.IdAlojamiento = alojamiento.IdAlojamiento;
            info.Nombre = alojamiento.Nombre;
            info.Descripcion = alojamiento.Descripcion;
            info.Descripcion2 = alojamiento.Descripcion2;
            info.Direccion = alojamiento.Direccion;
            info.Tipo = ConvertirTipoAlojamiento(alojamiento.IdTipoAlojamiento);
            info.Categoria = ConvertirCategoria(alojamiento.IdTipoEstrellaAlojamiento);
            info.PoliticasCancelacion = alojamiento.PoliticasCancelacion;
            //info.DiasCancelacionCargo = alojamiento.DiasCancelacionCargo;
            info.BajoPeticion = alojamiento.IdTipoPerfil == Guid.Parse("DDEAB0FD-6515-4788-9034-40C1888459C4");
            
            info.Telefono = alojamiento.Telefono;
            info.FotoAlojamientoDescripcion = "";

            info.FotoUrlLista = new List<string>();
            info.FotoDescripcionLista = new List<string>();
            info.Longitud = alojamiento.Longitud;
            info.Latitud = alojamiento.Latitud;

            var sitioAlojamiento = Config.LeerSetting("ArgentinahtlUrlSitiosAlojamientos") + alojamiento.UrlSubdominio.Replace(" ", "%20") + "/";
            var directorioAlojamiento = Config.LeerSetting("ArgentinahtlDirectorioSitiosAlojamientos") + alojamiento.UrlSubdominio + "\\";
            
            // Se lee la descripción de la foto principal
            if (File.Exists(directorioAlojamiento + Config.LeerSetting("ArgentinahtlDirectorioRelPaginaPrincipal") + "descripcionGeneral.txt"))
            {
                using (var file = File.OpenText(directorioAlojamiento + Config.LeerSetting("ArgentinahtlDirectorioRelPaginaPrincipal") + "descripcionGeneral.txt"))
                {
                    info.FotoAlojamientoDescripcion = file.ReadToEnd();
                }
            }

            // Se indica la URL de la foto principal
            if (File.Exists(directorioAlojamiento + Config.LeerSetting("ArgentinahtlDirectorioRelFotoAlojamiento") + "logo.jpg")) 
                info.FotoAlojamientoUrl = sitioAlojamiento + Config.LeerSetting("ArgentinahtlUrlRelFotoAlojamiento") + "logo.jpg";

            // Se leen las descripciones de las 3 fotos de la página principal
            for (int i = 1; i <= 3; i++)
            {
                var fileName = string.Format("{0}{1}descImgAloj{2}.txt", directorioAlojamiento, Config.LeerSetting("ArgentinahtlDirectorioRelPaginaPrincipal"), i);
                if (File.Exists(fileName))
                {
                    using (var file = File.OpenText(fileName))
                    {
                        var description = file.ReadToEnd();
                        if(!string.IsNullOrWhiteSpace(description))
                            info.FotoDescripcionLista.Insert(info.FotoDescripcionLista.Count, description);
                    }
                }

                // Se indican las URL de las 3 fotos de la página principal
                if (File.Exists(string.Format("{0}{1}\\ImagenesPPAL\\imgAloj{2}.jpg", directorioAlojamiento, Config.LeerSetting("ArgentinahtlDirectorioRelPaginaPrincipal"), i)))
                    info.FotoUrlLista.Insert(info.FotoUrlLista.Count, string.Format("{0}{1}ImagenesPPAL/imgAloj{2}.jpg", sitioAlojamiento, Config.LeerSetting("ArgentinahtlUrlRelPaginaPrincipal"), i));
            }

            // Se leen las descripciones de las 6 fotos de Más fotos
            for (int i = 4; i <= 9; i++)
            {
                var fileName = string.Format("{0}{1}descImgAloj{2}.txt", directorioAlojamiento, Config.LeerSetting("ArgentinahtlDirectorioRelFotosAlojamientos"), i);
                if (File.Exists(fileName))
                {
                    using (var file = File.OpenText(fileName))
                    {
                        var description = file.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(description))
                            info.FotoDescripcionLista.Insert(info.FotoDescripcionLista.Count, description);
                    }
                }

                // Se indican las URL de las 6 fotos de Más fotos
                if (File.Exists(string.Format("{0}{1}\\ImagenesMASFOT\\imgAloj{2}.jpg", directorioAlojamiento, Config.LeerSetting("ArgentinahtlDirectorioRelFotosAlojamientos"), i)))
                    info.FotoUrlLista.Insert(info.FotoUrlLista.Count, string.Format("{0}{1}ImagenesMASFOT/imgAloj{2}.jpg", sitioAlojamiento, Config.LeerSetting("ArgentinahtlUrlRelFotosAlojamientos"), i));
            }

            info.Destino = new InfoDestino()
            {
                TipoDestino = TipoDestino.Ciudad,
                IdDestino = alojamiento.IdCiudad,
                NombreDestino = alojamiento.Ciudad.Nombre
            };

            //info.Moneda = ConvertirMoneda(alojamiento.IdMoneda.GetValueOrDefault());//habría que enviar la moneda del usuario NO la del hotel
            info.Moneda = ConvertirMoneda(idMonedaUsuario);
            
        }

        private static SmtpClient ObtenerClienteSmtp()
        {
            var smtp = new SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = Config.LeerSetting("MailUseSSL", false);
            smtp.Host = Config.LeerSetting("MailServer");
            smtp.Port = Config.LeerSetting("MailPort", 25);

            string user = Config.LeerSetting("MailUser");

            if (!String.IsNullOrEmpty(user))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Config.LeerSetting("MailUser"),
                        Config.LeerSetting("MailPassword"));
            }
            return smtp;
        }

        #endregion
    }


}