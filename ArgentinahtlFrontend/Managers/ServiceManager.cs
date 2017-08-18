using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using CheckArgentina.Models;

namespace CheckArgentina.Managers
{
    public static class ServiceManager
    {
        public static UserModel GetUser(string userKey)
        {
            using (var dc = new TurismoDataContext())
            {
                var key = Guid.Empty;

                Guid.TryParse(userKey, out key);

                var user = dc.Usuarios.SingleOrDefault(u => u.IDUSUARIO == key);

                var userModel = new UserModel
                        {
                            UserId = Guid.Empty,
                            UserFirstName = string.Empty,
                            UserLastName = string.Empty,
                            UserLogo = string.Empty,
                            UsesPaymentInterface = false,
                            LodgingId = Guid.Empty,
                            LodgingName = string.Empty,
                            DestinationId = Guid.Empty,
                            DestinationName = string.Empty
                        };

                if (user != null && user.IDUSUARIO != null)
                {
                    userModel.UserId = key;
                    userModel.UserFirstName = user.NOMBRE;
                    userModel.UserLastName = user.APELLIDO;
                    userModel.UserLogo = user.URLLOGO;
                    userModel.UsesPaymentInterface = !user.INTERFAZPAGOPROPIA.GetValueOrDefault(false);

                    if (user.UsuarioAlojamientos.Count() > 0)
                    {
                        userModel.LodgingId = user.UsuarioAlojamientos.FirstOrDefault().IDALOJ;
                        userModel.LodgingName = user.UsuarioAlojamientos.FirstOrDefault().Alojamiento.NOMBRE;
                        userModel.DestinationId = user.UsuarioAlojamientos.FirstOrDefault().Alojamiento.IDCIUDAD;
                        userModel.DestinationName = user.UsuarioAlojamientos.FirstOrDefault().Alojamiento.Ciudad.NOMBRE;
                    }
                }

                return userModel;
            }
        }

        public static UserListModel GetUsers(string username, string userkey)
        {
            using (var dc = new TurismoDataContext())
            {
                var key = Guid.Empty;

                Guid.TryParse(userkey, out key);

                var users = dc.Usuarios.Where(p => p.ACTIVO == true && p.NOMBREUSUARIO.Contains(username) || p.NOMBRE.Contains(username)).ToList();

                List<UsersModel> usuarios = new List<UsersModel>();

                foreach (var user in users)
                {
                    UsersModel usuario = new UsersModel
                    {
                        UserUsername = user.NOMBREUSUARIO,
                        UserName = user.NOMBRE,
                        UserPass = user.CLAVE
                    };
                    usuarios.Add(usuario);
                }


                var usersModel = new UserListModel
                {
                    Users = usuarios
                };

                return usersModel;
            }
        }

        public static LodgingListModel GetHotels(string hotelName, string userkey)
        {
            using (var dc = new TurismoDataContext())
            {
                var key = Guid.Empty;

                Guid.TryParse(userkey, out key);

                var hotels = dc.Alojamientos.Where(p => p.ACTIVO == true && p.ALTACONFIRMADA == 1 && p.CODIGOBLOQUEO == 0 && p.NOMBRE.Contains(hotelName));

                List<LodgingModel> lodgings = new List<LodgingModel>();

                foreach (var hotel in hotels)
                {
                    LodgingModel lodging = new LodgingModel
                    {
                        LodgingName = hotel.NOMBRE,
                        LodgingId = hotel.IDALOJ.ToString()
                    };
                    lodgings.Add(lodging);
                }


                var lodgingsmodel = new LodgingListModel
                {
                    Lodgings = lodgings
                };

                return lodgingsmodel;
            }
        }

        public static UserListModel GetUsersWithoutClientId(string username, string userkey)
        {
            using (var dc = new TurismoDataContext())
            {
                var key = Guid.Empty;

                Guid.TryParse(userkey, out key);

                var users = dc.Usuarios.Where(p => p.IDCLIENTE == null && (p.NOMBRE.Contains(username) || p.NOMBREUSUARIO.Contains(username))).ToList();

                List<UsersModel> usuarios = new List<UsersModel>();

                foreach (var user in users)
                {
                    UsersModel usuario = new UsersModel()
                    {
                        UserName = user.NOMBRE,
                        UserPass = user.CLAVE,
                        UserUsername = user.NOMBREUSUARIO
                    };
                    usuarios.Add(usuario);
                }

                var usersModel = new UserListModel()
                {
                    Users = usuarios
                };

                return usersModel;
            }
        }

        //public static UserListModel GetClients(string username, string userkey)
        //{
        //    using (var dc = new TurismoDataContext())
        //    {
        //        var key = Guid.Empty;

        //        Guid.TryParse(userkey, out key);

        //        var users = dc.Clientes

        //        List<UsersModel> usuarios = new List<UsersModel>();

        //        foreach (var user in users)
        //        {
        //            UsersModel usuario = new UsersModel()
        //            {
        //                UserName = user.NOMBRE,
        //                UserPass = user.CLAVE,
        //                UserUsername = user.NOMBREUSUARIO
        //            };
        //            usuarios.Add(usuario);
        //        }

        //        var usersModel = new UserListModel()
        //        {
        //            Users = usuarios
        //        };

        //        return usersModel;
        //    }
        //}

        public static SmtpClient GetSmtpClient()
        {
            var smtp = new SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = Config.LeerSetting("MailUseSSL", false);
            smtp.Host = Config.LeerSetting("MailServer");
            smtp.Port = Config.LeerSetting("MailPort", 25);

            string user = Config.LeerSetting("MailUser");

            if (!String.IsNullOrEmpty(user))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Config.LeerSetting("MailUser"),
                        Config.LeerSetting("MailPassword"));
            }
            return smtp;
        }
    }
}