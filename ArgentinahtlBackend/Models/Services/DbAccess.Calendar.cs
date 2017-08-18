using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArgentinahtlBackend.Models;

namespace ArgentinahtlMVC.Models.Services
{
    public static partial class DbAccess
    {
        private static SeasonTypeModel MapSeasonType(TipoTemporada seasonType)
        {
            return new SeasonTypeModel()
            {
                SeasonTypeId = seasonType.IDTIPOTEMPORADA,
                SeasonTypeName = seasonType.NOMBRE,
                Description = seasonType.DESCRIPCION
            };
        }

        private static TarifasTypeModel MapTarifaType(Moneda tarifaType)
        {
            return new TarifasTypeModel()
            {
                TarifaTypeId = tarifaType.IDMONEDA,
                TarifaTypeSimbolo = tarifaType.SIMBOLO,
                Description = tarifaType.NOMBRE
            };
        }

        private static PromocionesTypeModel MapPromocionType(Tipo_Promociones_Alojamiento promocionType)
        {
            return new PromocionesTypeModel()
            {
                PromocionTypeId = promocionType.IDTIPOPROMOCION,
                Nombre = promocionType.NOMBRE,
                Description = promocionType.DESCRIPCION,
                DiasMax = promocionType.TOPEDIASMAX,
                DiasMin = promocionType.TOPEDIASMIN,
                Codigo = promocionType.CODIGO
            };
        }

        private static SeasonModel MapSeason(Temporada season)
        {
            return new SeasonModel()
            {
                SeasonId = season.IDTEMPORADA,
                SeasonName = season.NOMBRE,
                LodgingId = season.IDALOJ,
                StartDate = season.FECHA_INICIO,
                EndDate = season.FECHA_FIN,
                DateOfRegistration = season.FECHA_ALTA,
                Enabled = season.ACTIVO,
                Deadline = season.DEADLINE,
                SeasonTypeId = season.TIPO_TEMPORADA
            };
        }

        private static TarifaModel MapTarifa(Tarifas_Alojamiento tarifa)
        {
            return new TarifaModel()
            {
                TarifaId = tarifa.IdTarifasAloj,
                LodgingId = tarifa.IdAloj,
                StartDate = tarifa.FechaDesde,
                EndDate = tarifa.FechaHasta ?? DateTime.Now,
                TarifaTypeId = tarifa.IdMoneda,
                NationalityId = tarifa.IdNacionalidad
            };
        }

        private static PromocionModel MapPromocion(Promociones_Alojamiento promocion)
        {
            return new PromocionModel()
            {
                PromocionId = promocion.IDPROMOCION,
                LodgingId = promocion.IDALOJ,
                Nombre = promocion.NOMBRE,
                Descripcion1 = promocion.DESCRIPCION,
                Descripcion2 = promocion.DESCRIPCION2,
                PromocionTypeId = promocion.IDTIPOPUBLICACIONPROMO,
                IdUnidadPromo = promocion.IDUNIDADPROMO,
                Activo = promocion.ACTIVO,
                StartDate = promocion.FECHAINICIO,
                EndDate = promocion.FECHAFIN,
                DiasReservados = promocion.DIASRESERVADOS,
                DiasACobrar = promocion.DIASACOBRAR,
                Descuento = promocion.DESCUENTO,
                MinimoNoches = promocion.MINIMONOCHES,
                MaximoNoches = promocion.MAXIMONOCHES,
                Slogan = promocion.SLOGAN
            };
        }

        public static SeasonModel GetSeason(Guid seasonId)
        {
            using (var dc = new TurismoDataContext())
            {
                var season = dc.Temporadas.Single(s => s.IDTEMPORADA == seasonId);

                return MapSeason(season);
            }
        }

        public static TarifaModel GetTarifa(long? tarifaId)
        {
            using (var dc = new TurismoDataContext())
            {
                var tarifa = dc.Tarifas_Alojamientos.Single(s => s.IdTarifasAloj == tarifaId);

                return MapTarifa(tarifa);
            }
        }

        public static PromocionModel GetPromocion(Guid promocionId)
        {
            using (var dc = new TurismoDataContext())
            {
                var promocion = dc.Promociones_Alojamientos.Single(p => p.IDPROMOCION == promocionId);
                return MapPromocion(promocion);
            }
        }

