using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;

using RestSharp;
using RestSharp.Serialization.Json;

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

                foreach (NexmoNumber number in result.numbers)
                {
                    numberList.Add(number.msisdn);
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in Nexmo.GetVirtualNumbersList(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return numberList;
        }

        public bool PurchaseVirtualNumber(RegistrationData regData)
        {
            try
            {
                string countryAlphaCode;
                using (TextPortDA da = new TextPortDA())
                {
                    countryAlphaCode = da.GetCountryByCountryId(regData.CountryId).CountryAlphaCode;
                }

                string url = $"{baseUrl}/number/buy/";

                RestRequest request = new RestRequest(url, Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                PurchaseNumber numberToPurchase = new PurchaseNumber(countryAlphaCode, regData.VirtualNumber);
                request.AddParameter("api_key", nexmoApiKey, ParameterType.GetOrPost);
                request.AddParameter("api_secret", nexmoApiSecret, ParameterType.GetOrPost);
                request.AddParameter("country", numberToPurchase.country, ParameterType.GetOrPost);
                request.AddParameter("msisdn", numberToPurchase.msisdn, ParameterType.GetOrPost);

                // Disable for development and testing. Uncomment return true line.
                return true;
                IRestResponse response = _client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    NexmoResponse nexResp = new JsonDeserializer().Deserialize<NexmoResponse>(response);
                    if (nexResp != null)
                    {
                        return nexResp.error_code == "200" ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                regData.OrderingMessage = $"Failure ordering number {regData.VirtualNumber}. Exception: {ex.ToString()}. ";
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.placeOrderForNumber(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            return false;
        }

        public bool RouteMessageViaNexmoGateway(Message message)
        {
            try
            {
                string url = $"{baseUrl}/sms/json";

                RestRequest request = new RestRequest(url, Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                request.AddParameter("api_key", nexmoApiKey, ParameterType.GetOrPost);
                request.AddParameter("api_secret", nexmoApiSecret, ParameterType.GetOrPost);
                request.AddParameter("from", message.DedicatedVirtualNumber.VirtualNumber);
                request.AddParameter("to", message.MobileNumber);
                request.AddParameter("text", message.MessageText);

                IRestResponse response = _client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        NexmoMessageAck ack = new JsonDeserializer().Deserialize<NexmoMessageAck>(response);
                        if (ack != null && ack.messages.Any())
                        {
                            NexmoMessageAckDetails details = ack.messages.FirstOrDefault();
                            if (!string.IsNullOrEmpty(details.message_id))
                            {
                                da.UpdateMessageWithGatewayMessageId(message.MessageId, details.message_id, 1, QueueStatuses.SentToProvider, "Message delivered to Nexmo gateway. ");
                                return true;
                            }
                        }
                        else
                        {
                            message.ProcessingMessage += "Message delivery to Bandwidth gateway failed. Response processing failure. ";
                            da.UpdateMessageWithGatewayMessageId(message.MessageId, null, 0, QueueStatuses.SendToProviderFailed, message.ProcessingMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message.ProcessingMessage += "Bandwidth gateway delivery failed. Exception: " + ex.Message + ". ";
                message.QueueStatus = (byte)QueueStatuses.InternalFailure;
                EventLogging.WriteEventLogEntry("An error occurred in RouteMessageViaBandwidthDotComGateway. Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            return false;
        }

        //public bool ProcessDeliveryReceipt(NexmoDeliveryReceipt receipt)
        //{
        //    try
        //    {
        //        string jsonPayload = JsonConvert.SerializeObject(receipt);
        //        string resultDev = string.Empty;

        //        switch (receipt.Status)
        //        {
        //            case "delivered":
        //                resultDev = "Delivery receipt received from Nexmo\r\n";
        //                resultDev += "Notification Type: " + receipt.Status + "\r\n";
        //                resultDev += "Error Code: " + receipt.ErrCode + "\r\n";
        //                resultDev += "To virtual number: " + receipt.To + "\r\n";
        //                resultDev += "Data Received: " + jsonPayload + "\r\n";
        //                resultDev += checkForAPICallback(receipt);
        //                WriteXMLToDisk(resultDev, "NexmoDeliveryReceipt");
        //                break;

        //            default:
        //                resultDev = "Other notificationreceived from Nexmo\r\n";
        //                resultDev += "Notification Type: " + receipt.Status + "\r\n";
        //                resultDev += "Error Code: " + receipt.ErrCode + "\r\n";
        //                resultDev += "To virtual number: " + receipt.To + "\r\n";
        //                resultDev += "Data Received: " + jsonPayload + "\r\n";
        //                writeXMLToDisk(resultDev, "NexmoOTHERReceipt");
        //                break;
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string resultErr = $"An error occurred in Nexmo.ProcessDeliveryReceipt(). Message: {ex.ToString()}";
        //        writeXMLToDisk(resultErr, "Error_ProcessDeliveryReceipt");

        //        EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
        //    }
        //    return false;
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