using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using ArgentinahtlMVC.Models.Services;
using ArgentinahtlMVC.Models;

namespace ArgentinahtlMVC.Controllers
{
    public class RateController : Controller
    {
        //
        public ActionResult Menu()
        {
            return View();
        }

        // GET: /Rate/

        public ActionResult Index()
        {
            return View();
        }


        [UserProfile(UserProfile.Administrator)]
        public ActionResult Rates()
        {
            return View(new SeasonsModel()
            {
                Provinces = DbAccess.GetProvinces()
            });
        }

        [UserProfile(UserProfile.Administrator)]
        public ActionResult RateList(Guid? lodgingId, Guid? roomId, string fechaDesde, string fechaHasta)
        {

            IFormatProvider culture = new CultureInfo("en-US", true); 
            DateTime fdesde = DateTime.ParseExact(fechaDesde.Replace("{","").Replace("}",""), "dd/MM/yyyy", culture);
            DateTime fhasta = DateTime.ParseExact(fechaHasta.Replace("{","").Replace("}",""), "dd/MM/yyyy", culture);

            return View(new RateListModel()
            {
                //SeasonTypes = DbAccess.GetSeasonTypes(),
                Rates = DbAccess.GetRates(lodgingId.GetValueOrDefault(), roomId.GetValueOrDefault(), fdesde, fhasta),
                Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())
            });
        }

        [UserProfile(UserProfile.Administrator)]
        public ActionResult CreateRate(Guid roomId)
        {
            return PartialView("CreateEditRate", new RateModel
            {
                UnidadAlojamiento = DbAccess.GetRoom(roomId),
                //LodgingId = lodgingId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                FechaAlta = DateTime.Today
                //SeasonTypes = DbAccess.GetSeasonTypes()
            });
        }

