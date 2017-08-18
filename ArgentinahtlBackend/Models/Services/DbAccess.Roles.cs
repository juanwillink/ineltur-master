using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArgentinahtlBackend.Models;

namespace ArgentinahtlMVC.Models.Services
{
    public static partial class DbAccess
    {
        private static PassengerModel MapPassenger(Pasajero passenger)
        {
            if (passenger != null)
            {
                return new PassengerModel()
                {
                    PassengerId = passenger.IDPASAJERO,
                    FirstName = passenger.NOMBRE,
                    LastName = passenger.APELLIDO,
                    BornDate = passenger.FECHA_NACIMIENTO ?? new DateTime(4000, 1, 1)
                };
            }

            return new PassengerModel();
        }

        public static PassengerModel GetPassenger(Guid passengerId)
        {
            using (var dc = new TurismoDataContext())
            {
                var passenger = dc.Pasajeros.Single(p => p.IDPASAJERO == passengerId);

                return MapPassenger(passenger);
            }
        }

        public static List<PassengerModel> GetPassengers()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Pasajeros.Select(p => MapPassenger(p)).ToList();
            }
        }

        private static AgencyModel MapAgency(Usuario agency)
        {
            return new AgencyModel()
            {
                AgencyId = agency.IDUSUARIO,
                FirstName = agency.NOMBRE,
                LastName = agency.APELLIDO,
                BornDate = agency.FECHA_NACIMIENTO,
                Username = agency.NOMBREUSUARIO,
                Enabled = agency.ACTIVO
            };
        }

        public static AgencyModel GetAgency(Guid agencyId)
        {
            using (var dc = new TurismoDataContext())
            {
                var agency = dc.Usuarios.Single(p => p.IDUSUARIO == agencyId);

                return MapAgency(agency);
            }
        }

        public static List<AgencyModel> GetAgencys()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Usuarios.Where(u => u.TipoUsuario.NOMBRE == "usuarioCliente").Select(a => MapAgency(a)).ToList();
            }
        }

        public static List<AgencyModel> GetAgencys(Guid userId)
        {
            using (var dc = new TurismoDataContext())
            {
                var user = dc.Usuarios.SingleOrDefault(u => u.IDUSUARIO == userId);

                if (user != null)

                    if (user.UsuarioWebs.PERFIL >= (int)UserProfile.Administrator)
                    {
                        return dc.Usuarios.Where(u => u.TipoUsuario.NOMBRE == "usuarioCliente" &&
                            u.IDUSUARIO == userId).Select(a => MapAgency(a)).ToList();
                    }
                    else
                    {
                        return new List<AgencyModel> { MapAgency(user) };
                    }
                else
                {
                    var client = dc.Clientes.Where(u => u.IDCLIENTE == userId).SingleOrDefault();
                    
                    if (client == null)
                        return new List<AgencyModel>();

                    return new List<AgencyModel> { MapAgency(user) };
                }
            }
        }

        private static LodgingModel MapLodging(Alojamiento lodging)
        {
            return new LodgingModel()
            {
                LodgingId = lodging.IDALOJ,
                FirstName = lodging.NOMBRE,
                LastName = string.Empty,
                BornDate = lodging.FECHA_ALTA,
                Username = string.Empty,
                Enabled = lodging.ACTIVO,
                Currency = (lodging.Moneda != null ? MapCurrency(lodging.Moneda) : null),
                Rooms = lodging.UnidadAlojamientos.Select(ua => MapRoom(ua)).ToList()
            };
        }

        public static LodgingModel GetLodging(Guid? lodgingId)
        {
            using (var dc = new TurismoDataContext())
            {
                var lodging = dc.Alojamientos.Single(a => a.IDALOJ == lodgingId);

                return MapLodging(lodging);
            }
        }

        public static List<LodgingModel> GetLodgings()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Alojamientos.Select(a => MapLodging(a)).ToList();
            }
        }

        public static List<LodgingModel> GetLodgings(Guid cityId)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Alojamientos.Where(a => a.IDCIUDAD == cityId && a.ACTIVO)
                    .OrderBy(a => a.NOMBRE).Select(a => MapLodging(a)).ToList();
            }
        }

        public static List<LodgingModel> SearchLodgings(string name)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Alojamientos.Where(a => a.NOMBRE.ToUpper().Contains(name.ToUpper()) && a.ACTIVO)
                    .OrderBy(a => a.NOMBRE).Select(a => MapLodging(a)).ToList();
            }
        }

        private static RoomModel MapRoom(UnidadAlojamiento room)
        {
            return new RoomModel()
            {
                RoomId = room.IDUNIDAD_ALOJ,
                RoomName = room.NOMBRE,
                RoomDescription = room.DESCRIPCION,
                RoomCost = room.MONTOPORDEFECTO,
                RoomCupo = room.CUPOPORDEFECTO,
                RoomCamas = room.CANTCAMAS,
                RoomPersonas = room.CANTPERSONAS
            };
        }

        public static RoomModel GetRoom(Guid roomId)
        {
            using (var dc = new TurismoDataContext())
            {
                var room = dc.UnidadAlojamientos.Single(a => a.IDUNIDAD_ALOJ == roomId);

                return MapRoom(room);
            }
        }

        public static List<RoomModel> GetRooms()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.UnidadAlojamientos.Select(ua => MapRoom(ua)).ToList();
            }
        }

        public static List<RoomModel> GetRooms(Guid lodgingId)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.UnidadAlojamientos.Where(ua => ua.IDALOJ == lodgingId && ua.ACTIVO == true).Select(ua => MapRoom(ua)).ToList();
            }
        }
    }
}