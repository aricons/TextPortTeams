using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;

using RestSharp;
using RestSharp.Serializers;
using RestSharp.Authenticators;

using Newtonsoft.Json;

using TextPortCore.Data;
using TextPortCore.Models.IPData;
using TextPortCore.Helpers;
using TextPortCore.Integrations.APICallback;
using API = TextPortCore.Models.API;


namespace TextPortCore.Integrations.IPData
{
    public class IPData : IDisposable
    {
        private readonly RestClient _client;
        private string ipDataBaseUrl = $"https://api.ipdata.co/";


        public IPData()
        {
            this._client = new RestClient(ipDataBaseUrl);
        }

        public IPDataResult LookupIP(string ipAddress)
        {
            try
            {
                //_client.BaseUrl = new Uri(ipDataBaseUrl);
                RestRequest request = new RestRequest($"{ipAddress}?api-key={Constants.IPData.ApiKey}", Method.GET);
                IRestResponse<IPDataResult> response = _client.Execute<IPDataResult>(request);

                if (response != null && response.Data != null)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in IpData.LookupIP(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return new IPDataResult()
            {
                ip = "ERROR"
            };
        }

        #region "Disposal"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        #endregion
    }
}