using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArgentinahtlBackend.Models;

namespace ArgentinahtlMVC.Models.Services
{
    public static partial class DbAccess
    {

        private static RateModel MapRate(CupoUnidad cupoUnidad)
        {
            return new RateModel()
            {
                Id = cupoUnidad.IDCUPOUNIDAD,
                UnidadAlojamiento = MapRoom(cupoUnidad.UnidadAlojamiento),
                Fecha = cupoUnidad.FECHA,
                FechaAlta = cupoUnidad.FECHA_ALTA,
                StartDate = cupoUnidad.FECHA,
                EndDate = cupoUnidad.FECHA,
                CupoMaximo = cupoUnidad.CUPOMAXIMO,
                CupoReservado = cupoUnidad.CUPORESERVADO,
                MontoRACDTR = cupoUnidad.MONTO,
                MontoEXTCDTR = cupoUnidad.MONTO_EXT_CD_TR,
                MontoMERCDTR = cupoUnidad.MONTO_MER_CD_TR,
                MontoRASDTR = cupoUnidad.MONTO_RA_SD_TR,
                MontoEXTSDTR = cupoUnidad.MONTO_EXT_SD_TR,
                MontoMERSDTR = cupoUnidad.MONTO_MER_SD_TR,
                MontoRACDTNR = cupoUnidad.MONTO_RA_CD_TNR,
                MontoEXTCDTNR = cupoUnidad.MONTO_EXT_CD_TNR,
                MontoMERCDTNR = cupoUnidad.MONTO_MER_CD_TNR,
                MontoRASDTNR = cupoUnidad.MONTO_RA_SD_TNR,
                MontoEXTSDTNR = cupoUnidad.MONTO_EXT_SD_TNR,
                MontoMERSDTNR = cupoUnidad.MONTO_MER_SD_TNR,
                Lun = (int)cupoUnidad.FECHA.DayOfWeek == 1 ? true : false,
                Mar = (int)cupoUnidad.FECHA.DayOfWeek == 2 ? true : false,
                Mie = (int)cupoUnidad.FECHA.DayOfWeek == 3 ? true : false,
                Jue = (int)cupoUnidad.FECHA.DayOfWeek == 4 ? true : false,
                Vie = (int)cupoUnidad.FECHA.DayOfWeek == 5 ? true : false,
                Sab = (int)cupoUnidad.FECHA.DayOfWeek == 6 ? true : false,
                Dom = (int)cupoUnidad.FECHA.DayOfWeek == 0 ? true : false
            };
        }

        public static RateModel GetRate(Guid cupoUnidadId)
        {
            using (var dc = new TurismoDataContext())
            {
                var rate = dc.CupoUnidads.Single(s => s.IDCUPOUNIDAD == cupoUnidadId);

                return MapRate(rate);
            }
        }

