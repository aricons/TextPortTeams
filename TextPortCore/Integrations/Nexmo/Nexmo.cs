using System;
using System.Collections.Generic;
using System.Configuration;

using RestSharp;

using Newtonsoft.Json;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.APICallback;
using API = TextPortCore.Models.API;

namespace TextPortCore.Integrations.Nexmo
{
    public class Nexmo : IDisposable
    {
        const int orderCheckPollingCycles = 5; // number of times to check on an order
        const int orderCheckWaitTime = 1500; // milliseconds to wait between each order status check

        private string baseUrl = $"https://rest.nexmo.com";
        private string nexmoApiKey = ConfigurationManager.AppSettings["NexmoApiKey"];
        private string nexmoApiSecret = ConfigurationManager.AppSettings["NexmoApiSecret"];

        private readonly RestClient _client;

        public Nexmo()
        {
            this._client = new RestClient();
        }

        public List<string> GetVirtualNumbersList(int countryId, int numbersToReturn, int page)
        {
            string countryAlphaCode = string.Empty;

            using (TextPortDA da = new TextPortDA())
            {
                countryAlphaCode = da.GetCountryByCountryId(countryId).CountryAlphaCode;
            }

            List<string> numberList = new List<string>();

            // Don't fetch any more than 6 pages.
            if (page > 6)
            {
                numberList.Add("No more numbers");
                return numberList;
            }

            try
            {
                string url = $"{baseUrl}/number/search?api_key={nexmoApiKey}&api_secret={nexmoApiSecret}&country={countryAlphaCode}&type=mobile-lvn&features=SMS&size={numbersToReturn}&page={page}";

                RestRequest request = new RestRequest(url, Method.GET);
                request.AddHeader("Content-Type", "text/json");

                NexmoNumberSearchResult result = _client.Execute<NexmoNumberSearchResult>(request).Data;

                int resultNumber = 1;
                int startAtRecord = (numbersToReturn * (page - 1)) + 1;
                foreach (NexmoNumber number in result.numbers)
                {
                    if (resultNumber >= startAtRecord)
                    {
                        numberList.Add(number.msisdn);
                    }
                    resultNumber++;
                }

            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in Nexmo.GetVirtualNumbersList(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return numberList;
        }

        //public static bool PurchaseVirtualNumber(string countryCode, string virtualNumber)
        //{
        //    try
        //    {
        //        string nexmoUrl = "http://rest.nexmo.com/number/buy/" + nexmoKey + "/" + nexmoSecret + "/" + countryCode + "/" + virtualNumber;

        //        return (postDataWithNumericReturn(nexmoUrl) == HttpStatusCode.OK) ? true : false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //private static List<string> processNexmoNumbersResponse(string inputXml)
        //{
        //    List<string> numberList = new List<string>();

        //    try
        //    {
        //        XElement response = XElement.Parse(inputXml);

        //        IEnumerable<XElement> numbers =
        //        from number in response.Elements("numbers").Elements("msisdn").ToList()
        //        select number;

        //        foreach (XElement item in numbers)
        //        {
        //            numberList.Add(item.Value);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        numberList = new List<string>()
        //        {
        //           "No numbers available"
        //        };
        //    }
        //    return numberList;
        //}

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