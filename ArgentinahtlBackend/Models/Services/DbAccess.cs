using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data.Odbc;
using ArgentinahtlBackend.Models;
//using IBM.Data.DB2;

namespace ArgentinahtlMVC.Models.Services
{
    public static partial class DbAccess
    {
        #region Helpers

        //private static OdbcConnection GetConnection()
        //{
        //    var conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["DB2"].ConnectionString);

        //    conn.Open();
        //    return conn;
        //}

        /*private static DB2Connection GetConnection()
        {
            var conn = new DB2Connection(ConfigurationManager.ConnectionStrings["DB2"].ConnectionString);

            conn.Open();
            return conn;
        }

        private static DB2Transaction BeginTransaction()
        {
            return GetConnection().BeginTransaction();
        }

        private static void CommitTransaction(DB2Transaction transaction)
        {
            if (transaction != null && transaction.Connection != null)
            {
                if (transaction.Connection.State == ConnectionState.Open)
                {
                    transaction.Commit();

                    if (transaction.Connection != null)
                        transaction.Connection.Dispose();
                }
            }
        }

        private static void RollbackTransaction(DB2Transaction transaction)
        {
            if (transaction != null && transaction.Connection != null)
            {
                if (transaction.Connection.State == ConnectionState.Open)
                {
                    transaction.Rollback();

                    if (transaction.Connection != null)
                        transaction.Connection.Dispose();
                }
            }
        }

        private static List<T> Load<T>(string query, Func<IDataRecord, T> loader, DB2Transaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = transaction.Connection;
                var cmd = conn.CreateCommand();

                if (transaction != null)
                    cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                var list = new List<T>();

                var attempts = 3;
                while (attempts > 0)
                {
                    try
                    {
                        attempts--;
                        using (var rs = cmd.ExecuteReader())
                        {
                            while (rs.Read())
                                list.Add(loader(rs));
                        }
                        attempts = 0;
                    }
                    catch (Exception)
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                    }
                }

                return list;
            }
            else
            {
                using (var conn = (transaction != null ? transaction.Connection : DbAccess.GetConnection()))
                {
                    var cmd = conn.CreateCommand();

                    if (transaction != null)
                        cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;

                    var list = new List<T>();

                    var attempts = 3;
                    while (attempts > 0)
                    {
                        try
                        {
                            attempts--;
                            using (var rs = cmd.ExecuteReader())
                            {
                                while (rs.Read())
                                    list.Add(loader(rs));
                            }
                            attempts = 0;
                        }
                        catch (Exception)
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();
                        }
                    }

                    return list;
                }
            }
        }

        private static T LoadOne<T>(string query, Func<IDataRecord, T> loader, DB2Transaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = transaction.Connection;
                var cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                var attempts = 3;
                while (attempts > 0)
                {
                    try
                    {
                        attempts--;
                        using (var rs = cmd.ExecuteReader())
                        {
                            if (rs.Read()) return loader(rs);
                        }
                        attempts = 0;
                    }
                    catch (Exception)
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                    }
                }
            }
            else
            {
                using (var conn = DbAccess.GetConnection())
                {
                    var cmd = conn.CreateCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;

                    var attempts = 3;
                    while (attempts > 0)
                    {
                        try
                        {
                            attempts--;
                            using (var rs = cmd.ExecuteReader())
                            {
                                if (rs.Read()) return loader(rs);
                            }
                            attempts = 0;
                        }
                        catch (Exception ex)
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();
                        }
                    }
                }
            }

            return default(T);
        }

        private static DataTable LoadTable(string query, DB2Transaction transaction = null)
        {
            var table = new DataTable();

            if (transaction != null)
            {
                var conn = transaction.Connection;

                var cmd = conn.CreateCommand();

                if (transaction != null)
                    cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                var attempts = 3;
                while (attempts > 0)
                {
                    try
                    {
                        attempts--;
                        using (var adapter = new IBM.Data.DB2.DB2DataAdapter(cmd))
                        {
                            adapter.Fill(table);
                        }
                        attempts = 0;
                    }
                    catch (Exception)
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                    }
                }

                return table;
            }
            else
            {
                using (var conn = DbAccess.GetConnection())
                {
                    var cmd = conn.CreateCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;

                    var attempts = 3;
                    while (attempts > 0)
                    {
                        try
                        {
                            attempts--;
                            using (var adapter = new IBM.Data.DB2.DB2DataAdapter(cmd))
                            {
                                adapter.Fill(table);
                            }
                            attempts = 0;
                        }
                        catch (Exception)
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();
                        }
                    }

                    return table;
                }
            }
        }

        private static bool InsertUpdate(string query, DB2Transaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = transaction.Connection;

                var cmd = conn.CreateCommand();

                if (transaction != null)
                    cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                try
                {
                    return cmd.ExecuteNonQuery() != 0;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                using (var conn = DbAccess.GetConnection())
                {
                    var cmd = conn.CreateCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;

                    try
                    {
                        return cmd.ExecuteNonQuery() != 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }*/

