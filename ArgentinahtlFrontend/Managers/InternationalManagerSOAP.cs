using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using CheckArgentina.Models;
using CheckArgentina.Commons;
using NemoTypes;

using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using CheckArgentina.Models.Service;

namespace CheckArgentina.Managers
{
    public partial class Manager
    {
        protected class InternationalManager : ISearchManager
        {
            //private HotelsClient _service;
            private Manager _globalManager;

            public InternationalManager(Manager globalManager)
            {
                _globalManager = globalManager;

                //_service = new HotelsClient();
                //_service.ClientCredentials.UserName.UserName = "ws.test";
                //_service.ClientCredentials.UserName.Password = "123456";

                ServicePointManager.ServerCertificateValidationCallback
                    = delegate(Object obj, X509Certificate certificate, X509Chain
                        chain, SslPolicyErrors errors)
                    {
                        return (true);
                    };
            }


            #region Credentials

            public Credential GetCredential(string userKey)
            {
                var credential = new CheckArgentina.Models.Credential();
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

            public DestinationListModel SearchDestination(string destinationName, CheckArgentina.Models.Credential userCredential, DestinationModel parent = null)
            {
                SessionData.SearchType = SearchType.International;

                var results = new DestinationListModel();
                var destinations = new List<DestinationModel>();

                var rqlocations = new WSHCEV_main.DestinationListRQ();
                var x = new System.Xml.Serialization.XmlSerializer(rqlocations.GetType());
                var sw = new System.IO.StringWriter();
                var xw = new System.Xml.XmlTextWriter(sw);
                var wsRnd = new ServicioPruebaHeredado();

                try
                {
                    if (parent != null || (parent == null && (CacheData.Countries == null || CacheData.CountryListLastModification < DateTime.Today)))
                    {
                        rqlocations.Source = new WSHCEV_main.Source();
                        rqlocations.Source.PricingSurferConfiguration = new WSHCEV_main.PricingSurferConfiguration();
                        rqlocations.Source.PricingSurferConfiguration.PreferedLanguage = new WSHCEV_main.PreferedLanguage();
                        rqlocations.Source.PricingSurferConfiguration.PreferedCurrency = new WSHCEV_main.PreferedCurrency();
                        rqlocations.Source.PricingSurferConfiguration.ResponseSet = new WSHCEV_main.ResponseSet();
                        rqlocations.Source.PricingSurferConfiguration.ContractId = 10273; // 10857;
                        rqlocations.Source.PricingSurferConfiguration.PreferedLanguage.LanguageCode = "es";
                        rqlocations.Source.PricingSurferConfiguration.PreferedCurrency.CurrencyCode = "USD";
                        rqlocations.Source.PricingSurferConfiguration.ResponseSet.FirstItem = 1;
                        rqlocations.Source.PricingSurferConfiguration.ResponseSet.ItemsPerPage = 1500;

                        rqlocations.Details = new WSHCEV_main.DestinationListRQDetails();
                        rqlocations.Details.Criterion = new WSHCEV_main.DestinationListRQDetailsCriterion();

                        if (parent == null)
                        {
                            rqlocations.Details.Criterion.DestinationType = "NMO.HTL.DST.CTR";
                        }
                        else
                        {
                            switch (parent.DestinationType)
                            {
                                case DestinationType.State:
                                    rqlocations.Details.Criterion.DestinationType = "NMO.HTL.DST.CTY";  // No hay regiones, se buscan directamente las ciudades
                                    rqlocations.Details.Criterion.DestinationFilter = new WSHCEV_main.Destination() { Sequence = 1, DestinationType = "NMO.HTL.DST.CTR", DestinationCode = parent.DestinationId };
                                    break;

                                case DestinationType.Region:
                                    rqlocations.Details.Criterion.DestinationType = "NMO.HTL.DST.CTY";
                                    rqlocations.Details.Criterion.DestinationFilter = new WSHCEV_main.Destination() { Sequence = 1, DestinationType = "NMO.HTL.DST.ARE", DestinationCode = parent.DestinationId };
                                    break;

                                case DestinationType.City:
                                    rqlocations.Details.Criterion.DestinationType = "NMO.HTL.DST.CTY";
                                    rqlocations.Details.Criterion.DestinationFilter = new WSHCEV_main.Destination() { Sequence = 1, DestinationType = "NMO.HTL.DST.CTY", DestinationCode = parent.DestinationId };
                                    break;
                            }
                        }

                        xw.Formatting = System.Xml.Formatting.None;
                        x.Serialize(xw, rqlocations);

                        wsRnd.Timeout = 30000;
                        var response = wsRnd.DestinationList(sw.ToString());
                        var sr = new System.IO.StringReader(response);

                        x = new System.Xml.Serialization.XmlSerializer(typeof(WSHCEV_main.DestinationListRS));
                        var DestinationListRS = (WSHCEV_main.DestinationListRS)x.Deserialize(sr);
                        sr.Close();

                        if (DestinationListRS != null && DestinationListRS.Item is WSHCEV_main.DestinationListRSDetails &&
                            ((WSHCEV_main.DestinationListRSDetails)DestinationListRS.Item).Destinations.Destination != null)
                        {
                            var filteredDestination = ((WSHCEV_main.DestinationListRSDetails)DestinationListRS.Item).Destinations.Destination.Where(d => d.Value != "Argentina").Select(d => new DestinationModel {
                                    DestinationId = d.DestinationCode,
                                    DestinationName = d.Value,
                                    DestinationType = MapToDestinationType(d.DestinationType)
                                });

                            if (parent == null)
                            {
                                CacheData.Countries = filteredDestination.ToList();
                                CacheData.CountryListLastModification = DateTime.Now;
                            }

                            filteredDestination = filteredDestination.Where(d => d.DestinationName.PrepareNonStrictCompararison().Contains(destinationName.PrepareNonStrictCompararison()));

                            destinations = filteredDestination.ToList();
                        }
                    }
                    else
                    {
                        if (parent == null)
                            destinations = CacheData.Countries.Where(d => d.DestinationName.PrepareNonStrictCompararison().Contains(destinationName.PrepareNonStrictCompararison())).ToList();
                    }
                }
                catch (Exception ex)
                { 
                }

                results.Destinations = destinations;

                return results;
            }

            //public LodgingListModel SearchLodging(string destinationId, string destinationType, string lodgingType,
            //    DateTime checkin, DateTime checkout,
            //    int room1, int room2, int room3, int room4, int room5, int room6, string order, CheckArgentina.Models.Credential userCredential)
            //{

            public LodgingListModel SearchLodging(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential)
            {
                SessionData.SearchType = SearchType.International;

                var results = new LodgingListModel();
                var lodgings = new List<LodgingModel>();

                var rqHotelAvail = new WSHCEV_main.HotelRoomPricedInventorySearchRQ();
                var x = new System.Xml.Serialization.XmlSerializer(rqHotelAvail.GetType());
                var sw = new System.IO.StringWriter();
                var xw = new System.Xml.XmlTextWriter(sw);
                var wsRnd = new ServicioPruebaHeredado();
                
                var roomTypes = CacheData.NemoRoomTypes;

                try
                {
                    rqHotelAvail.Source = new WSHCEV_main.Source();
                    rqHotelAvail.Source.PricingSurferConfiguration = new WSHCEV_main.PricingSurferConfiguration();
                    rqHotelAvail.Source.PricingSurferConfiguration.PreferedLanguage = new WSHCEV_main.PreferedLanguage();
                    rqHotelAvail.Source.PricingSurferConfiguration.PreferedCurrency = new WSHCEV_main.PreferedCurrency();
                    rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet = new WSHCEV_main.ResponseSet();
                    rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria = new WSHCEV_main.SortingCriteria();
                    rqHotelAvail.Source.PricingSurferConfiguration.ContractId = 10273; // 10857;
                    rqHotelAvail.Source.PricingSurferConfiguration.PreferedLanguage.LanguageCode = "es";
                    rqHotelAvail.Source.PricingSurferConfiguration.PreferedCurrency.CurrencyCode = "USD";
                    rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.FirstItem = 1;
                    rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.ItemsPerPage = 10000;
                    rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria.ItemsCount = 1;
                    rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria.SortField = new WSHCEV_main.SortField[] { new WSHCEV_main.SortField{Sequence = 1, OrderBy = "ASC", SortFieldName="name"} };

                    rqHotelAvail.Details = new WSHCEV_main.HotelRoomPricedInventorySearchRQDetails();
                    rqHotelAvail.Details.Criterion = new WSHCEV_main.HotelRoomPricedInventorySearchRQDetailsCriterion();
                    rqHotelAvail.Details.Criterion.Stay = new WSHCEV_main.Stay();

                    rqHotelAvail.Details.Criterion.Availability = "NMO.HTL.AVB.ALL";

                    rqHotelAvail.Details.Criterion.Stay.DestinationDetails = new WSHCEV_main.DestinationDetails();
                    rqHotelAvail.Details.Criterion.Stay.DestinationDetails.Destination = new WSHCEV_main.Destination[] { 
                        new WSHCEV_main.Destination {
                            Sequence = 1,
                            DestinationCode = searchLodgingRequestModel.DestinationId,
                            DestinationType = "NMO.HTL.DST.CTY"
                        }
                    };

                    rqHotelAvail.Details.Criterion.Stay.DestinationDetails.ItemsCount = 1;

                    rqHotelAvail.Details.Criterion.Stay.CheckIn = searchLodgingRequestModel.Checkin;
                    rqHotelAvail.Details.Criterion.Stay.CheckOut = searchLodgingRequestModel.Checkout;

                    rqHotelAvail.Details.Criterion.Stay.Rooms = new WSHCEV_main.AvailRequestRooms();

                    uint i = 1;
                    var requestRooms = searchLodgingRequestModel.Rooms.Select(r => new WSHCEV_main.AvailRequestRoom
                    {
                        Sequence = i++,
                        RoomType = r.RoomTypeCode,
                        Guests = new WSHCEV_main.Guests
                        {
                            Guest = GetRequestGuests(r.ChildrenAges, GetAdultsByRoomType(r.RoomType)).ToArray()
                        }
                    });

                    rqHotelAvail.Details.Criterion.Stay.Rooms.Room = requestRooms.ToArray();

                    xw.Formatting = System.Xml.Formatting.None;
                    x.Serialize(xw, rqHotelAvail);

                    wsRnd.Timeout = 30000;
                    var response = wsRnd.HotelAvail(sw.ToString());
                    var sr = new StringReader(response);

                    x = new System.Xml.Serialization.XmlSerializer(typeof(WSHCEV_main.HotelRoomPricedInventorySearchRS));
                    var lodgingListRS = (WSHCEV_main.HotelRoomPricedInventorySearchRS)x.Deserialize(sr);
                    sr.Close();

                    if (lodgingListRS != null && lodgingListRS.Item is WSHCEV_main.HotelRoomPricedInventorySearchRSDetails &&
                        ((WSHCEV_main.HotelRoomPricedInventorySearchRSDetails)lodgingListRS.Item).TinyHotels != null)
                    {
                        lodgings = ((WSHCEV_main.HotelRoomPricedInventorySearchRSDetails)lodgingListRS.Item).TinyHotels
                            .Where(l => l.Rates.Count(r => r.Offer == WSHCEV_main.BooleanEnum.False) > 0).Select(l => new LodgingModel
                        {
                            LodgingId = l.Code,
                            LodgingName = l.Name,
                            LodgingDescription = "",
                            LodgingCategory = (int)l.Rating.Value,
                            LodgingCurrency = l.Rates.First().RatePrices.First().Currency, //"U$S"
                            LodgingCurrencyCode = MapCurrencyToNPS(l.Rates.First().RatePrices.First().Currency),
                            LodgingPrice = (decimal)l.Rates.First().RatePrices.First().Value,
                            LodgingSupplierId = l.SupplierID,
                            Vacancies = l.Rates.Where(r => r.Offer == WSHCEV_main.BooleanEnum.False).Select(r => new VacancyModel
                            {
                                LodgingId = l.Code,
                                LodgingName = l.Name,
                                LodgingCurrency = r.RatePrices.First().Currency,
                                VacancyId = r.RateID,
                                VacancyName = r.Rooms.First().Value + " / " + r.Boards.First().Value,
                                VacancyPrice = (decimal)(r.RatePrices.First().Value / (searchLodgingRequestModel.Checkout - searchLodgingRequestModel.Checkin).TotalDays),
                                VacancyCheckin = searchLodgingRequestModel.Checkin,
                                VacancyCheckout = searchLodgingRequestModel.Checkout,
                                VacancyDates = GetDatesBetween(searchLodgingRequestModel.Checkin, searchLodgingRequestModel.Checkout),
                                VacancyCount = 99,

                                Rooms = r.Rooms.Select(rr => new RoomModel{
                                            RoomId = rr.RoomID.ToString(),
                                            RoomName = rr.Value,
                                            RoomCount = 1
                                        }).ToList()
                            }).ToList()
                        }).ToList();
                    }
                }
                catch (Exception ex)
                {

                }

                results.Lodgings = lodgings;

                return results;
            }

            public ReservationModel ConfirmAvailability(ReservationModel reservationModel, Credential userCredential)
            {
                var wsRnd = new ServicioPruebaHeredado();

                var roomTypes = CacheData.NemoRoomTypes;

                try
                {
                    foreach (var vacancy in reservationModel.Vacancies)
                    {
                        var rqHotelAvail = new WSHCEV_main.HotelRoomPricedInventorySearchRQ();
                        var x = new System.Xml.Serialization.XmlSerializer(rqHotelAvail.GetType());
                        var sw = new System.IO.StringWriter();
                        var xw = new System.Xml.XmlTextWriter(sw);

                        rqHotelAvail.Source = new WSHCEV_main.Source();
                        rqHotelAvail.Source.PricingSurferConfiguration = new WSHCEV_main.PricingSurferConfiguration();
                        rqHotelAvail.Source.PricingSurferConfiguration.PreferedLanguage = new WSHCEV_main.PreferedLanguage();
                        rqHotelAvail.Source.PricingSurferConfiguration.PreferedCurrency = new WSHCEV_main.PreferedCurrency();
                        rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet = new WSHCEV_main.ResponseSet();
                        rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria = new WSHCEV_main.SortingCriteria();
                        rqHotelAvail.Source.PricingSurferConfiguration.ContractId = 10273; // 10857;
                        rqHotelAvail.Source.PricingSurferConfiguration.PreferedLanguage.LanguageCode = "es";
                        rqHotelAvail.Source.PricingSurferConfiguration.PreferedCurrency.CurrencyCode = "USD";
                        rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.FirstItem = 1;
                        rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.ItemsPerPage = 10000;
                        rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria.ItemsCount = 1;
                        rqHotelAvail.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria.SortField = new WSHCEV_main.SortField[] { new WSHCEV_main.SortField { Sequence = 1, OrderBy = "ASC", SortFieldName = "name" } };

                        rqHotelAvail.Details = new WSHCEV_main.HotelRoomPricedInventorySearchRQDetails();
                        rqHotelAvail.Details.Criterion = new WSHCEV_main.HotelRoomPricedInventorySearchRQDetailsCriterion();
                        rqHotelAvail.Details.Criterion.Stay = new WSHCEV_main.Stay();

                        rqHotelAvail.Details.Criterion.Availability = "NMO.HTL.AVB.ALL";

                        rqHotelAvail.Details.Criterion.Stay.DestinationDetails = new WSHCEV_main.DestinationDetails();
                        rqHotelAvail.Details.Criterion.Stay.DestinationDetails.Destination = new WSHCEV_main.Destination[] { 
                                new WSHCEV_main.Destination {
                                    Sequence = 1,
                                    DestinationCode = reservationModel.DestinationId,
                                    DestinationType = "NMO.HTL.DST.CTY"
                                }
                            };

                        rqHotelAvail.Details.Criterion.Stay.DestinationDetails.ItemsCount = 1;

                        rqHotelAvail.Details.Criterion.Stay.CheckIn = reservationModel.Vacancies.First().VacancyCheckin;
                        rqHotelAvail.Details.Criterion.Stay.CheckOut = reservationModel.Vacancies.First().VacancyCheckout;

                        rqHotelAvail.Details.Criterion.Stay.Rooms = new WSHCEV_main.AvailRequestRooms();

                        uint i = 1;
                        var requestRooms = new List<WSHCEV_main.AvailRequestRoom>();

                        foreach (var room in vacancy.Rooms)
                        {
                            requestRooms.Add(new WSHCEV_main.AvailRequestRoom
                            {
                                Sequence = i++,
                                RoomType = MapFromRoomType(room.RoomType),
                                Guests = new WSHCEV_main.Guests
                                {
                                    Guest = GetRequestGuests(room.ChildrenAges, GetAdultsByRoomType(room.RoomType)).ToArray()
                                }
                            });
                        }

                        rqHotelAvail.Details.Criterion.RateID = vacancy.VacancyId;

                        rqHotelAvail.Details.Criterion.Stay.Rooms.Room = requestRooms.ToArray();

                        rqHotelAvail.Details.Criterion.HotelCodes = new WSHCEV_main.HotelCodes
                        {
                            ItemsCount = 1,
                            HotelCode = new WSHCEV_main.HotelCode[]
                            {
                                new WSHCEV_main.HotelCode
                                {
                                    SupplierID = reservationModel.LodgingSupplierId,
                                    Value = reservationModel.LodgingId
                                }
                            },
                            OrderBy = "ASC"
                        };

                        xw.Formatting = System.Xml.Formatting.None;
                        x.Serialize(xw, rqHotelAvail);

                        wsRnd.Timeout = 30000;
                        var response = wsRnd.HotelAvail(sw.ToString());
                        var sr = new StringReader(response);

                        x = new System.Xml.Serialization.XmlSerializer(typeof(WSHCEV_main.HotelRoomPricedInventorySearchRS));
                        var lodgingListRS = (WSHCEV_main.HotelRoomPricedInventorySearchRS)x.Deserialize(sr);
                        sr.Close();

                        if (lodgingListRS != null && lodgingListRS.Item is WSHCEV_main.HotelRoomPricedInventorySearchRSDetails &&
                            ((WSHCEV_main.HotelRoomPricedInventorySearchRSDetails)lodgingListRS.Item).TinyHotels != null)
                        {
                            var lodgings = ((WSHCEV_main.HotelRoomPricedInventorySearchRSDetails)lodgingListRS.Item).TinyHotels;
                            
                            foreach (var lodging in lodgings)
                            {
                                var rate = lodging.Rates.First(r => r.RateID == vacancy.VacancyId);

                                if (rate != null)
                                {
                                    vacancy.Available = true;
                                    vacancy.ConfirmedVacancyPrice = (decimal)(rate.RatePrices.FirstOrDefault().Value / 
                                        (reservationModel.Vacancies.First().VacancyCheckout - reservationModel.Vacancies.First().VacancyCheckin).TotalDays);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                return reservationModel;
            }

            public ReservationModel BlockVacancies(ReservationModel reservationModel, CheckArgentina.Models.Credential userCredential)
            {
                var ticks = DateTime.Now.Ticks.ToString();
                reservationModel.ReservationId = ticks.Substring(0, Math.Min(ticks.Length, 16));

                return reservationModel;
            }

            public ReservationModel Reserve(ReservationModel reservationModel, CheckArgentina.Models.Credential userCredential)
            {
                var wsRnd = new ServicioPruebaHeredado();

                var roomTypes = CacheData.NemoRoomTypes;

                try
                {
                    foreach (var vacancy in reservationModel.Vacancies)
                    {
                        var rqHotelBookService = new WSHCEV_main.HotelBookServiceRQ();
                        var x = new System.Xml.Serialization.XmlSerializer(rqHotelBookService.GetType());
                        var sw = new System.IO.StringWriter();
                        var xw = new System.Xml.XmlTextWriter(sw);

                        rqHotelBookService.Source = new WSHCEV_main.Source();
                        rqHotelBookService.Source.PricingSurferConfiguration = new WSHCEV_main.PricingSurferConfiguration();
                        rqHotelBookService.Source.PricingSurferConfiguration.PreferedLanguage = new WSHCEV_main.PreferedLanguage();
                        rqHotelBookService.Source.PricingSurferConfiguration.PreferedCurrency = new WSHCEV_main.PreferedCurrency();
                        rqHotelBookService.Source.PricingSurferConfiguration.ResponseSet = new WSHCEV_main.ResponseSet();
                        rqHotelBookService.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria = new WSHCEV_main.SortingCriteria();
                        rqHotelBookService.Source.PricingSurferConfiguration.ContractId = 10273; // 10857;
                        rqHotelBookService.Source.PricingSurferConfiguration.PreferedLanguage.LanguageCode = "es";
                        rqHotelBookService.Source.PricingSurferConfiguration.PreferedCurrency.CurrencyCode = "USD";
                        rqHotelBookService.Source.PricingSurferConfiguration.ResponseSet.FirstItem = 1;
                        rqHotelBookService.Source.PricingSurferConfiguration.ResponseSet.ItemsPerPage = 10000;
                        rqHotelBookService.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria.ItemsCount = 1;
                        rqHotelBookService.Source.PricingSurferConfiguration.ResponseSet.SortingCriteria.SortField = new WSHCEV_main.SortField[] { new WSHCEV_main.SortField { Sequence = 1, OrderBy = "ASC", SortFieldName = "name" } };

                        var bookingService = new WSHCEV_main.BookingService();

                        bookingService.BookingServiceAgent = new WSHCEV_main.BookingServiceAgent
                        {
                            PersonNames = new WSHCEV_main.PersonNames
                            {
                                ItemsCount = 1,
                                PersonName = new WSHCEV_main.PersonName[]
                                { 
                                    new WSHCEV_main.PersonName
                                    {
                                        Sequence = 1,
                                        NameType = "NMO.GBL.PNT.FIR",
                                        Value = "Ineltur"
                                    }
                                }
                            },
                            Identifiers = new WSHCEV_main.Identifiers
                            {
                                ItemsCount = 1,
                                Identifier = new WSHCEV_main.Identifier[]
                                {
                                    new WSHCEV_main.Identifier
                                    {
                                        Sequence = 1,
                                        Type = "NMO.HTL.RPT.CUT",
                                        Value = "111111111111"  //TODO:Colocar el CUIT
                                    }
                                }
                            }
                        };

                        bookingService.Client = new WSHCEV_main.Client
                        {
                            Sequence = 1,
                            PersonNames = new WSHCEV_main.PersonNames
                            {
                                ItemsCount = 2,
                                PersonName = new WSHCEV_main.PersonName[]
                                {
                                    new WSHCEV_main.PersonName
                                    {
                                        Sequence = 1,
                                        NameType = "NMO.GBL.PNT.FIR",
                                        Value = reservationModel.ReservationOwner.TravelerFirstName
                                    },
                                    new WSHCEV_main.PersonName
                                    {
                                        Sequence = 2,
                                        NameType = "NMO.GBL.PNT.LAS",
                                        Value = reservationModel.ReservationOwner.TravelerLastName
                                    }
                                }
                            },
                            Identifiers = new WSHCEV_main.Identifiers
                            {
                                ItemsCount = 1,
                                Identifier = new WSHCEV_main.Identifier[]
                                {
                                    new WSHCEV_main.Identifier
                                    {
                                        Sequence = 1,
                                        Type =  MapFromIdType(reservationModel.ReservationOwner.TravelerIdType),   //TODO:Mapear el tipo de ID
                                        Value = reservationModel.ReservationOwner.TravelerId  
                                    }
                                }
                            },

                            AgeSpecified = true,
                            AgeType = "NMO.GBL.AGT.ADT",
                            Age = (uint)(DateTime.Today - DateTime.Parse(reservationModel.ReservationOwner.TravelerBornDate)).TotalDays / 365
                        };

                        bookingService.BookingServiceItems = new WSHCEV_main.BookingServiceItems
                        {
                            ItemsCount = 1,
                            BookingServiceItem = reservationModel.Vacancies.Select(v => new WSHCEV_main.BookingServiceItem
                            {
                                Sequence = 1,
                                SupplierID = reservationModel.LodgingSupplierId,
                                DestinationDetails = new WSHCEV_main.DestinationDetails
                                {
                                    ItemsCount = 1,
                                    Destination = new WSHCEV_main.Destination[]
                                    {
                                        new WSHCEV_main.Destination 
                                        {
                                            Sequence = 1,
                                            DestinationCode = reservationModel.DestinationId,
                                            DestinationType = "NMO.HTL.DST.CTY"
                                        }
                                    }
                                },
                                CheckIn = v.VacancyCheckin,
                                CheckOut = v.VacancyCheckout,
                                Rate = new WSHCEV_main.BookingServiceRate
                                {
                                    RateID = v.VacancyId,
                                    Rooms = new WSHCEV_main.BookingServiceRooms
                                    {
                                        Room = v.Rooms.Select(r => new WSHCEV_main.BookingServiceRoom
                                        {
                                            RoomDescription = new WSHCEV_main.RoomDescription
                                            {
                                                RoomType = MapFromRoomType(r.RoomType)
                                            },
                                            Occupancy = new WSHCEV_main.BookingServiceOccupancy
                                            {
                                                RoomsCount = (uint)vacancy.VacancyReserved,
                                                AdultsCount = GetAdultsByRoomType(r.RoomType),
                                                ChildrenCount = (uint)r.ChildrenAges.Count(),
                                                Guests = new WSHCEV_main.BookingServiceGuests
                                                {
                                                    Guest = GetRequestBookingGuests(r.Travelers).ToArray()
                                                }
                                            }
                                        }).ToArray()
                                    }
                                }
                            }).ToArray()
                        };

                        rqHotelBookService.Details = new WSHCEV_main.BookingService[] { bookingService };

                        xw.Formatting = System.Xml.Formatting.None;
                        x.Serialize(xw, rqHotelBookService);

                        wsRnd.Timeout = 30000;
                        var response = wsRnd.HotelAvail(sw.ToString());
                        var sr = new StringReader(response);

                        x = new System.Xml.Serialization.XmlSerializer(typeof(WSHCEV_main.HotelBookServiceRS));
                        var lodgingListRS = (WSHCEV_main.HotelBookServiceRS)x.Deserialize(sr);
                        sr.Close();

                        if (lodgingListRS != null && lodgingListRS.Item is WSHCEV_main.HotelBookServiceRSDetails &&
                            ((WSHCEV_main.HotelBookServiceRSDetails)lodgingListRS.Item).BookingService != null)
                        {
                            var responseBookingService = ((WSHCEV_main.HotelBookServiceRSDetails)lodgingListRS.Item).BookingService;

                            foreach (var book in responseBookingService)
                            {
                                //vacancy. book.BookingServiceItems.BookingServiceItem
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                return reservationModel;
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

            private DestinationType MapToDestinationType(string destinationType)
            {
                var ret = DestinationType.City;

                switch (destinationType)
                {
                    case "NMO.HTL.DST.ALL":
                    case "NMO.HTL.DST.ARE":
                    case "NMO.HTL.DST.CNT":
                    case "NMO.HTL.DST.LOC":
                        ret = DestinationType.Region; break;

                    case "NMO.HTL.DST.CTY":
                        ret = DestinationType.City; break;

                    case "NMO.HTL.DST.CTR":
                        ret = DestinationType.State; break;

                    case "NMO.HTL.DST.AIR":
                        ret = DestinationType.Airport; break;
                }

                return ret;
            }

            private string MapFromIdType(IdType idType)
            {
                string type = string.Empty;

                switch (idType)
                {
                    case IdType.DNI: type = "NMO.HTL.RPT.DNI"; break;
                    case IdType.CUIT: type = "NMO.HTL.RPT.CUT"; break;
                    case IdType.Pasaporte: type = "NMO.HTL.RPT.PAS"; break;
                    case IdType.LC: type = "NMO.HTL.RPT.DNI"; break;    // No tiene un tipo para LC y LE
                    case IdType.LE: type = "NMO.HTL.RPT.DNI"; break;
                }

                return type;
            }

            private string MapFromRoomType(CheckArgentina.Models.RoomType roomType)
            {
                string type = string.Empty;

                switch (roomType)
                {
                    case Models.RoomType.Single: type = "NMO.HTL.RMT.SGL"; break;
                    case Models.RoomType.DSU: type = "NMO.HTL.RMT.DBL.TSU"; break;
                    case Models.RoomType.Double: type = "NMO.HTL.RMT.DBL"; break;
                    case Models.RoomType.Twin: type = "NMO.HTL.RMT.DBL.TWN"; break;
                    case Models.RoomType.Triple: type = "NMO.HTL.RMT.TPL"; break;
                    case Models.RoomType.Quad: type = "NMO.HTL.RMT.QUA"; break;
                    case Models.RoomType.Quintuple: type = "NMO.HTL.RMT.PEN"; break;
                    case Models.RoomType.Sextuple: type = "NMO.HTL.RMT.HEX"; break;
                    case Models.RoomType.Septuple: type = "NMO.HTL.RMT.SEP"; break;
                    case Models.RoomType.Octuple: type = "NMO.HTL.RMT.OCT"; break;
                    case Models.RoomType.Nonuple: type = "NMO.HTL.RMT.NON"; break;
                }

                return type;
            }

            private uint GetAdultsByRoomType(CheckArgentina.Models.RoomType roomType)
            {
                uint adults = 0;

                switch (roomType)
                {
                    case CheckArgentina.Models.RoomType.Single:
                    case CheckArgentina.Models.RoomType.DSU:
                        adults = 1;
                        break;
                    case CheckArgentina.Models.RoomType.Double:
                        adults = 2;
                        break;
                    case CheckArgentina.Models.RoomType.Twin:
                        adults = 2;
                        break;
                    case CheckArgentina.Models.RoomType.Triple:
                        adults = 3;
                        break;
                    case CheckArgentina.Models.RoomType.Quad:
                        adults = 4;
                        break;
                    case CheckArgentina.Models.RoomType.Quintuple:
                        adults = 5;
                        break;
                    case CheckArgentina.Models.RoomType.Sextuple:
                        adults = 6;
                        break;
                }

                return adults;
            }

            private List<WSHCEV_main.Guest> GetRequestGuests(IEnumerable<int> childrenAges, uint adultCount)
            {
                var result = new List<WSHCEV_main.Guest>();
                uint seq = 1;

                for (; seq <= adultCount; seq++)
                {
                    result.Add(new WSHCEV_main.Guest
                    {
                        Sequence = seq,
                        AgeType = "NMO.GBL.AGT.ADT"                        
                    });
                }

                var ages = (childrenAges != null ? childrenAges.ToList() : new List<int>());

                for (; seq <= ages.Count + adultCount; seq++)
                {
                    result.Add(new WSHCEV_main.Guest
                    {
                        Sequence = seq,
                        AgeType = (ages[(int)(seq - adultCount - 1)] >= 2 ? "NMO.GBL.AGT.CHD" : "NMO.GBL.AGT.INF"),
                        Age = (uint)ages[(int)(seq - adultCount - 1)]
                    });
                }

                return result;
            }

            private List<WSHCEV_main.BookingServiceGuest> GetRequestBookingGuests(List<TravelerModel> travelers)
            {
                var result = new List<WSHCEV_main.BookingServiceGuest>();
                uint seq = 1;

                foreach (var traveler in travelers)
                {
                    result.Add(new WSHCEV_main.BookingServiceGuest
                    {
                        Sequence = seq++,

                        PersonNames = new WSHCEV_main.PersonNames
                        {
                            ItemsCount = 2,
                            PersonName = new WSHCEV_main.PersonName[]
                            {
                                new WSHCEV_main.PersonName
                                {
                                    Sequence = 1,
                                    NameType = "NMO.GBL.PNT.FIR",
                                    Value = traveler.TravelerFirstName
                                },
                                new WSHCEV_main.PersonName
                                {
                                    Sequence = 2,
                                    NameType = "NMO.GBL.PNT.LAS",
                                    Value = traveler.TravelerLastName
                                }
                            }
                        },
                        Identifiers = new WSHCEV_main.Identifiers
                        {
                            ItemsCount = 1,
                            Identifier = new WSHCEV_main.Identifier[]
                            {
                                new WSHCEV_main.Identifier
                                {
                                    Sequence = 1,
                                    Type =  MapFromIdType(traveler.TravelerIdType),   //TODO:Mapear el tipo de ID
                                    Value = traveler.TravelerId  
                                }
                            }
                        },

                        AgeSpecified = true,
                        AgeType = GetAgeType(DateTime.Parse(traveler.TravelerBornDate)),
                        Age = (uint)(DateTime.Today - DateTime.Parse(traveler.TravelerBornDate)).TotalDays / 365
                    });
                }

                return result;
            }

            private List<DateTime> GetDatesBetween(DateTime first, DateTime last)
            {
                var result = new List<DateTime>();

                while (first <= last)
                {
                    result.Add(first);
                    first = first.AddDays(1);
                }

                return result;
            }

            private string GetAgeType(DateTime bornDate)
            {
                var age = (uint)(DateTime.Today - bornDate).TotalDays / 365;

                if (age >= 18)
                    return "NMO.GBL.AGT.ADT";
                else if (age >= 3)
                    return "NMO.GBL.AGT.CHD";
                else
                    return "NMO.GBL.AGT.INF";
            }

            private string MapCurrencyToNPS(string currency)
            {
                string code = currency;

                switch (currency)
                {
                    case "ARS": code = "032"; break;
                    case "EUR": code = "978"; break;
                    case "USD": code = "840"; break;
                }

                return code;
            }
        }
    }
}