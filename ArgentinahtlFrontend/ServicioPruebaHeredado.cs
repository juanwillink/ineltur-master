using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckArgentina
{
    public class ServicioPruebaHeredado : InternationalService.Hotels
    {
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.HttpWebRequest webRequest = base.GetWebRequest(uri) as System.Net.HttpWebRequest;
            Credentials = new System.Net.NetworkCredential("ws.test", "123456");
            PreAuthenticate = true;
            System.Net.NetworkCredential myCredentials = Credentials as System.Net.NetworkCredential;

            if (myCredentials != null)
            {
                string authInfo = (((myCredentials.Domain != null) && (myCredentials.Domain.Length > 0) ? myCredentials.Domain + "\\" : string.Empty) + myCredentials.UserName + " : ") + myCredentials.Password;
                authInfo = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo));
                webRequest.Headers["Authorization"] = "Basic " + authInfo;
                
                //Testing
                webRequest.Credentials = myCredentials;
                webRequest.PreAuthenticate = true;
            }

            return webRequest;
        }
    }
}