        public static List<SeasonModel> GetSeasons()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Temporadas.Select(s => MapSeason(s)).ToList();
            }
        }

        public static List<SeasonModel> GetSeasons(Guid lodgingId)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Temporadas.Where(s => s.IDALOJ == lodgingId)
                    .OrderBy(s => s.FECHA_INICIO).Select(s => MapSeason(s)).ToList();
            }
        }

        public static List<TarifaModel> GetTarifas()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Tarifas_Alojamientos.Select(s => MapTarifa(s)).ToList();
            }
        }

        public static List<TarifaModel> GetTarifas(Guid lodgingId)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Tarifas_Alojamientos.Where(s => s.IdAloj == lodgingId)
                    .OrderBy(s => s.FechaDesde).Select(s => MapTarifa(s)).ToList();
            }
        }

        public static List<PromocionModel> GetPromociones(Guid lodgingId)
        {
            using (var dc = new TurismoDataContext()) 
            {
                return dc.Promociones_Alojamientos.Where(p => p.IDALOJ == lodgingId)
                    .OrderBy(p => p.FECHAINICIO).Select(p => MapPromocion(p)).ToList();
            }
        }

        public static List<SeasonTypeModel> GetSeasonTypes()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.TipoTemporadas.Where(st => st.ACTIVO).Select(st => MapSeasonType(st)).ToList();
            }
        }

        public static List<TarifasTypeModel> GetTarifaTypes()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Monedas.Where(st => st.ACTIVO).Select(st => MapTarifaType(st)).ToList();
            }
        }

        public static List<PromocionesTypeModel> GetPromocionesTypes()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Tipo_Promociones_Alojamientos.Where(pt => pt.ACTIVO).Select(pt => MapPromocionType(pt)).ToList();
            }
        }

        public static bool CreateSeason(string name, Guid lodgingId, DateTime startDate, DateTime endDate, Guid seasonTypeId, int deadline)
        {
            using (var dc = new TurismoDataContext())
            {
                var seasons = dc.Temporadas.Where(s => s.IDALOJ == lodgingId &&
                    ((s.FECHA_INICIO <= startDate && startDate <= s.FECHA_FIN) ||
                    (s.FECHA_INICIO <= endDate && endDate <= s.FECHA_FIN)));

                if (seasons != null && seasons.Count() > 0)
                    return false;

                dc.Temporadas.InsertOnSubmit(new Temporada
                {
                    IDTEMPORADA = Guid.NewGuid(),
                    IDALOJ = lodgingId,
                    NOMBRE = name,
                    FECHA_INICIO = startDate,
                    FECHA_FIN = endDate,
                    FECHA_ALTA = DateTime.Now,
                    ACTIVO = true,
                    DEADLINE = deadline,
                    TIPO_TEMPORADA = seasonTypeId
                });

                dc.SubmitChanges();
            }

            return true;
        }

        public static bool CreateTarifa(string nacionalidad, Guid lodgingId, DateTime startDate, DateTime endDate, Guid tarifaTypeId)
        {
            using (var dc = new TurismoDataContext())
            {
                var tarifas = dc.Tarifas_Alojamientos.Where(s => s.IdAloj == lodgingId && s.IdNacionalidad == nacionalidad &&
                    ((s.FechaDesde <= startDate && startDate <= s.FechaHasta) ||
                    (s.FechaDesde <= endDate && endDate <= s.FechaHasta)));

                if (tarifas != null && tarifas.Count() > 0)
                    return false;

                dc.ExecuteCommand(@"INSERT INTO TARIFAS_ALOJAMIENTO (IDALOJ, IDNACIONALIDAD, IDMONEDA, FECHA_DESDE, FECHA_HASTA) VALUES ({0}, {1}, {2}, {3}, {4})", lodgingId, nacionalidad, tarifaTypeId, startDate, endDate);

                //dc.Tarifas_Alojamientos.InsertOnSubmit(new Tarifas_Alojamiento
                //{
                //    IdAloj = lodgingId,
                //    IdNacionalidad = nacionalidad,
                //    FechaDesde = startDate,
                //    FechaHasta = endDate,
                //    IdMoneda = tarifaTypeId
                //});

                //dc.SubmitChanges();
            }

            return true;
        }

        public static bool CreatePromocion(PromocionModel model)
        {
            using (var dc = new TurismoDataContext())
            {
                var promociones = dc.Promociones_Alojamientos.Where(p => p.IDALOJ == model.LodgingId &&
                    ((p.DIASRESERVADOS == model.DiasReservados)));

                if (promociones != null && promociones.Count() > 0)
                {
                    return false;
                }

                dc.Promociones_Alojamientos.InsertOnSubmit(new Promociones_Alojamiento
                {
                    IDPROMOCION = Guid.NewGuid(),
                    IDALOJ = model.LodgingId,
                    ACTIVO = true,
                    DESCRIPCION = model.Descripcion1,
                    DESCRIPCION2 = model.Descripcion2,
                    FECHAALTA = DateTime.Today.Date,
                    FECHAINICIO = model.StartDate,
                    FECHAFIN = model.EndDate,
                    NOMBRE = model.Nombre,
                    BLOQUEACUPOS = false,
                    DESCUENTO = model.Descuento,
                    DIASRESERVADOS = model.DiasReservados,
                    DIASACOBRAR = model.DiasACobrar,
                    FECHABAJAPUBLICACION = null,
                    FECHAFINPUBLICACION = null,
                    FECHAPUBLICACION = DateTime.Today.Date,
                    MINIMONOCHES = model.MinimoNoches,
                    MAXIMONOCHES = model.MaximoNoches,
                    FINVIGENCIAINDEFINIDO = null,
                    IDTIPOPUBLICACIONPROMO = model.PromocionTypeId,
                    IDUNIDADPROMO = model.IdUnidadPromo,
                    SLOGAN = model.Slogan
                });

                dc.SubmitChanges();
            }
            return true;
        }

        public static bool SaveSeason(Guid seasonId, string name, Guid lodgingId, DateTime startDate, DateTime endDate, Guid seasonTypeId, int deadline)
        {
            if (seasonId != Guid.Empty)
            {
                using (var dc = new TurismoDataContext())
                {
                    var seasons = dc.Temporadas.Where(s => s.IDALOJ == lodgingId &&
                        s.IDTEMPORADA != seasonId &&
                        ((s.FECHA_INICIO <= startDate && startDate <= s.FECHA_FIN) ||
                        (s.FECHA_INICIO <= endDate && endDate <= s.FECHA_FIN)));

                    if (seasons != null && seasons.Count() > 0)
                        return false;

                    var season = dc.Temporadas.Single(s => s.IDTEMPORADA == seasonId);

                    season.FECHA_INICIO = startDate;
                    season.FECHA_FIN = endDate;
                    season.DEADLINE = deadline;
                    season.TIPO_TEMPORADA = seasonTypeId;

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool SaveTarifa(long? tarifaId, string name, Guid lodgingId, DateTime startDate, DateTime endDate, Guid tarifaTypeId)
        {
            if (tarifaId != 0)
            {
                using (var dc = new TurismoDataContext())
                {
                    var tarifas = dc.Tarifas_Alojamientos.Where(s => s.IdAloj == lodgingId &&
                        s.IdTarifasAloj != tarifaId &&
                        ((s.FechaDesde <= startDate && startDate <= s.FechaHasta) ||
                        (s.FechaDesde <= endDate && endDate <= s.FechaHasta)));

                    if (tarifas != null && tarifas.Count() > 0)
                        return false;

                    var tarifa = dc.Tarifas_Alojamientos.Single(s => s.IdTarifasAloj == tarifaId);

                    tarifa.FechaDesde = startDate;
                    tarifa.FechaHasta = endDate;
                    tarifa.IdMoneda = tarifaTypeId;

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool SavePromocion(PromocionModel model)
        {
            if (model.PromocionId.ToString() != "00000000-0000-0000-000000000000")
            {
                using (var dc = new TurismoDataContext())
                {
                    var promociones = dc.Promociones_Alojamientos.Where(s => s.IDALOJ == model.LodgingId &&
                        s.IDPROMOCION != model.PromocionId &&
                        ((s.FECHAINICIO <= model.StartDate && model.StartDate <= s.FECHAFIN) ||
                        (s.FECHAINICIO <= model.EndDate && model.EndDate <= s.FECHAFIN)));

                    if (promociones != null && promociones.Count() > 0)
                        return false;

                    var promocion = dc.Promociones_Alojamientos.Single(s => s.IDPROMOCION == model.PromocionId);

                    promocion.NOMBRE = model.Nombre;
                    promocion.DESCRIPCION = model.Descripcion1;
                    promocion.DESCRIPCION2 = model.Descripcion2;
                    promocion.ACTIVO = model.Activo;
                    promocion.BLOQUEACUPOS = false;
                    promocion.DESCUENTO = model.Descuento;
                    promocion.DIASRESERVADOS = model.DiasReservados;
                    promocion.DIASACOBRAR = model.DiasACobrar;
                    promocion.FECHAFIN = model.EndDate;
                    promocion.FECHAINICIO = model.StartDate;
                    promocion.IDTIPOPUBLICACIONPROMO = model.PromocionTypeId;
                    promocion.IDUNIDADPROMO = model.IdUnidadPromo;
                    promocion.MINIMONOCHES = model.MinimoNoches;
                    promocion.MAXIMONOCHES = model.MaximoNoches;
                    promocion.SLOGAN = model.Slogan;

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool EnableDisableSeason(Guid seasonId, bool enable)
        {
            if (seasonId != Guid.Empty)
            {
                using (var dc = new TurismoDataContext())
                {
                    var season = dc.Temporadas.Single(s => s.IDTEMPORADA == seasonId);

                    season.ACTIVO = enable;

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool DeleteSeason(Guid seasonId, bool enable)
        {
            if (seasonId != Guid.Empty)
            {
                using (var dc = new TurismoDataContext())
                {
                    var season = dc.Temporadas.Single(s => s.IDTEMPORADA == seasonId);

                    dc.Temporadas.DeleteOnSubmit(season);

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool DeleteTarifa(long tarifaId, bool enable)
        {
            using (var dc = new TurismoDataContext())
            {
                var tarifa = dc.Tarifas_Alojamientos.Single(s => s.IdTarifasAloj == tarifaId.ToLong());

                dc.Tarifas_Alojamientos.DeleteOnSubmit(tarifa);

                dc.SubmitChanges();
            }

            return true;
        }

        public static bool DeletePromocion(Guid promocionId, bool enable)
        {
            using (var dc = new TurismoDataContext())
            {
                var promocion = dc.Promociones_Alojamientos.Single(p => p.IDPROMOCION == promocionId);
                dc.Promociones_Alojamientos.DeleteOnSubmit(promocion);
                dc.SubmitChanges();
            }
            return true;
        }
    }
}