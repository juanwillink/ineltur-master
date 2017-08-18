using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArgentinahtlMVC.Models.Services;
using ArgentinahtlMVC.Models;

namespace ArgentinahtlMVC.Controllers
{
    public class LodgingController : Controller
    {
        public ActionResult Menu()
        {
            return View();
        }

        #region Season management

        [UserProfile(UserProfile.Administrator)]
        public ActionResult Seasons()
        {
            return View(new SeasonsModel()
            {
                Provinces = DbAccess.GetProvinces()
            });
        }

        [UserProfile(UserProfile.Administrator)]
        public ActionResult Tarifas()
        {
            return View(new TarifasModel()
            {
                Provinces = DbAccess.GetProvinces()
            });
        }

        [UserProfile(UserProfile.Administrator)]
        public ActionResult Promociones()
        {
            return View(new PromocionesModel()
            {
                Provinces = DbAccess.GetProvinces()
            });
        }


        // **************************************
        // URL: /Management/Users
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult SeasonList(Guid? lodgingId)
        {
            return View(new SeasonListModel()
            {
                SeasonTypes = DbAccess.GetSeasonTypes(),
                Seasons = DbAccess.GetSeasons(lodgingId.GetValueOrDefault()),
                Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())
            });
        }

        // **************************************
        // URL: /Management/Users
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult TarifasList(Guid? lodgingId)
        {
            //TarifasListModel tlm = new TarifasListModel();
            //tlm.TarifasTypes = DbAccess.GetTarifaTypes(),
            //tlm.Tarifas = DbAccess.GetTarifas(lodgingId.GetValueOrDefault()),
            //tlm.Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())

            return View(new TarifasListModel()
            {
                TarifasTypes = DbAccess.GetTarifaTypes(),
                Tarifas = DbAccess.GetTarifas(lodgingId.GetValueOrDefault()),
                Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())
            });
        }

        // **************************************
        // URL: /Management/Users
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult PromocionesList(Guid? lodgingId)
        {
            //TarifasListModel tlm = new TarifasListModel();
            //tlm.TarifasTypes = DbAccess.GetTarifaTypes(),
            //tlm.Tarifas = DbAccess.GetTarifas(lodgingId.GetValueOrDefault()),
            //tlm.Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())

            return View(new PromocionesListModel()
            {
                PromocionesType = DbAccess.GetPromocionesTypes(),
                Promociones = DbAccess.GetPromociones(lodgingId.GetValueOrDefault()),
                Lodging = DbAccess.GetLodging(lodgingId.GetValueOrDefault())
            });
        }

        // **************************************
        // URL: /Management/NewUser
        // **************************************

        [UserProfile(UserProfile.Administrator)]
        public ActionResult CreateSeason(Guid lodgingId)
        {
            return PartialView("CreateEditSeason", new SeasonModel
            {
                LodgingId = lodgingId,                
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                DateOfRegistration = DateTime.Today,
                SeasonTypes = DbAccess.GetSeasonTypes()
            });
        }

        public ActionResult CreateTarifa(Guid? lodgingId)
        {
            return PartialView("CreateEditTarifa", new TarifaModel
            {
                LodgingId = lodgingId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                TarifasTypes = DbAccess.GetTarifaTypes()
            });
        }

        public ActionResult CreatePromocion(Guid lodgingId)
        {
            return PartialView("CreateEditPromocion", new PromocionModel
            {
                LodgingId = lodgingId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                PromocionesType = DbAccess.GetPromocionesTypes(),
                Lodging = DbAccess.GetLodging(lodgingId)
            });
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult CreateTarifa(TarifaModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.CreateTarifa(model.NationalityId, model.LodgingId.GetValueOrDefault(),
                        model.StartDate, model.EndDate, model.TarifaTypeId))
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult CreatePromocion(PromocionModel promocion)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.CreatePromocion(promocion))
                {
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult CreateSeason(SeasonModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.CreateSeason(model.SeasonName, model.LodgingId.GetValueOrDefault(),
                        model.StartDate, model.EndDate, model.SeasonTypeId.GetValueOrDefault(), model.Deadline))
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

        [HttpGet]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditSeason(Guid seasonId)
        {
            var model = DbAccess.GetSeason(seasonId);
            model.SeasonTypes = DbAccess.GetSeasonTypes();

            return PartialView("CreateEditSeason", model);
        }

        [HttpGet]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditTarifa(long? tarifaId)
        {
            var model = DbAccess.GetTarifa(tarifaId);
            model.TarifasTypes = DbAccess.GetTarifaTypes();

            return PartialView("CreateEditTarifa", model);
        }

        [HttpGet]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditPromocion(Guid promocionId)
        {
            var model = DbAccess.GetPromocion(promocionId);
            model.PromocionesType = DbAccess.GetPromocionesTypes();
            model.Lodging = DbAccess.GetLodging(model.LodgingId);

            return PartialView("CreateEditPromocion", model);
        }


        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditSeason(SeasonModel model)
        {
            
            if (DbAccess.SaveSeason(model.SeasonId.GetValueOrDefault(), model.SeasonName, model.LodgingId.GetValueOrDefault(),
                    model.StartDate, model.EndDate, model.SeasonTypeId.GetValueOrDefault(), model.Deadline))
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditTarifa(TarifaModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.SaveTarifa(model.TarifaId, model.NationalityId, model.LodgingId.GetValueOrDefault(),
                        model.StartDate, model.EndDate, model.TarifaTypeId))
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult EditPromocion(PromocionModel model)
        {
            if (ModelState.IsValid)
            {
                if (DbAccess.SavePromocion(model))
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet); ;
        }

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
        [UserProfile(UserProfile.Administrator)]
        public ActionResult DeleteSeason(Guid seasonId)
        {
            if (DbAccess.DeleteSeason(seasonId, true))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        // **************************************
        // URL: /Management/DeleteUser
        // **************************************
        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult DeleteTarifa(long tarifaId)
        {
            if (DbAccess.DeleteTarifa(tarifaId, true))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        // **************************************
        // URL: /Management/DeleteUser
        // **************************************
        [HttpPost]
        [UserProfile(UserProfile.Administrator)]
        public ActionResult DeletePromocion(Guid promocionId)
        {
            if (DbAccess.DeletePromocion(promocionId, true))
            {
                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        [HttpGet]
        public ActionResult GetCities(string provinceId)
        {
            return Json(DbAccess.GetCities(Guid.Parse(provinceId)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetLodgings(string cityId)
        {
            return Json(DbAccess.GetLodgings(Guid.Parse(cityId)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SearchLodgings(string lodgingName)
        {
            return Json(DbAccess.SearchLodgings(lodgingName), JsonRequestBehavior.AllowGet);
        }
    }
}
