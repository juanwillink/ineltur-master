using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArgentinahtlBackend.Models;

namespace ArgentinahtlMVC.Models.Services
{
    public static partial class DbAccess
    {
        private static TransactionModel MapTransaction(Transaccion transaction)
        {
            DateTime? checkin = null;
            DateTime? checkout = null;

            if (transaction.ReservaUnidads != null && transaction.ReservaUnidads.Count != 0)
            {
                checkin = transaction.ReservaUnidads.First().FECHAINICIAL;
                checkout = transaction.ReservaUnidads.First().FECHAFINAL;
            }

            return new TransactionModel()
            {
                TransactionId = transaction.IDTRANSACCION,
                ReservationCode = transaction.CODIGO_RESERVA,
                Amount = transaction.MONTOTOTALSINDESC ?? 0,
                Status = (ReservationStatus)transaction.ESTADORESERVA,
                StatusDescription = MapReservationStatus((ReservationStatus)transaction.ESTADORESERVA),
                PaymentMethod = transaction.FormaPago != null ? transaction.FormaPago.NOMBRE : string.Empty,
                PaymentMethodId = transaction.FormaPago != null ? transaction.FormaPago.IDFORMAPAGO : Guid.Empty,
                StartDate = transaction.FECHA_ALTA,
                Checkin = checkin,
                Checkout = checkout,
                Owner = transaction.Pasajero != null ? MapPassenger(transaction.Pasajero) : new PassengerModel { FirstName = transaction.PASAJERONOMBRE, LastName = transaction.PASAJEROAPELLIDO },
                Agency = MapAgency(transaction.Usuario),
                Lodging = MapLodging(transaction.Alojamiento),
                Reservations = GetReservationsByTransactionId(transaction.IDTRANSACCION),
                Currency = MapCurrency(transaction.Moneda)
            };
        }

        public static TransactionModel GetTransaction(Guid transactionId)
        {
            using (var dc = new TurismoDataContext())
            {
                var transaction = dc.Transaccions.Single(t => t.IDTRANSACCION == transactionId);

                return MapTransaction(transaction);
            }
        }

        public static TransactionModel GetTransaction(long reservationCode)
        {
            using (var dc = new TurismoDataContext())
            {
                var transaction = dc.Transaccions.Single(t => t.CODIGO_RESERVA == reservationCode);

                return MapTransaction(transaction);
            }
        }

        public static List<TransactionModel> GetTransactions()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Transaccions.Select(u => MapTransaction(u)).ToList();
            }
        }

        public static List<TransactionModel> GetTransactions(Guid userId)
        {
            return GetTransactions(new TransactionsModel { UserId = userId });
        }

        public static List<TransactionModel> GetTransactions(TransactionsModel filters)
        {
            using (var dc = new TurismoDataContext())
            {
                if (UserDependsOf(filters.UserId, SessionData.UserId))
                {
                    if (dc.Clientes.Count(c => c.IDCLIENTE == filters.UserId) == 1)
                        return dc.Transaccions
                            .Where(t => t.Usuario.IDCLIENTE == filters.UserId &&
                                (filters.Status == -9999 || t.ESTADORESERVA == filters.Status) &&
                                ((filters.LodgingId ?? Guid.Empty) == Guid.Empty || t.IDALOJ == filters.LodgingId) &&
                                ((filters.CityId ?? Guid.Empty) == Guid.Empty || t.Alojamiento.IDCIUDAD == filters.CityId) &&
                                ((filters.ProvinceId ?? Guid.Empty) == Guid.Empty || t.Alojamiento.Ciudad.IDPROVINCIA == filters.ProvinceId))
                            .OrderByDescending(t => t.CODIGO_RESERVA)
                            .Select(t => MapTransaction(t)).ToList();
                    else
                        return dc.Transaccions
                            .Where(t => t.IDUSUARIO == filters.UserId &&
                                (filters.Status == -9999 || t.ESTADORESERVA == filters.Status) &&
                                ((filters.LodgingId ?? Guid.Empty) == Guid.Empty || t.IDALOJ == filters.LodgingId) &&
                                ((filters.CityId ?? Guid.Empty) == Guid.Empty || t.Alojamiento.IDCIUDAD == filters.CityId) &&
                                ((filters.ProvinceId ?? Guid.Empty) == Guid.Empty || t.Alojamiento.Ciudad.IDPROVINCIA == filters.ProvinceId))
                            .OrderByDescending(t => t.CODIGO_RESERVA)
                            .Select(t => MapTransaction(t)).ToList();
                }
                else
                    return new List<TransactionModel>();
            }
        }

        private static ReservationModel MapReservation(ReservaUnidad reservation)
        {
            return new ReservationModel()
            {
                TransactionId = reservation.IDTRANSACCION,
                ReservationId = reservation.IDRESERVAUNIDAD,
                Price = reservation.MONTO,
                Room = MapRoom(reservation.UnidadAlojamiento),
                Checkin = reservation.FECHAINICIAL,
                Checkout = reservation.FECHAFINAL,
                Currency = MapCurrency(reservation.Transaccion.Moneda),
                Passengers = reservation.ReservaUnidadPasajeros.Select(rup => MapPassenger(rup.Pasajero)).ToList()
            };
        }

        public static ReservationModel GetReservation(Guid reservationId)
        {
            using (var dc = new TurismoDataContext())
            {
                var reservation = dc.ReservaUnidads.Single(r => r.IDRESERVAUNIDAD == reservationId);

                return MapReservation(reservation);
            }
        }

        public static List<ReservationModel> GetReservationsByTransactionId(Guid transactionId)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.ReservaUnidads.Where(r => r.IDTRANSACCION == transactionId).Select(r => MapReservation(r)).ToList();
            }
        }

        public static List<ReservationModel> GetReservationsByReservationCode(long reservationCode)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Transaccions.Single(t => t.CODIGO_RESERVA == reservationCode).ReservaUnidads.Select(ru => MapReservation(ru)).ToList();
            }
        }

        public static string MapReservationStatus(ReservationStatus reservationStatus)
        {
            var status = string.Empty;

            switch (reservationStatus)
            {
                case ReservationStatus.ToCheck:
                    status = "A constatar";
                    break;

                case ReservationStatus.Effective:
                    status = "Efectiva";
                    break;

                case ReservationStatus.Cancelled:
                    status = "Cancelada";
                    break;

                case ReservationStatus.Refused:
                    status = "Rechazada";
                    break;

                case ReservationStatus.UnfinishedWithoutAvailabilityMarked:
                    status = "Sin finalizar - Cupos no bloqueados";
                    break;

                case ReservationStatus.UnfinishedWithAvailabilityMarked:
                    status = "Sin finalizar - Cupos bloqueados";
                    break;

                case ReservationStatus.CancellationPending:
                    status = "Pendiente de cancelación";
                    break;
            }

            return status;
        }

        public static bool UserDependsOf(Guid userId, Guid parentId)
        {
            var depends = userId == parentId || SessionData.UserProfile >= UserProfile.Administrator;

            if (!depends)
            {
                using (var dc = new TurismoDataContext())
                {
                    depends = dc.Usuarios.Count(u => u.IDUSUARIO == userId && u.IDCLIENTE == parentId) == 1;
                }
            }

            return depends;
        }
    }
}