        [HttpPost]
        [UserProfile(UserProfile.Superadmin)]
        public ActionResult DeleteRate(Guid rateId)
        {
            if (DbAccess.DeleteRate(rateId))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult CreateOrUpdateRates(RateModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.CreateOrUpdateRates(model))
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

        [HttpGet]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditRate(Guid rateId)
        {
            var model = DbAccess.GetRate(rateId);
           // model.SeasonTypes = DbAccess.GetSeasonTypes();

            return PartialView("CreateEditRate", model);
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditRate(RateModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.SaveRate(model))
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

        [UserProfile(UserProfile.Administrator)]
        public ActionResult Cierres(Guid? lodgingId, Guid? roomId, string fechaDesde, string fechaHasta)
        {
            //Guid l = new Guid("d4867abb-f568-4abe-a11b-85932390c520");
            //Guid r = new Guid("291d436d-9c50-46e0-98bd-2095595c41b0");
            Guid l = lodgingId.HasValue ? (Guid)lodgingId : Guid.Empty;
            Guid r = roomId.HasValue ? (Guid)roomId : Guid.Empty; 
            //DateTime fd = new DateTime(2017,1,1);
            //DateTime fh = new DateTime(2017, 12, 31);

            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime fd = DateTime.ParseExact(fechaDesde.Replace("{", "").Replace("}", ""), "dd/MM/yyyy", culture);
            DateTime fh = DateTime.ParseExact(fechaHasta.Replace("{", "").Replace("}", ""), "dd/MM/yyyy", culture);//esta línea no tiene sentido

            fd = new DateTime(fd.Year, 1, 1);
            fh = new DateTime(fd.Year, 12, 31);

            RateListModel m = new RateListModel()
            {
                //Rates = DbAccess.GetRates(lodgingId.GetValueOrDefault(), roomId.GetValueOrDefault(), fdesde, fhasta),
                Rates = DbAccess.GetRates(l,r,fd,fh),
                //Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())
            };

            m.TablaCupos = Utils.GenerarTablaHTML(m.Rates, fd, "Cupos");

            return View(m);

        }


        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult realizarCierre()
        {
            Guid idcu;
            string respuesta = string.Empty;
            string roomid = string.Empty;
            int anio = 0;
            int cupoAInsertarGrises = 0;
            int cupoASumarRojos = 0;

            foreach (string name in Request.Form.AllKeys)
            {
                var idCupoUnidad = string.Empty;

                if (name.Equals("roomid")) 
                    roomid = Request.Form[name];
                else if (name.Equals("anio"))
                    anio = Int32.Parse(Request.Form[name]);
                else if (name.Equals("cupogris"))
                    cupoAInsertarGrises = Int32.Parse(Request.Form[name]);
                else if (name.Equals("cuporojo"))
                    cupoASumarRojos = Int32.Parse(Request.Form[name]);
                else
                {
                    idCupoUnidad = name; //Request.Form[name] si quisiera obtener el valor de ese control

                    if (!Guid.TryParse(idCupoUnidad, out idcu) && !string.IsNullOrEmpty(roomid))
                    {  
                        //respuesta += " IdCupoUnidad no válido: " + idCupoUnidad;
                        DbAccess.InsertarCupo(roomid, anio, Int32.Parse(name.Substring(0, 2)), Int32.Parse(name.Substring(2, 2)), cupoAInsertarGrises);
                    }
                    else
                    {
                        if (!DbAccess.SaveCierre(idcu, cupoASumarRojos))
                            respuesta += " Error al cerrar " + idcu.ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(respuesta))
                return Json(new { OK = true }, JsonRequestBehavior.DenyGet);
            else
                return Json(new { OK = false, Descripcion = respuesta }, JsonRequestBehavior.DenyGet);
        }

        #region tarifas
        [UserProfile(UserProfile.Administrator)]
        public ActionResult Tarifas(Guid? lodgingId, Guid? roomId, string fechaDesde, string fechaHasta)
        {
            //Guid l = new Guid("d4867abb-f568-4abe-a11b-85932390c520");
            //Guid r = new Guid("291d436d-9c50-46e0-98bd-2095595c41b0");
            Guid l = lodgingId.HasValue ? (Guid)lodgingId : Guid.Empty;
            Guid r = roomId.HasValue ? (Guid)roomId : Guid.Empty;
            //DateTime fd = new DateTime(2017,1,1);
            //DateTime fh = new DateTime(2017, 12, 31);

            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime fd = DateTime.ParseExact(fechaDesde.Replace("{", "").Replace("}", ""), "dd/MM/yyyy", culture);
            DateTime fh = DateTime.ParseExact(fechaHasta.Replace("{", "").Replace("}", ""), "dd/MM/yyyy", culture);//esta línea no tiene sentido

            fd = new DateTime(fd.Year, 1, 1);
            fh = new DateTime(fd.Year, 12, 31);

            RateListModel m = new RateListModel()
            {
                //Rates = DbAccess.GetRates(lodgingId.GetValueOrDefault(), roomId.GetValueOrDefault(), fdesde, fhasta),
                Rates = DbAccess.GetRates(l, r, fd, fh),
                //Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())
            };

            m.TablaCupos = Utils.GenerarTablaHTML(m.Rates, fd, "Tarifas");

            return View(m);

        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult ActualizarTarifa()
        {
            Guid idcu;
            float t;
            string respuesta = string.Empty;
            string roomid = string.Empty;

            RateModel model = new RateModel();

            foreach (string name in Request.Form.AllKeys)
            {
                var idCupoUnidad = string.Empty;

                if (name.Contains("Rates[0].Monto"))
                {
                    if (!string.IsNullOrEmpty(Request.Form[name]))
                    {
                        if (!float.TryParse(Request.Form[name], out t))
                            return Json(new { OK = false, Descripcion = name + " tiene formato incorrecto" }, JsonRequestBehavior.DenyGet);
                        else
                        {
                            if (name.Contains("RACDTR"))
                                model.MontoRACDTR = t;
                            else if (name.Contains("EXTCDTR"))
                                model.MontoEXTCDTR = t;
                            else if (name.Contains("MERCDTR"))
                                model.MontoMERCDTR = t;
                            else if (name.Contains("RASDTR"))
                                model.MontoRASDTR = t;
                            else if (name.Contains("EXTSDTR"))
                                model.MontoEXTSDTR = t;
                            else if (name.Contains("MERSDTR"))
                                model.MontoMERSDTR = t;
                            else if (name.Contains("RACDTNR"))
                                model.MontoRACDTNR = t;
                            else if (name.Contains("EXTCDTNR"))
                                model.MontoEXTCDTNR = t;
                            else if (name.Contains("MERCDTNR"))
                                model.MontoMERCDTNR = t;
                            else if (name.Contains("RASDTNR"))
                                model.MontoRASDTNR = t;
                            else if (name.Contains("EXTSDTNR"))
                                model.MontoEXTSDTNR = t;
                            else if (name.Contains("MERSDTNR"))
                                model.MontoMERSDTNR = t;
                        }
                    }
                        
                }
                else
                {
                    idCupoUnidad = name;

                    if (Guid.TryParse(idCupoUnidad, out idcu))
                    {
                        model.Id = idcu;
                        DbAccess.ActualizarTarifas(model);
                    }
                }
            }

            if (string.IsNullOrEmpty(respuesta))
                return Json(new { OK = true }, JsonRequestBehavior.DenyGet);
            else
                return Json(new { OK = false, Descripcion = respuesta }, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Season management

        // **************************************
        // URL: /Management/EnableUser
        // **************************************
        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EnableSeason(Guid seasonId)
        {
            if (DbAccess.EnableDisableSeason(seasonId, true))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        // **************************************
        // URL: /Management/DisableUser
        // **************************************
        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult DisableSeason(Guid seasonId)
        {
            if (DbAccess.EnableDisableSeason(seasonId, false))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        // **************************************
        // URL: /Management/DeleteUser
        // **************************************
        [HttpPost]
        [UserProfile(UserProfile.Superadmin)]
        public ActionResult DeleteSeason(Guid seasonId)
        {
            if (DbAccess.DeleteSeason(seasonId, true))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult GetRooms(string lodgingId)
        {
            return Json(DbAccess.GetRooms(Guid.Parse(lodgingId)), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
