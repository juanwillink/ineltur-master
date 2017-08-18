using System;
using System.Linq;
using CheckArgentina.Models;
using CheckArgentina.Commons;
using System.Collections.Generic;

namespace CheckArgentina.Managers
{
    public enum SearchType { International, National, Undefined }

    public partial class Manager
    {
        protected interface ISearchManager
        {
            Credential GetCredential(string userKey);

            DestinationListModel SearchDestination(string destinationName, Credential userCredential, DestinationModel parent = null);

            //LodgingListModel SearchLodging(string destinationId, string destinationType, string lodging,
            //    DateTime checkin, DateTime checkout,
            //    int room1, int room2, int room3, int room4, int room5, int room6, string order, Credential userCredential);

            LodgingListModel SearchLodging(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential);

            LodgingModel SearchLodgingInfo(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential);

            ReservationModel ConfirmAvailability(ReservationModel reservationModel, Credential userCredential);

            ReservationModel BlockVacancies(ReservationModel reservationModel, Credential userCredential);

            ReservationModel Reserve(ReservationModel reservationModel, Credential userCredential);

            int GetDiasCancelacionCargo(Guid alojamientoId);


            Dictionary<string, string> GetPaymentMethods();

            bool CancelReservation(string reservationCode, Credential userCredential);
        }

        private SearchType _searchType;
        private ISearchManager _searchManager;

        #region Properties
        public SearchType Type { get { return _searchType; } }
        #endregion

        #region Constructor
        public Manager(SearchType searchType)
        {
            _searchType = searchType;

            switch (_searchType)
            {
                case SearchType.International:
                    //_searchManager = new InternationalManager(this);
                    break;

                case SearchType.National:
                    _searchManager = new NationalManager(this);
                    break;
            }
        }
        #endregion

        #region Methods
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

        public DestinationListModel SearchDestination(string destinationName, Credential userCredential, DestinationModel parent = null)
        {
            return _searchManager.SearchDestination(destinationName, userCredential, parent);
        }

        //public LodgingListModel SearchLodging(string destinationId, string destinationType, string lodgingName,
        //        DateTime checkin, DateTime checkout,
        //        int room1, int room2, int room3, int room4, int room5, int room6, string order, Credential userCredential)
        //{
        //    return _searchManager.SearchLodging(destinationId, destinationType, lodgingName,
        //        checkin, checkout,
        //        room1, room2, room3, room4, room5, room6, order, userCredential);
        //}

        public LodgingListModel SearchLodging(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential)
        {
            return _searchManager.SearchLodging(searchLodgingRequestModel, userCredential);
        }

        public ReservationModel ConfirmAvailability(ReservationModel reservationModel, Credential userCredential)
        {
            return _searchManager.ConfirmAvailability(reservationModel, userCredential);
        }

        public ReservationModel BlockVacancies(ReservationModel reservationModel, Credential userCredential)
        {
            return _searchManager.BlockVacancies(reservationModel, userCredential);
        }

        public int GetDiasCancelacionCargo(Guid alojamientoId)
        {
            return _searchManager.GetDiasCancelacionCargo(alojamientoId);
        }
        public ReservationModel Reserve(ReservationModel reservationModel, Credential userCredential)
        {
            return _searchManager.Reserve(reservationModel, userCredential);
        }

        public Dictionary<string, string> GetPaymentMethods()
        {
            return _searchManager.GetPaymentMethods();
        }

        public bool CancelReservation(string reservationCode, Credential userCredential)
        {
            return _searchManager.CancelReservation(reservationCode, userCredential);
        }

        public LodgingModel SearchLodgingInfo(SearchLodgingRequestModel searchLodgingRequestModel, Credential userCredential)
        {
            return _searchManager.SearchLodgingInfo(searchLodgingRequestModel, userCredential);
        }
        #endregion
    }
}