        private static string Escape(string str)
        {
            return str != null ? String.Concat("'", str.Replace("'", "''"), "'") : "NULL";
        }

        private static string MapPinStatus(string pinStatus)
        {
            switch (pinStatus)
            {
                case "OK": return "OK";
                case "USADO": return "USED";
                case "INACTIVO": return "DEACTIVATED";
            }
            return String.Empty;
        }

        private static string FormatDate(DateTime date)
        {
            var newDate = date + DateTimeOffset.Now.Offset;

            return String.Concat("'", newDate.ToString("yyyy'-'MM'-'dd HH':'mm':'ss'.'ffffff"), "'");
        }

        private static string AsString(object obj)
        {
            string str = Convert.ToString(obj);

            return String.IsNullOrWhiteSpace(str) ? null : str;
        }

        private static bool AsBool(object obj)
        {
            return Convert.ToBoolean(obj);
        }

        private static DateTime AsDateTime(object obj)
        {
            DateTime dt = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);

            return dt - DateTimeOffset.Now.Offset;
        }

        private static TimeSpan AsTimeSpan(object obj)
        {
            TimeSpan ts = TimeSpan.Parse(Convert.ToString(obj, CultureInfo.InvariantCulture));

            return ts;
        }

        private static int AsInt(object obj)
        {
            int i = Convert.ToInt32(obj, CultureInfo.InvariantCulture);

            return i;
        }

        private static long AsLong(object obj)
        {
            long i = Convert.ToInt64(obj, CultureInfo.InvariantCulture);

            return i;
        }

        private static decimal AsDecimal(object obj)
        {
            decimal d = Convert.ToDecimal(obj, CultureInfo.InvariantCulture);

            return d;
        }

        private static double OrderTimeZones(TimeZoneInfo timeZone)
        {
            return timeZone.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
        }

        private static TimeZoneInfo GetTimeZoneByCorrectName(string correctName)
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            TimeZoneInfo timeZoneInfoSelected = TimeZoneInfo.Utc;

            foreach (TimeZoneInfo timeZoneInfo in timeZones)
            {
                if (timeZoneInfo.GetCorrectName() == correctName)
                {
                    timeZoneInfoSelected = timeZoneInfo;
                    break;
                }
            }

