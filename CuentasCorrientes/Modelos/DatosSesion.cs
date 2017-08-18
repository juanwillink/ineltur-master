using System;
using System.Web;
using System.Web.SessionState;

namespace Ineltur.CuentasCorrientes.Modelos
{
    public static class DatosSesion
    {
        private static HttpSessionState Sesion
        {
            get { return Contexto.Session; }
        }

        private static HttpContext Contexto
        {
            get { return HttpContext.Current; }
        }

        public static string NombreUsuario
        {
            get { return Contexto.User.Identity.Name; }
        }

        public static Guid? IdUsuario
        {
            get { return (Guid?)Sesion["IdUsuario"]; }
            set { Sesion["IdUsuario"] = value; }
        }
    }
}