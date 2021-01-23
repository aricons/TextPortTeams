using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using TextPortCore.Helpers;

namespace TextPortTeams.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TextPortSMS" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TextPortSMS.svc or TextPortSMS.svc.cs at the Solution Explorer and start debugging.
    public class TextPortSMS : ITextPortSMS
    {
        public string Ping()
        {
            return "TextPort .NET 3.5/4.0 WCF service is up";
        }

        public string VerifyAuthentication(string userName, string password)
        {
            return WebServicesCommon.ValidateUserCredentials(userName, password);
        }

        public TextPortSMSResponses SendMessages(TextPortSMSMessages messagesList)
        {
            return WebServicesCommon.ProcessMessageRequests(messagesList, MessageTypes.SVC);
        }

        public int GetCreditBalance(string userName, string password)
        {
            return WebServicesCommon.GetCreditBalance(userName, password);
        }
    }
}
