using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Authenticators;

using TextPortCore.Models;
using API = TextPortCore.Models.API;

namespace TextPortCore.Integrations.APICallback
{
    public static class CallbackProcessor
    {
        public static bool ProcessAPICallback(APIApplication apiApp, API.MessageEvent msgEvent, ref string callbackProcessingMessage)
        {
            callbackProcessingMessage = string.Empty;

            var client = new RestClient(apiApp.CallbackURL);

            if (apiApp.CallBackCredentialsRequired && !string.IsNullOrEmpty(apiApp.CallbackUserName))
            {
                client.Authenticator = new HttpBasicAuthenticator(apiApp.CallbackUserName, apiApp.CallbackPassword);
            }

            client.AddDefaultHeader("Content-type", "application/json");

            var request = new RestRequest(apiApp.CallbackURL, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(msgEvent);

            client.Execute(request);

            return true;
        }
    }
}
