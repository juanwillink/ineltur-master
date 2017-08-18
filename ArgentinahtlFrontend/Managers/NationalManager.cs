using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using CheckArgentina.NationalService;
using CheckArgentina.Models;
using CheckArgentina.Commons;

namespace CheckArgentina.Managers
{
    public partial class Manager
    {
        protected class NationalManager : ISearchManager
        {
            private HotelesSoapSoap _service;
            private Manager _globalManager;

            public NationalManager(Manager globalManager)
            {
                _globalManager = globalManager;
                _service = new NationalService.HotelesSoapSoapClient();
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

            public DestinationListModel SearchDestination(string destinationName, Credential userCredential, DestinationModel parent = null)
            {
                SessionData.SearchType = SearchType.National;

                var results = new DestinationListModel();
                var destinations = new List<DestinationModel>();

                var petition = new PeticionBuscarDestinos();
                CompletePetition(petition, userCredential);

                petition.Destino = destinationName;

                var petitionResults = _service.BuscarDestinos(petition);

                if (petitionResults.Estado == EstadoRespuesta.Ok)
                {
                    foreach (var destination in petitionResults.Destinos)
                    {
                        if ((DestinationType)destination.TipoDestino == DestinationType.City || destination.NombreDestino.Contains("Stand By"))
                        {
                            destinations.Add(new DestinationModel()
                            {
                                DestinationId = destination.IdDestino.ToString(),
                                DestinationName = destination.NombreDestino,
                                DestinationType = (DestinationType)destination.TipoDestino
                            });
                        }    
                    }
                }

                List<DestinationModel> orderedList = destinations.OrderBy(o => o.DestinationName).ToList();
                for (int i = 0; i < orderedList.Count; i++)
                {
                    if (orderedList[i].DestinationId == "44a61c51-e583-46a5-b7b7-596441953248" || 
                        orderedList[i].DestinationId == "b82161ef-a9d7-43b1-bd5a-53f80ac8d3df" || 
                        orderedList[i].DestinationId == "8a5e4469-8df7-456b-a0e3-3f091eb5fa3d" ||
                        orderedList[i].DestinationId == "166ba3db-ef47-4ac6-960b-df7884bfbbff" ||
                        orderedList[i].DestinationId == "4023c2d8-eb8c-464e-954d-be64d06cd06f" ||
                        orderedList[i].DestinationId == "b6543d09-8a7a-479c-9c52-7b579277ef6a")
                    {
                        DestinationModel item = orderedList[i];
                        orderedList.RemoveAt(i);
                        orderedList.Insert(0, item);
                    }
                }
                results.Destinations = orderedList;
                return results;
            }

            public LodgingListModel SearchLodging(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential)
            {
                SessionData.SearchType = SearchType.National;

                var results = new LodgingListModel();
                var lodgings = new List<LodgingModel>();

                var petition = new PeticionBuscarAlojamientos();
                CompletePetition(petition, userCredential);

                if(!string.IsNullOrEmpty(searchLodgingRequestModel.DestinationId))
                    petition.IdDestino = Guid.Parse(searchLodgingRequestModel.DestinationId);

                petition.TipoDestino = (TipoDestino)Enum.Parse(typeof(TipoDestino), searchLodgingRequestModel.DestinationType);

                petition.FechaInicio = searchLodgingRequestModel.Checkin;
                petition.FechaFin = searchLodgingRequestModel.Checkout;
                foreach (var room in searchLodgingRequestModel.Rooms)
                {
                    switch (room.RoomType)
                    {
                        case RoomType.Single:
                        case RoomType.DSU:
                            petition.Habitacion1 = petition.Habitacion1 ?? 0 + room.Count;
                            break;

                        case RoomType.Double:
                        case RoomType.Twin:
                            petition.Habitacion2 = petition.Habitacion2 ?? 0 + room.Count;
                            break;

                        case RoomType.Triple:
                            petition.Habitacion3 = petition.Habitacion3 ?? 0 + room.Count;
                            break;

                        case RoomType.Quad:
                            petition.Habitacion4 = petition.Habitacion4 ?? 0 + room.Count;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(searchLodgingRequestModel.Order))
                    petition.Orden = (OrdenAlojamientos)Enum.Parse(typeof(OrdenAlojamientos), searchLodgingRequestModel.Order);
                if (!string.IsNullOrEmpty(searchLodgingRequestModel.Nationality))
                    petition.Nacionalidad = searchLodgingRequestModel.Nationality;
                if (!string.IsNullOrEmpty(searchLodgingRequestModel.LodgingName) && searchLodgingRequestModel.LodgingName != "{}")
                    petition.NombreAlojamiento = searchLodgingRequestModel.LodgingName;

                var petitionResults = _service.BuscarAlojamientosEInfo(petition);

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
                                LodgingServices = lodging.Alojamiento.Amenidades,
                                LodgingCategory = GetCategory(lodging.Alojamiento.Categoria),
                                LodgingPhoto = lodging.Alojamiento.FotoAlojamientoUrl,
                                LodgingPrice = CalculateTotalPrice(lodging, petition.Habitacion1 ?? 0, petition.Habitacion2 ?? 0,
                                        petition.Habitacion3 ?? 0, petition.Habitacion4 ?? 0, 0, 0),
                                LodgingUnderPetition = lodging.Alojamiento.BajoPeticion,
                                LodgingCancelationPolitic = lodging.Alojamiento.PoliticasCancelacion
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
                                    

                                    Available = true,
                                    ConfirmedVacancyPrice = v.MontoPorUnidad,

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

            public ReservationModel ConfirmAvailability(ReservationModel reservationModel, Credential userCredential)
            {
                foreach (var vacancy in reservationModel.Vacancies)
                {
                    vacancy.Available = true;
                    vacancy.ConfirmedVacancyPrice = vacancy.VacancyPrice;
                }

                return reservationModel;
            }

            public ReservationModel BlockVacancies(ReservationModel reservationModel, Credential userCredential)
            {
                //var credential = GetCredential(userKey);

                //var petition = new PeticionBloquearAlojamiento();

                //CompletePetition(petition, credential);
                //var unidades = new List<DetalleUnidad>();

                //foreach (var vacancy in reservationModel.Vacancies)
                //{
                //    unidades.Add(new DetalleUnidad() 
                //        {
                //            IdUnidad = Guid.Parse(vacancy.VacancyId),
                //            FechaInicio = vacancy.VacancyDates.Min(),
                //            FechaFin = vacancy.VacancyDates.Max(),
                //            Cantidad = vacancy.VacancyReserved
                //        });
                //}

                //petition.Unidades = unidades.ToArray();
                //petition.MinutosBloqueo = 20;

                //var response = _service.BloquearAlojamiento(petition);

                //if (response.Estado != EstadoRespuesta.Ok)
                //    reservationModel.ReservationStatus = ReservationStatus.Error;
                //else
                //{
                //    reservationModel.ReservationStatus = ReservationStatus.InProcess;
                //    reservationModel.ReservationId = response.CodigoReserva;
                //}
                var ticks = DateTime.Now.Ticks.ToString();
                reservationModel.ReservationId = ticks.Substring(0, Math.Min(ticks.Length, 16));

                return reservationModel;
            }

            public ReservationModel Reserve(ReservationModel reservationModel, Credential userCredential)
            {
                reservationModel.ReservationStatus = ReservationStatus.Error;

                var titular = new InfoPasajero()
                            {
                                Nombre = reservationModel.ReservationOwner.TravelerFirstName,
                                Apellido = reservationModel.ReservationOwner.TravelerLastName,
                                Email = reservationModel.ReservationOwner.TravelerEmail,
                                TipoDocumento = MapIdType(reservationModel.ReservationOwner.TravelerIdType),
                                Documento = reservationModel.ReservationOwner.TravelerId,
                                Pais = reservationModel.ReservationOwner.TravelerCountry,
                                FechaNacimiento = DateTime.Today
                            };

                //var petition = new PeticionReservarAlojamientoConObservaciones()
                //    {
                //        Titular = titular,
                //        IdAlojamiento = Guid.Parse(reservationModel.LodgingId),
                //        IdFormaPago = Guid.Parse(reservationModel.PaymentMethodId),
                //        Observaciones = reservationModel.Observations
                //    };

                var petition = new PeticionReservarAlojamiento()
                {
                    Titular = titular,
                    IdAlojamiento = Guid.Parse(reservationModel.LodgingId),
                    IdFormaPago = Guid.Parse(reservationModel.PaymentMethodId)
                };

                CompletePetition(petition, userCredential);

                var unidades = new List<DetalleUnidad>();

                foreach (var vacancy in reservationModel.Vacancies)
                {
                    foreach(var room in vacancy.Rooms)
                    {
                        unidades.Add(new DetalleUnidad()
                        {
                            IdUnidad = Guid.Parse(room.RoomId),
                            FechaInicio = vacancy.VacancyCheckin,
                            FechaFin = vacancy.VacancyCheckout,
                            Cantidad = vacancy.VacancyReserved,
                            Pasajeros = room.Travelers.Select(t => new InfoPasajero
                            {
                                Nombre = t.TravelerFirstName,
                                Apellido = t.TravelerLastName,
                                TipoDocumento = MapIdType(t.TravelerIdType),
                                Documento = t.TravelerId,
                                FechaNacimiento = Convert.ToDateTime(t.TravelerBornDate),
                                Pais = t.TravelerCountry
                            }).ToArray()
                        });
                    }
                }

                petition.Unidades = unidades.ToArray();         
                petition.Observaciones = reservationModel.Observations;
                petition.Desayuno = reservationModel.Vacancies.FirstOrDefault().Breakfast;
                petition.TarifaReembolsable = reservationModel.Vacancies.FirstOrDefault().Tarifa;
                if (reservationModel.Vacancies[0].VacancyCheckin.AddDays(-reservationModel.DiasCancelacionCargo) < DateTime.Now.Date)
                {
                    petition.IncurreGastos = true;
                }
                else
                {
                    petition.IncurreGastos = false;
                }
                petition.TienePromocion = reservationModel.TienePromocion;
                petition.PrecioPromocional = reservationModel.PromotionPrice;
                //var response = _service.ReservarAlojamientoConObservaciones(petition);
                var response = _service.ReservarAlojamiento(petition);

                if (response.Estado == EstadoRespuesta.Ok)
                {
                    reservationModel.ReservationStatus = ReservationStatus.Reserved;
                    reservationModel.ReservationId = response.IdReserva.ToString();
                    reservationModel.ReservationCode = response.CodigoReserva.ToString();
                }

                return reservationModel;
            }

            private TipoDocumento MapIdType(IdType idType)
            {
                var ret = TipoDocumento.DNI;

                switch (idType)
                { 
                    case IdType.DNI:
                    case IdType.LE:
                    case IdType.LC:
                        ret = TipoDocumento.DNI; break;
                    case IdType.CUIT:
                        ret = TipoDocumento.CUIT; break;
                    case IdType.Pasaporte:
                        ret = TipoDocumento.Pasaporte; break;
                }

                return ret;
            }

            public Dictionary<string, string> GetPaymentMethods()
            {
                using (var dc = new TurismoDataContext())
                {
                    var paymentMethods = new Dictionary<string, string>();

                    var paymentMethodsDB = dc.TipoFormaPagos
                        .Where(tfp => tfp.ACTIVO && tfp.IDTIPOFORMAPAGO == Guid.Parse("6D5424E4-493A-41AA-821A-41F370A58A86"))
                        .Join(dc.FormaPagos.Where(fp => fp.ACTIVO), tfp => tfp.IDTIPOFORMAPAGO, fp => fp.IDTIPOFORMAPAGO,
                            (tfp, fp) => new { PaymentMethodId = fp.IDFORMAPAGO.ToString(), PaymentMethodName = fp.NOMBRE });

                    return paymentMethodsDB.ToDictionary(e => e.PaymentMethodId, e => e.PaymentMethodName);
                }
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

            public LodgingModel SearchLodgingInfo(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential)
            {
                SessionData.SearchType = SearchType.National;

                LodgingModel result = new LodgingModel();

                if (searchLodgingRequestModel.Checkin == null)
                {
                    searchLodgingRequestModel.Checkin = DateTime.Now.Date;
                }
                if (searchLodgingRequestModel.Checkout == null)
                {
                    searchLodgingRequestModel.Checkout = DateTime.Now.Date;
                }
                var petition = new PeticionInfoAlojamiento();
                petition.IdAlojamiento = Guid.Parse(searchLodgingRequestModel.LodgingId);
                CompletePetition(petition, userCredential);

                var petitionResults = _service.InfoAlojamiento(petition);

                if (petitionResults.Estado == EstadoRespuesta.Ok)
                {
                    var lodging = petitionResults.Alojamiento;

                    var lodgingModel = new LodgingModel
                    {
                        LodgingId = lodging.IdAlojamiento.ToString(),
                        LodgingName = lodging.Nombre,
                        LodgingDescription = lodging.Descripcion,
                        LodgingCategory = GetCategory(lodging.Categoria),
                        LodgingPhoto = lodging.FotoAlojamientoUrl,
                        LodgingCity = lodging.Destino.NombreDestino,
                        LodgingCancelationPolitic = lodging.PoliticasCancelacion,
                        LodgingLocation = lodging.Direccion,
                        LodgingServices = lodging.Amenidades,
                        LodgingPhone = lodging.Telefono,
                        Latitud = lodging.Latitud,
                        Longitud = lodging.Longitud
                        //LodgingPrice = CalculateTotalPrice(lodging, petition.Habitacion1 ?? 0, petition.Habitacion2 ?? 0,
                        //        petition.Habitacion3 ?? 0, petition.Habitacion4 ?? 0, 0, 0)
                    };

                    var currency = "$";

                    switch (lodging.Moneda)
                    {
                        case Moneda.ARS: currency = "$"; break;
                        case Moneda.EUR: currency = "€"; break;
                        case Moneda.USD: currency = "U$S"; break;
                    }

                    lodgingModel.LodgingCurrency = currency;
                    lodgingModel.LodgingCurrencyCode = MapCurrencyToNPS(lodging.Moneda);

                    var vacancies = new List<VacancyModel>();

                    if (lodging.Unidades != null)
                    {
                        var vacancyGroups = lodging.Unidades.GroupBy(u => u.IdUnidad);

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
                                VacancyPrice = v.MontoPorUnidad,
                                VacancyCheckin = searchLodgingRequestModel.Checkin,
                                VacancyCheckout = searchLodgingRequestModel.Checkout,
                                VacancyDates = group.Select(vg => vg.Fecha).ToList(),
                                Available = true,
                                ConfirmedVacancyPrice = v.MontoPorUnidad,

                                Rooms = new List<RoomModel>{
                                new RoomModel{
                                    RoomId = v.IdUnidad.ToString(),
                                    RoomName = v.NombreUnidad,
                                    RoomAdults = v.Personas,
                                    RoomBeds = v.Camas,
                                    RoomCount = v.Disponibles,
                                }
                            }
                            }).First();

                            vacancies.Add(vacancy);
                        }
                        lodgingModel.Vacancies = vacancies;
                    }
                    result = lodgingModel;
                }

                return result;
            }

            public bool CancelReservation(string reservationCode, Credential userCredential)
            {
                var petition = new PeticionCancelarReservaAlojamiento()
                {
                    CodigoReserva = int.Parse(reservationCode)
                };
                CompletePetition(petition, userCredential);
                var respuesta = _service.CancelarReservaAlojamiento(petition);

                if (respuesta.Estado == EstadoRespuesta.Ok)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetDiasCancelacionCargo(Guid alojamientoId)
            {
                using (var dc = new TurismoDataContext())
                {
                    var alojamiento = dc.Alojamientos.SingleOrDefault(a => a.IDALOJ == alojamientoId);
                    return int.Parse(alojamiento.DIASCANCELACIONCARGO.ToString());
                }
            }
        }
    }
}