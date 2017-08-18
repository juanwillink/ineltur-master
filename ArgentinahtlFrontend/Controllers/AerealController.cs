using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;
using System.Text;
using System.IO;

namespace CheckArgentina.Controllers
{
    public class AerealController : Controller
    {
        //
        // GET: /Aereal/

        public ActionResult AerealSearch()
        {
            return View();
        }

        [HttpGet]
        public void searchA(string q, string limit, string timestamp)
        {
            q = (q ?? string.Empty).Replace(" ", "^");

            Response.Write(GetHttpResponseAsString("http://apteknet.com/ineltur/searchA.php?q=" + q));
        }

        [HttpGet]
        public void search2A(string q, string limit, string timestamp)
        {
            q = (q ?? string.Empty).Replace(" ", "^");

            Response.Write(GetHttpResponseAsString("http://apteknet.com/ineltur/search2A.php?q=" + q));
        }

        private string GetHttpResponseAsString(string url)
        {
            var responseString = string.Empty;

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                var response = request.GetResponse();

                if (response.GetResponseStream().CanRead)
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            return responseString;
        }
    }
}