            return timeZoneInfoSelected;
        }

        private static long GetLong(IDataRecord dr)
        {
            if (dr[0] is long || dr[0] is int)
                return Convert.ToInt64(dr[0]);

            return 0;
        }

        #endregion

        #region Queries

        #endregion

        #region User management

        private static UserModel MapUser(Usuario user)
        {
            return new UserModel()
            {
                UserId = user.IDUSUARIO,
                UserDescription = user.NOMBRE + " " + user.APELLIDO,
                UserName = user.NOMBREUSUARIO,
                Password = user.CLAVE,
                Profile = (user.UsuarioWebs != null ? (UserProfile)user.UsuarioWebs.PERFIL : UserProfile.Operator),
                Enabled = user.ACTIVO,
                Email = user.EMAIL
            };
        }

        private static UserModel MapUser(Cliente user)
        {
            return new UserModel()
            {
                UserId = user.IDCLIENTE,
                UserDescription = user.NOMBRE + " " + user.APELLIDO,
                UserName = user.NOMBREUSUARIO,
                Password = user.CLAVE,
                Profile = (user.UsuarioWebs != null ? (UserProfile)user.UsuarioWebs.PERFIL : UserProfile.AdministratorClient),
                Enabled = user.ACTIVO,
                Email = user.EMAIL
            };
        }

        public static UserModel GetUser(string userName, string password)
        {
            string passwordHash = AccountValidation.ComputePasswordHash(password);

            using (var dc = new TurismoDataContext())
            {
                var user = dc.Usuarios.Where(u => u.NOMBREUSUARIO == userName).AsEnumerable()
                    .SingleOrDefault(u => AccountValidation.ComputePasswordHash(u.CLAVE) == passwordHash);

                if (user != null)
                    return MapUser(user);   
                else
                {
                    var client = dc.Clientes.Where(u => u.NOMBREUSUARIO == userName).AsEnumerable()
                        .SingleOrDefault(u => AccountValidation.ComputePasswordHash(u.CLAVE) == passwordHash);

                    if (client == null)
                        return null;

                    return MapUser(client);
                }
            }
        }

        public static List<UserModel> GetUsers()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Usuarios.Select(u => MapUser(u)).ToList();
            }
        }

        public static bool CheckUsedEmail(string email)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Usuarios.Where(u => u.EMAIL == email).Count() == 1;
            }
        }

        public static bool CheckUsedUserName(string userName)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Usuarios.Where(u => u.NOMBREUSUARIO == userName).Count() == 1;
            }
        }

        public static bool CreateUser(string userName, string userDescription, string password, string email, UserProfile profile, int clientCode)
        {
            // No se permite la creación de usuarios desde esta aplicación
            return false;
        }

        public static bool ChangeUserPassword(string userName, string oldPassword, string newPassword)
        {
            // No se permite el cambio de password desde esta aplicación
            return false;
        }

        public static bool ChangeUserStatus(string userName, bool status, UserProfile profile)
        {
            using (var dc = new TurismoDataContext())
            {
                var user = dc.Usuarios.Single(u => u.NOMBREUSUARIO == userName);
                user.ACTIVO = status;

                dc.SubmitChanges();

                return true;
            }
        }

        public static bool ResetUserPassword(string userName, UserProfile profile)
        {
            // No se permite el cambio de password desde esta aplicación
            return false;
        }

        public static bool DeleteUser(string userName)
        {
            // No se permite eliminar un usuario desde esta aplicación
            return false;
        }

        #endregion

        #region Security

        private static UserPermissionModel MapUserPermission(IDataRecord dr)
        {
            return new UserPermissionModel()
            {
                Username = AsString(dr[0])
            };
        }

        private static ObjectPermissionModel MapObjectPermission(IDataRecord dr)
        {
            return new ObjectPermissionModel()
            {
                ObjectCode = AsLong(dr[0]),
                Description = AsString(dr[1]),
                Type = (ObjectType)AsInt(dr[2]),
                Parent = AsLong(dr[3])
            };
        }

        private static Permission MapPermission(IDataRecord dr)
        {
            return (Permission)Enum.Parse(typeof(Permission), AsString(dr[0]));
        }

        public static UserPermissionModel GetUserPermission(string userName)
        {
//            var userPermissionModel = LoadOne(String.Format(@"SELECT
//    U.USERNAME
//FROM DBO.VSEC_USER U
//WHERE U.USERNAME = {0}", Escape(userName)), MapUserPermission);

//            var dictionary = new Dictionary<long, ObjectPermissionModel>();
            
//            foreach(ObjectPermissionModel model in GetObjectPermission(userName))
//                dictionary.Add(model.ObjectCode, model);

//            userPermissionModel.Objects = dictionary;

//            return userPermissionModel;
            return new UserPermissionModel
            {
                Username = userName,
                Objects = new Dictionary<long,ObjectPermissionModel>()
            };
        }

        public static List<ObjectPermissionModel> GetObjectPermission(string userName)
        {
//            var objectPermissionModelList = Load(String.Format(@"SELECT
//    UP.OBJECTCODE, 
//    O.OBJECTDESCRIPTION,
//    O.TYPECODE,
//    COALESCE(O.PARENTOBJECTCODE, -1)
//FROM DBO.VSEC_USERPERMISSION UP INNER JOIN 
//    DBO.VSEC_OBJECT O ON O.OBJECTCODE = UP.OBJECTCODE 
//WHERE USERNAME = {0}
//UNION
//SELECT DISTINCT 
//    GP.OBJECTCODE, 
//    O.OBJECTDESCRIPTION,
//    O.TYPECODE,
//    COALESCE(O.PARENTOBJECTCODE, -1)
//FROM DBO.VSEC_GROUPPERMISSION GP INNER JOIN 
//    DBO.VSEC_USERGROUP UG ON GP.GROUPCODE = UG.GROUPCODE INNER JOIN 
//    DBO.VSEC_OBJECT O ON O.OBJECTCODE = GP.OBJECTCODE 
//WHERE UG.USERNAME = {0}", Escape(userName)), MapObjectPermission);

//            foreach (ObjectPermissionModel objectPermissionModel in objectPermissionModelList)
//            {
//                objectPermissionModel.Permissions = GetPermissions(userName, objectPermissionModel.ObjectCode);
//            }

//            return objectPermissionModelList.Distinct().ToList();
            return new List<ObjectPermissionModel>();
        }

        public static List<Permission> GetPermissions(string userName, long objectCode)
        {
//            var permissionList = Load(String.Format(@"SELECT DISTINCT
//    UP.PERMISSIONCODE
//FROM DBO.VSEC_USERPERMISSION UP 
//WHERE UP.USERNAME = {0} AND UP.OBJECTCODE = {1}
//UNION
//SELECT DISTINCT 
//    GP.PERMISSIONCODE
//FROM DBO.VSEC_GROUPPERMISSION GP INNER JOIN 
//    DBO.VSEC_USERGROUP UG ON GP.GROUPCODE = UG.GROUPCODE
//WHERE UG.USERNAME = {0} AND GP.OBJECTCODE = {1}", Escape(userName), objectCode), MapPermission);

//            return permissionList;
            return new List<Permission>();
        }

        #endregion

        private static CurrencyModel MapCurrency(Moneda currency)
        {
            return new CurrencyModel()
            {
                CurrencyId = currency.IDMONEDA,
                CurrencyName = currency.NOMBRE,
                CurrencySymbol = currency.SIMBOLO,
                CurrencyValue = currency.COTIZACION
            };
        }

        public static CurrencyModel GetCurrency(Guid currencyId)
        {
            using (var dc = new TurismoDataContext())
            {
                var currency = dc.Monedas.Single(m => m.IDMONEDA == currencyId);

                return MapCurrency(currency);
            }
        }

        public static List<CurrencyModel> GetCurrencys()
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Monedas.Select(m => MapCurrency(m)).ToList();
            }
        }

        private static DestinationModel MapDestination(Provincia province, bool propagate = false)
        {
            return new DestinationModel()
            {
                DestinationId = province.IDPROVINCIA,
                DestinationName = province.NOMBRE,
                Description = province.DESCRIPCION
            };
        }

        private static DestinationModel MapDestination(Ciudad city, bool propagate = false)
        {
            return new DestinationModel()
            {
                DestinationId = city.IDCIUDAD,
                DestinationName = city.NOMBRE,
                Description = city.ABREVIATURA,
                Lodgings = (propagate ? GetLodgings(city.IDCIUDAD) : new List<LodgingModel>())
            };
        }

        public static DestinationModel GetDestination(Guid destinationId)
        {
            using (var dc = new TurismoDataContext())
            {
                var province = dc.Provincias.Single(m => m.IDPROVINCIA == destinationId);

                if (province != null)
                    return MapDestination(province);
                else
                {
                    var city = dc.Ciudads.Single(m => m.IDCIUDAD == destinationId);

                    if (city != null)
                        return MapDestination(city);
                    else
                        return null;
                }
            }
        }

        public static List<DestinationModel> GetProvinces(bool propagate = false)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Provincias.Where(p => p.ACTIVO)
                    .OrderBy(p => p.NOMBRE).Select(m => MapDestination(m, propagate)).ToList();
            }
        }

        public static List<DestinationModel> GetCities(Guid provinceId, bool propagate = false)
        {
            using (var dc = new TurismoDataContext())
            {
                return dc.Ciudads.Where(c => c.IDPROVINCIA == provinceId && c.ACTIVO)
                    .OrderBy(c => c.NOMBRE).Select(m => MapDestination(m, propagate)).ToList();
            }
        }
    }
}