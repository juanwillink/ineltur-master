using CheckArgentina.Models;
using CheckArgentina.LocalService;
using System;
using System.Linq;

using System.Collections.Generic;
using System.IO;

using CheckArgentina.Commons;

namespace CheckArgentina.Managers
{
    //public partial class Manager
    //{
    public class LocalManager
    {
        private ServiciosSoapSoap _service;
        private Manager _globalManager;

        public LocalManager()
        {
            _service = new LocalService.ServiciosSoapSoapClient();
        }

        public RespuestaLogin LoginAttemp(LoginModel loginModel)
        {
            var peticion = new PeticionLogin() { Usuario = loginModel.Username, Clave = loginModel.Password };

            try
            {
                var rptaLogin = _service.Login(peticion);

                return rptaLogin;
            }
            catch (Exception ex)
            {
                Exception a = ex;
                return null;
            }

        }

        public MyReservationListModel SearchMyReservations(Guid userKey, int reservationCode, string searchParameter)
        {
            var myReservations = new List<MyReservationModel>();
            var results = new MyReservationListModel();
            var peticion = new PeticionBuscarMisReservas()
            {
                SearchParameter = searchParameter,
                UserId = userKey,
                ReservationCode = reservationCode
            };

            try
            {
                var petitionResults = _service.BuscarMisReservas(peticion);
                if (petitionResults.Estado == EstadoRespuesta.Ok)
                {
                    foreach (var reserva in petitionResults.MisReservas)
                    {
                        var reserveModel = new MyReservationModel()
                        {
                            CodigoReserva = reserva.CodigoReserva,
                            Descripcion = reserva.Descripcion,
                            EstadoPago = reserva.EstadoPago,
                            EstadoReserva = GetEstadoReserva(reserva.EstadoReserva),
                            FechaAlta = reserva.FechaAlta,
                            FechaVencimiento = reserva.FechaVencimiento,
                            IdAlojamiento = reserva.IdAlojamiento,
                            IdFormaDePago = reserva.IdFormaDePago,
                            IdMoneda = reserva.IdMoneda,
                            IdPu = reserva.IdPu,
                            IdSitioOrigen = reserva.IdSitioOrigen,
                            IdTipoFormaDePago = reserva.IdTipoFormaDePago,
                            IdTransaccion = reserva.IdTransaccion,
                            IdUsuario = reserva.IdUsuario,
                            MontoTotalConDescuento = reserva.MontoTotalConDescuento,
                            MontoTotalSinDescuento = reserva.MontoTotalSinDescuento,
                            NombreAlojamiento = reserva.NombreAlojamiento,
                            NombreFormaDePago = reserva.NombreFormaDePago,
                            NombrePasajero = reserva.NombrePasajero,
                            TipoTransaccion = reserva.TipoTransaccion
                        };

                        var vacancies = new List<VacancyModel>();
                        var vacancyGroups = reserva.Unidades.GroupBy(u => u.IdUnidad);

                        foreach (var group in vacancyGroups)
                        {
                            var vacancy = group.Select(v => new VacancyModel()
                            {
                                VacancyCheckin = v.FechaInicio,
                                VacancyCheckout = v.FechaFin,
                                VacancyCount = int.Parse(v.Cantidad.ToString()),
                                VacancyDescription = v.NombreHabitacion,
                                VacancyPrice = Decimal.Round(Decimal.Parse(v.Monto.ToString()), 2),
                                ConfirmedVacancyPrice = Decimal.Round(Decimal.Parse(v.MontoTotal.ToString()), 2)
                            }).First();
                            vacancies.Add(vacancy);
                        }
                        var unidades = new VacancyListModel()
                        {
                            Vacancies = vacancies
                        };
                        reserveModel.Unidades = unidades;
                        myReservations.Add(reserveModel);
                    }
                }
                results.MyReservations = myReservations;
                return results;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }

        private string GetEstadoReserva(int estadoReserva)
        {
            switch (estadoReserva)
            {
                case -3:
                    return "A Cancelar";
                case -2:
                    return "En Proceso";
                case -1:
                    return "Sin Terminar";
                case 0:
                    return "A Constatar";
                case 1:
                    return "Efectiva";
                case 2:
                    return "Cancelada";
                case 3:
                    return "Rechazada";
                default:
                    return "Error";
            }
        }

        #region Credentials

        public Credential GetCredential(string userKey)
        {
            var credential = new Credential();
            using (var dc = new TurismoDataContext())
            {
                Guid idUsuario;

                if (Guid.TryParse(userKey, out idUsuario))
                {
                    var users = dc.Usuarios.Where(u => u.IDUSUARIO == idUsuario);

                    if (users.Count() == 1)
                    {
                        credential.Username = users.First().NOMBREUSUARIO;
                        credential.Password = users.First().CLAVE;
                        credential.Language = "es";
                    }
                    else
                    {
                        credential.Username = string.Empty;
                        credential.Password = string.Empty;
                        credential.Language = string.Empty;
                    }
                }
            }

            return credential;
        }
        #endregion

        private void CompletePetition(PeticionBase petition, Credential credential)
        {
            petition.Usuario = credential.Username;
            petition.Clave = credential.Password;
            petition.IdiomaDeseado = credential.Language;
        }

        public UnitListModel GetLodgingWeeklyPrices(Guid lodgingId, DateTime date, Credential userCredential)
        {
            SessionData.SearchType = SearchType.National;
            UnitListModel result = new UnitListModel();
            var petition = new PeticionInfoAlojamiento()
            {
                IdAlojamiento = lodgingId,
                Fecha = date
            };
            CompletePetition(petition, userCredential);
            var petitionResults = _service.BuscarPreciosSemanalesHotel(petition);
            if (petitionResults.Estado == EstadoRespuesta.Ok)
            {
                result.Units = petitionResults.Alojamiento.Unidades.Select(uni => new Unit()
                {
                    IdUnidad = uni.IdUnidad,
                    NombreUnidad = uni.NombreUnidad,
                    Personas = uni.Personas,
                    Description = uni.Descripcion,
                    Quota = uni.Cupos.Select(quota => new Quota()
                    {
                        Activo = quota.Activo,
                        BloqueadoPorPromo = quota.BloaqueadoPorPromo,
                        CupoMaximo = quota.Cupomaximo,
                        CupoReservado = quota.CupoReservado,
                        Fecha = quota.Fecha,
                        Fecha_Alta = quota.Fecha_Alta,
                        IdCupoUnidad = quota.IdCupoUnidad,
                        IdUnidadAloj = quota.IdUnidadAloj,
                        MarcaTemporada = quota.MarcaTemporada,
                        Monto = quota.Monto
                    }).ToArray()
                }).ToArray();
            }
            return result;
        }

        public LodgingListModel SearchHotels(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential)
        {
            var lodgings = new List<LodgingModel>();
            var results = new LodgingListModel();
            var petition = new PeticionBuscarAlojamientos();
            CompletePetition(petition, userCredential);
            if (!string.IsNullOrEmpty(searchLodgingRequestModel.DestinationId))
                petition.IdDestino = Guid.Parse(searchLodgingRequestModel.DestinationId);

            petition.TipoDestino = (TipoDestino)Enum.Parse(typeof(TipoDestino), searchLodgingRequestModel.DestinationType);

            petition.FechaInicio = searchLodgingRequestModel.Checkin;
            petition.FechaFin = searchLodgingRequestModel.Checkout;
            petition.desayuno = searchLodgingRequestModel.Breakfast;
            petition.tarifaReembolsable = searchLodgingRequestModel.Tarifa;

            if (searchLodgingRequestModel.Rooms == null)
            {
                petition.Habitacion1 = petition.Habitacion1 ?? 0 + 1;
                petition.Habitacion2 = petition.Habitacion2 ?? 0 + 1;
                petition.Habitacion3 = petition.Habitacion3 ?? 0 + 1;
                petition.Habitacion4 = petition.Habitacion4 ?? 0 + 1;
                petition.Habitacion5 = petition.Habitacion5 ?? 0 + 1;
                petition.Habitacion6 = petition.Habitacion6 ?? 0 + 1;
            }
            else
            {
                foreach (var room in searchLodgingRequestModel.Rooms)
                {
                    switch (room.RoomType)
                    {
                        case RoomType.Single:
                            petition.Habitacion1 = (petition.Habitacion1 ?? 0) + 1;
                            break;
                        case RoomType.Double:
                            petition.Habitacion2 = (petition.Habitacion2 ?? 0) + 1;
                            break;
                        case RoomType.Triple:
                            petition.Habitacion3 = (petition.Habitacion3 ?? 0) + 1;
                            break;
                        case RoomType.Quad:
                            petition.Habitacion4 = (petition.Habitacion4 ?? 0) + 1;
                            break;
                    }
                }
            }
                
            if (!string.IsNullOrEmpty(searchLodgingRequestModel.Order))
                petition.Orden = (OrdenAlojamientos)Enum.Parse(typeof(OrdenAlojamientos), searchLodgingRequestModel.Order);

            if (!string.IsNullOrEmpty(searchLodgingRequestModel.Nationality))
                petition.Nacionalidad = searchLodgingRequestModel.Nationality;

            if (!string.IsNullOrEmpty(searchLodgingRequestModel.LodgingName) && searchLodgingRequestModel.LodgingName != "{}")
                petition.NombreAlojamiento = searchLodgingRequestModel.LodgingName;

            var petitionResults = _service.SearchHotels(petition);

            if (petitionResults.Estado == EstadoRespuesta.Ok)
            {
                foreach (var lodging in petitionResults.AlojamientosDisponibles)
                {
                    if (searchLodgingRequestModel.LodgingName == "{}" || lodging.Nombre.ToLower().Contains((searchLodgingRequestModel.LodgingName ?? string.Empty).ToLower()))
                    {
                        var lodgingModel = new LodgingModel
                        {
                            LodgingId = lodging.IdAlojamiento.ToString(),
                            LodgingName = lodging.Nombre,
                            LodgingDescription = lodging.Alojamiento.Descripcion,
                            LodgingLocation = lodging.Alojamiento.Direccion,
                            LodgingCity = lodging.Destino.NombreDestino,
                            LodgingPhone = lodging.Alojamiento.Telefono,
                            LodgingPrice = Math.Round(Decimal.Parse(lodging.MontoUnidadMasBarata.ToString()), 0),
                            LodgingServices = lodging.Alojamiento.Amenidades,
                            LodgingCategory = GetCategory(lodging.Alojamiento.Categoria),
                            LodgingUnderPetition = lodging.Alojamiento.BajoPeticion,
                            LodgingCancelationPolitic = lodging.Alojamiento.PoliticasCancelacion,
                            LodgingBreakfast = searchLodgingRequestModel.Breakfast,
                            LodgingTarifa = searchLodgingRequestModel.Tarifa,
                            TienePromocion = lodging.Alojamiento.TienePromocion
                        };
                        var currency = "$";

                        switch (lodging.Moneda.GetValueOrDefault())
                        {
                            case Moneda.ARS: currency = "$"; break;
                            case Moneda.EUR: currency = "€"; break;
                            case Moneda.USD: currency = "U$S"; break;
                        }

                        lodgingModel.LodgingCurrency = currency;
                        lodgingModel.LodgingCurrencyCode = MapCurrencyToNPS(lodging.Moneda.GetValueOrDefault());
                        if (lodging.Alojamiento.Unidades != null)
                        {
                            var vacancies = new List<VacancyModel>();

                            var vacancyGroups = lodging.Alojamiento.Unidades.GroupBy(u => u.IdUnidad);

                            foreach (var group in vacancyGroups)
                            {
                                var vacancy = group.Select(v => new VacancyModel
                                {
                                    LodgingId = lodgingModel.LodgingId,
                                    LodgingName = lodgingModel.LodgingName,
                                    LodgingCurrency = lodgingModel.LodgingCurrency,
                                    VacancySelected = false,
                                    VacancyId = v.IdUnidad.ToString(),
                                    VacancyName = v.NombreUnidad,
                                    VacancyAdults = v.Personas,
                                    VacancyBeds = v.Camas,
                                    VacancyCount = v.Disponibles,
                                    VacancyPrice = Decimal.Round(v.MontoPorUnidad, 0),
                                    VacancyCheckin = searchLodgingRequestModel.Checkin,
                                    VacancyCheckout = searchLodgingRequestModel.Checkout,
                                    VacancyDates = group.Select(vg => vg.Fecha).ToList(),
                                    Breakfast = v.Desayuno,
                                    Tarifa = v.Tarifa,
                                    Available = true,
                                    ConfirmedVacancyPrice = v.MontoPorUnidad,
                                    TienePromocionNxM = v.TienePromocionNxM,
                                    TienePromocionMinimoMaximo = v.TienePromocionMinimoMaximo,
                                    MinimoNoches = v.MinimoDias,
                                    MaximoNoches = v.MaximoDias,
                                    Rooms = new List<RoomModel>{
                                        new RoomModel{
                                            RoomId = v.IdUnidad.ToString(),
                                            RoomName = v.NombreUnidad,
                                            RoomAdults = v.Personas,
                                            RoomBeds = v.Camas,
                                            RoomCount = v.Disponibles
                                        }
                                    }
                                }).First();

                                vacancies.Add(vacancy);
                            }
                            lodgingModel.Vacancies = vacancies;
                        }
                        lodgings.Add(lodgingModel);
                    }
                }
            }
            results.Lodgings = lodgings;
            return results;
        }
        private int GetCategory(CategoriaAlojamiento category)
        {
            int categoryInt = 5;

            switch (category)
            {
                case CategoriaAlojamiento.Estrellas1: categoryInt = 1; break;
                case CategoriaAlojamiento.Estrellas2: categoryInt = 2; break;
                case CategoriaAlojamiento.Estrellas3: categoryInt = 3; break;
                case CategoriaAlojamiento.Estrellas4: categoryInt = 4; break;
                case CategoriaAlojamiento.Estrellas5: categoryInt = 5; break;
                default: categoryInt = 5; break;
            }

            return categoryInt;
        }

        private decimal CalculateTotalPrice(InfoAlojamientoDisponibleCompleta lodging, int room1, int room2,
                int room3, int room4, int room5, int room6)
        {
            decimal totalPrice = Math.Min(room1, lodging.Cupo1.GetValueOrDefault()) * lodging.Tarifa1.GetValueOrDefault();
            totalPrice += Math.Min(room2, lodging.Cupo2.GetValueOrDefault()) * lodging.Tarifa2.GetValueOrDefault();
            totalPrice += Math.Min(room3, lodging.Cupo3.GetValueOrDefault()) * lodging.Tarifa3.GetValueOrDefault();
            totalPrice += Math.Min(room4, lodging.Cupo4.GetValueOrDefault()) * lodging.Tarifa4.GetValueOrDefault();
            totalPrice += Math.Min(room5, lodging.Cupo5.GetValueOrDefault()) * lodging.Tarifa5.GetValueOrDefault();
            totalPrice += Math.Min(room6, lodging.Cupo6.GetValueOrDefault()) * lodging.Tarifa6.GetValueOrDefault();

            return Decimal.Round(totalPrice, 0);
        }

        private string MapCurrencyToNPS(Moneda? currency)
        {
            string code = "N/N";

            if (currency != null)
            {
                switch (currency)
                {
                    case Moneda.ARS: code = "032"; break;
                    case Moneda.EUR: code = "978"; break;
                    case Moneda.USD: code = "840"; break;
                }
            }

            return code;
        }
    }
}