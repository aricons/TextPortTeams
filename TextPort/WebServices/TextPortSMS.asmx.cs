using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using TextPortCore.Helpers;

namespace TextPort.WebServices
{
    /// <summary>
    /// Summary description for TextPortSMSASMX
    /// </summary>
    [WebService(Namespace = "http://www.textport.com/WebServices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SMSClient : System.Web.Services.WebService
    {

        [WebMethod]
        public string Ping()
        {
            return "TextPort .NET 2.0 web service is up";
        }

        [WebMethod]
        public string VerifyAuthentication(string userName, string password)
        {
            return WebServicesCommon.ValidateUserCredentials(userName, password);
        }

        [WebMethod]
        public TextPortSMSResponses SendMessages(TextPortSMSMessages messagesList)
        {
            return WebServicesCommon.ProcessMessageRequests(messagesList, MessageTypes.ASMX);
        }

        [WebMethod]
        public int GetCreditBalance(string userName, string password)
        {
            return WebServicesCommon.GetCreditBalance(userName, password);
        }
    }
}
