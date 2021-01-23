using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

using TextPortCore.Integrations.PayPal;

namespace TextPortTeams.Controllers
{
    public class PayPalController : Controller
    {
        [HttpPost]
        public HttpStatusCodeResult IPNService()
        {
            //Store the IPN received from PayPal
            LogRequest(Request);

            VerifyTask(Request);

            //Reply back a 200 code
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void VerifyTask(HttpRequestBase ipnRequest)
        {
            var verificationResponse = string.Empty;

            try
            {
                var verificationRequest = (HttpWebRequest)WebRequest.Create("https://www.paypal.com/cgi-bin/webscr");
                // https://www.paypal.com/cgi-bin/webscr -- Production
                // https://www.sandbox.paypal.com/cgi-bin/webscr - Development

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                var param = Request.BinaryRead(ipnRequest.ContentLength);
                var strRequest = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                strRequest = "cmd=_notify-validate&" + strRequest;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                var streamOut = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();

                //Send the request to PayPal and get the response
                var streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream());
                verificationResponse = streamIn.ReadToEnd();
                streamIn.Close();

            }
            catch (Exception ex)
            {
            }

            //ProcessVerificationResponse(verificationResponse);
        }

        private void LogRequest(HttpRequestBase request)
        {
            byte[] param = request.BinaryRead(request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            if (strRequest.Length > 0)
            {
                System.Collections.Specialized.NameValueCollection qs = HttpUtility.ParseQueryString(strRequest);
                // Persist the request values into a database or temporary data store
                IPN ipn = new IPN();
                bool foo = ipn.ParsePayPalIPN(qs);
            }

        }
    }
}