        public static List<RateModel> GetRates()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.CupoUnidads.Select(s => MapRate(s)).ToList();
            }
        }

        public static List<RateModel> GetRates(Guid lodgingId, Guid roomId, DateTime fechaDesde, DateTime fechaHasta)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.CupoUnidads.Where(s => s.UnidadAlojamiento.IDALOJ == lodgingId
                                                && s.IDUNIDAD_ALOJ == roomId
                                                && s.FECHA >= fechaDesde && s.FECHA <= fechaHasta)
                    .OrderBy(s => s.FECHA).Select(s => MapRate(s)).ToList();
            }
        }

        public static bool CreateRate(RateModel rate)
        {
            using (var dc = new TurismoDataContext())
            {
                var cupounidad = dc.CupoUnidads.Where(s => s.IDUNIDAD_ALOJ == rate.UnidadAlojamiento.RoomId && s.FECHA == rate.Fecha);

                if (cupounidad != null && cupounidad.Count() > 0)
                    return false;

                dc.CupoUnidads.InsertOnSubmit(new CupoUnidad
                {
                    IDCUPOUNIDAD = rate.Id,
                    IDUNIDAD_ALOJ = rate.UnidadAlojamiento.RoomId,
                    FECHA = rate.Fecha,
                    FECHA_ALTA = rate.FechaAlta,
                    CUPOMAXIMO = rate.CupoMaximo,
                    CUPORESERVADO = rate.CupoReservado,
                    MONTO = rate.MontoRACDTR,
                    MONTO_EXT_CD_TR = rate.MontoEXTCDTR,
                    MONTO_MER_CD_TR = rate.MontoMERCDTR,
                    MONTO_RA_SD_TR = rate.MontoRASDTR,
                    MONTO_EXT_SD_TR = rate.MontoEXTSDTR,
                    MONTO_MER_SD_TR = rate.MontoMERSDTR,
                    MONTO_RA_CD_TNR = rate.MontoRACDTNR,
                    MONTO_EXT_CD_TNR = rate.MontoEXTCDTNR,
                    MONTO_MER_CD_TNR = rate.MontoMERCDTNR,
                    MONTO_RA_SD_TNR = rate.MontoRASDTNR,
                    MONTO_EXT_SD_TNR = rate.MontoEXTSDTNR,
                    MONTO_MER_SD_TNR = rate.MontoMERSDTNR
                });

                dc.SubmitChanges();
            }

            return true;
        }

        public static bool SaveRate(RateModel rate)
        {
            if (rate != null)
            {
                using (var dc = new TurismoDataContext())
                {
                    var tarifas = dc.CupoUnidads.SingleOrDefault(s => s.IDCUPOUNIDAD == rate.Id);

                    tarifas.CUPOMAXIMO = rate.CupoMaximo;
                    tarifas.CUPORESERVADO = rate.CupoReservado;
                    tarifas.MONTO = rate.MontoRACDTR;
                    tarifas.MONTO_EXT_CD_TR = rate.MontoEXTCDTR;
                    tarifas.MONTO_MER_CD_TR = rate.MontoMERCDTR;
                    tarifas.MONTO_RA_SD_TR = rate.MontoRASDTR;
                    tarifas.MONTO_EXT_SD_TR = rate.MontoEXTSDTR;
                    tarifas.MONTO_MER_SD_TR = rate.MontoMERSDTR;
                    tarifas.MONTO_RA_CD_TNR = rate.MontoRACDTNR;
                    tarifas.MONTO_EXT_CD_TNR = rate.MontoEXTCDTNR;
                    tarifas.MONTO_MER_CD_TNR = rate.MontoMERCDTNR;
                    tarifas.MONTO_RA_SD_TNR = rate.MontoRASDTNR;
                    tarifas.MONTO_EXT_SD_TNR = rate.MontoEXTSDTNR;
                    tarifas.MONTO_MER_SD_TNR = rate.MontoMERSDTNR;

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool DeleteRate(Guid cupoUnidadId)
        {
            if (cupoUnidadId != Guid.Empty)
            {
                using (var dc = new TurismoDataContext())
                {
                    var rate = dc.CupoUnidads.Single(s => s.IDCUPOUNIDAD == cupoUnidadId);

                    dc.CupoUnidads.DeleteOnSubmit(rate);

                    dc.SubmitChanges();
                }

                return true;
            }

            return false;
        }

        public static bool CreateOrUpdateRates(RateModel model)
        {
            if (model != null)
            {
                using (var dc = new TurismoDataContext())
                {
                    DateTime fecha = model.StartDate;

                    while (fecha <= model.EndDate)
                    {
                        if ((int)fecha.DayOfWeek == 0 && model.Dom || (int)fecha.DayOfWeek == 1 && model.Lun ||
                           (int)fecha.DayOfWeek == 2 && model.Mar || (int)fecha.DayOfWeek == 3 && model.Mie ||
                           (int)fecha.DayOfWeek == 4 && model.Jue || (int)fecha.DayOfWeek == 5 && model.Vie ||
                           (int)fecha.DayOfWeek == 6 && model.Sab)
                        {

                            var cupounidad = dc.CupoUnidads.SingleOrDefault(s => s.IDUNIDAD_ALOJ == model.UnidadAlojamiento.RoomId && s.FECHA == fecha);

                            if (cupounidad != null)
                            {
                                //if (model.CupoMaximo < cupounidad.CUPORESERVADO)
                                //    cupounidad.CUPOMAXIMO = cupounidad.CUPORESERVADO;
                                //else
                                //    cupounidad.CUPOMAXIMO = model.CupoMaximo;

                                if(model.CupoMaximo > cupounidad.CUPORESERVADO && cupounidad.CUPOMAXIMO > cupounidad.CUPORESERVADO )
                                    cupounidad.CUPOMAXIMO = model.CupoMaximo;

                                //cupounidad.CUPORESERVADO = model.CupoReservado;
                                cupounidad.MONTO = model.MontoRACDTR;
                                cupounidad.MONTO_EXT_CD_TR = model.MontoEXTCDTR;
                                cupounidad.MONTO_MER_CD_TR = model.MontoMERCDTR;
                                cupounidad.MONTO_RA_SD_TR = model.MontoRASDTR;
                                cupounidad.MONTO_EXT_SD_TR = model.MontoEXTSDTR;
                                cupounidad.MONTO_MER_SD_TR = model.MontoMERSDTR;
                                cupounidad.MONTO_RA_CD_TNR = model.MontoRACDTNR;
                                cupounidad.MONTO_EXT_CD_TNR = model.MontoEXTCDTNR;
                                cupounidad.MONTO_MER_CD_TNR = model.MontoMERCDTNR;
                                cupounidad.MONTO_RA_SD_TNR = model.MontoRASDTNR;
                                cupounidad.MONTO_EXT_SD_TNR = model.MontoEXTSDTNR;
                                cupounidad.MONTO_MER_SD_TNR = model.MontoMERSDTNR;
                            }
                            else
                            {

                                dc.CupoUnidads.InsertOnSubmit(new CupoUnidad
                                {
                                    IDCUPOUNIDAD = Guid.NewGuid(),
                                    IDUNIDAD_ALOJ = model.UnidadAlojamiento.RoomId,
                                    FECHA = fecha,
                                    FECHA_ALTA = model.FechaAlta,
                                    CUPOMAXIMO = model.CupoMaximo,
                                    CUPORESERVADO = model.CupoReservado, //deberìa llegar 0
                                    MONTO = model.MontoRACDTR,
                                    MONTO_EXT_CD_TR = model.MontoEXTCDTR,
                                    MONTO_MER_CD_TR = model.MontoMERCDTR,
                                    MONTO_RA_SD_TR = model.MontoRASDTR,
                                    MONTO_EXT_SD_TR = model.MontoEXTSDTR,
                                    MONTO_MER_SD_TR = model.MontoMERSDTR,
                                    MONTO_RA_CD_TNR = model.MontoRACDTNR,
                                    MONTO_EXT_CD_TNR = model.MontoEXTCDTNR,
                                    MONTO_MER_CD_TNR = model.MontoMERCDTNR,
                                    MONTO_RA_SD_TNR = model.MontoRASDTNR,
                                    MONTO_EXT_SD_TNR = model.MontoEXTSDTNR,
                                    MONTO_MER_SD_TNR = model.MontoMERSDTNR,
                                    ACTIVO = true
                                });
                            }

                            dc.SubmitChanges();

                        }

                        fecha = fecha.AddDays(1);
                    }

                    return true;
                }

            }

            return false;
        }

        public static bool SaveCierre(Guid idCupoUnidad, int cupoASumar)
        {
            if (idCupoUnidad != null)
            {
                using (var dc = new TurismoDataContext())
                {
                    var cupoUnidad = dc.CupoUnidads.SingleOrDefault(s => s.IDCUPOUNIDAD == idCupoUnidad);

                    if (cupoUnidad != null)
                    {
                        if (cupoUnidad.CUPOMAXIMO > cupoUnidad.CUPORESERVADO)
                            cupoUnidad.CUPOMAXIMO = cupoUnidad.CUPORESERVADO;
                        else
                            cupoUnidad.CUPOMAXIMO += cupoASumar;
                        //cupoUnidad.CUPOMAXIMO += cupoUnidad.UnidadAlojamiento.CUPOPORDEFECTO;

                        dc.SubmitChanges();
                        return true;
                    }
                }

            }

            return false;
        }

        public static bool InsertarCupo(string idUnidadAlojamiento, int anio, int mes, int dia, int cupoAInsertar)
        {
            using (var dc = new TurismoDataContext())
            {
                var idua = new Guid(idUnidadAlojamiento);

                var uniAloj = dc.UnidadAlojamientos.SingleOrDefault(s => s.IDUNIDAD_ALOJ == idua);

                if (uniAloj != null)
                {

                    dc.CupoUnidads.InsertOnSubmit(new CupoUnidad
                    {
                        IDCUPOUNIDAD = Guid.NewGuid(),
                        IDUNIDAD_ALOJ = idua,
                        FECHA = new DateTime(anio, mes, dia),
                        FECHA_ALTA = DateTime.Now,
                        //CUPOMAXIMO = uniAloj.CUPOPORDEFECTO,
                        CUPOMAXIMO = cupoAInsertar,
                        CUPORESERVADO = 0,
                        MONTO = uniAloj.MONTOPORDEFECTO,
                        ACTIVO = true
                    });

                    dc.SubmitChanges();
                    return true;
                }
            }

            return false;
        }

        public static bool ActualizarTarifas(RateModel model)
        {
            using (var dc = new TurismoDataContext())
            {
                var cupounidad = dc.CupoUnidads.SingleOrDefault(s => s.IDCUPOUNIDAD == model.Id);

                if (cupounidad != null)
                {
                    cupounidad.MONTO = model.MontoRACDTR;
                    cupounidad.MONTO_EXT_CD_TR = model.MontoEXTCDTR ?? cupounidad.MONTO_EXT_CD_TR;
                    cupounidad.MONTO_MER_CD_TR = model.MontoMERCDTR ?? cupounidad.MONTO_MER_CD_TR;
                    cupounidad.MONTO_RA_SD_TR = model.MontoRASDTR ?? cupounidad.MONTO_RA_SD_TR;
                    cupounidad.MONTO_EXT_SD_TR = model.MontoEXTSDTR ?? cupounidad.MONTO_EXT_SD_TR;
                    cupounidad.MONTO_MER_SD_TR = model.MontoMERSDTR ?? cupounidad.MONTO_MER_SD_TR;
                    cupounidad.MONTO_RA_CD_TNR = model.MontoRACDTNR ?? cupounidad.MONTO_RA_CD_TNR;
                    cupounidad.MONTO_EXT_CD_TNR = model.MontoEXTCDTNR ?? cupounidad.MONTO_EXT_CD_TNR;
                    cupounidad.MONTO_MER_CD_TNR = model.MontoMERCDTNR ?? cupounidad.MONTO_MER_CD_TNR;
                    cupounidad.MONTO_RA_SD_TNR = model.MontoRASDTNR ?? cupounidad.MONTO_RA_SD_TNR;
                    cupounidad.MONTO_EXT_SD_TNR = model.MontoEXTSDTNR ?? cupounidad.MONTO_EXT_SD_TNR;
                    cupounidad.MONTO_MER_SD_TNR = model.MontoMERSDTNR ?? cupounidad.MONTO_MER_SD_TNR;

                    dc.SubmitChanges();
                    return true;
                }
            }

            return false;
        }
    }
}