using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using Ineltur.Datos.Entidades;
using System.Configuration;

namespace Ineltur.CuentasCorrientes.Controllers
{
    public abstract class BaseController : Controller
    {
        private const string Culture = "en-US";

        protected WebServiceDataContext DataContext { get; private set; }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            CultureInfo ci = CultureInfo.GetCultureInfo(Culture);

            DataContext = new WebServiceDataContext(ConfigurationManager.ConnectionStrings["TurismoConnectionString"].ConnectionString);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            base.Initialize(requestContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (DataContext != null)
            {
                if (DataContext.Transaction != null) DataContext.Transaction.Commit();
                DataContext.SubmitChanges();
                DataContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}