using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using RestSharp;
using RestSharp.Authenticators;

using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;
using TextPortCore.AppConfig;
using TextPortCore.Models.Bandwidth;

namespace TextPortCore.Integrations.Bandwidth
{
    public class Bandwidth : IDisposable
    {
        const string userId = "u-imdmn6chhceskespwwwox7a";
        const string baseUrl = @"https://api.catapult.inetwork.com/v1/";
        const string applicationId = "a-gp37trhojrbs2fgemvw5wzq";
        const string apiToken = "t-h5xlmcsiydxo5ye7l4q7why";
        const string apiSecret = "uw7ybc3i5vqatixqktw5wtrlz5gk3jnds5dbchq";
        const decimal defaultPrice = (decimal)0.0075;

        private readonly TextPortContext _context;
        private readonly RestClient _client = new RestClient(baseUrl);

        public Bandwidth(TextPortContext context)
        {
            this._context = context;
            this._client = new RestClient(baseUrl);
            this._client.Authenticator = new HttpBasicAuthenticator(apiToken, apiSecret);
        }

        public List<string> GetVirtualNumbersList(string areaCode)
        {
            List<String> numbersOut = new List<String>();

            try
            {
                RestRequest request = new RestRequest($"availableNumbers/local?areaCode={areaCode}&quantity=20", Method.GET);
                foreach (BandwidthNumber number in _client.Execute<List<BandwidthNumber>>(request).Data)
                {
                    numbersOut.Add(number.nationalNumber);
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
            return numbersOut;
        }


        //public List<string> GetVirtualNumbersListOld(string areaCode)
        //{
        //    List<String> numbersOut = new List<String>();

        //    try
        //    {
        //        var numbersJson = REST.GetData(String.Format("{0}/availableNumbers/local?areaCode={1}&quantity=20", baseUrl, HttpUtility.UrlEncode(areaCode)));

        //        List<BandwidthNumber> bwNumbersList = JsonConvert.DeserializeObject<List<BandwidthNumber>>(numbersJson);
        //        foreach (BandwidthNumber number in bwNumbersList)
        //        {
        //            numbersOut.Add(number.nationalNumber);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string foo = ex.Message;
        //    }
        //    return numbersOut;
        //}

        public bool PurchaseVirtualNumber(RegistrationData regData)
        {
            try
            {
                var request = new RestRequest($"users/{userId}/phoneNumbers/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new BandwidthAllocateNumber(regData.VirtualNumberGlobalFormat, regData.AccountId, applicationId));
                _client.Execute(request);

                //RestRequest request = new RestRequest($"/availableNumbers/local?areaCode={areaCode}&quantity=20", Method.POST);
                //_client.Execute(request);

                //foreach (BandwidthClasses number in _client.Execute<List<BandwidthClasses>>(request).Data)

                //  string url = String.Format("{0}/users/{1}/phoneNumbers/{2}", baseUrl, userId, HttpUtility.UrlEncode(String.Format("+{0}", virtualNumber)));
                //string foo = REST.PostData(url, String.Empty);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CancelVirtualNumber(string virtualNumber)
        {
            try
            {
                string url = String.Format("{0}/users/{1}/phoneNumbers/{2}", baseUrl, userId, HttpUtility.UrlEncode(String.Format("+{0}", virtualNumber)));
                //string foo = REST.Delete(url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Message ProcessBandwidthInboundMessage(BandwidthInboundMessage bwMessage)
        {
            string resultMessage = String.Empty;
            string forwardVNMessagesTo = String.Empty;
            string userName = String.Empty;

            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    Message messageIn = new Message(bwMessage);
                    DedicatedVirtualNumber dvn = da.GetVirtualNumberByNumber(messageIn.VirtualNumber, true);
                    if (dvn != null)
                    {
                        messageIn.AccountId = dvn.AccountId;
                        messageIn.VirtualNumberId = dvn.VirtualNumberId;
                    }

                    string result = "Notification received from Bandwidth on " + messageIn.TimeStamp.ToString() + "\r\n";
                    result += "API ID: " + bwMessage.applicationId + "\r\n";
                    result += "From mobile number: " + messageIn.MobileNumber + "\r\n";
                    result += "To virtual number: " + messageIn.VirtualNumber + "\r\n";
                    result += "Virtual Number ID: " + messageIn.VirtualNumberId.ToString() + "\r\n";
                    result += "Gateway Message ID: " + messageIn.GatewayMessageId + "\r\n";
                    result += "Account Id: " + messageIn.AccountId.ToString() + "\r\n";
                    result += "Message: " + messageIn.MessageText + "\r\n";

                    int messageId = da.InsertMessage(messageIn);
                    result += (messageId > 0) ? $"Message successfully added to messages table.{Environment.NewLine}" : $"Failure adding message to messages table.{Environment.NewLine}";

                    writeXMLToDisk(result, "BandwidthInboundMessage");

                    return messageIn;

                    //        // If a virtual number forwarding address is specified for the account, send the message to the forwarding number.
                    //        if (accountId != null)
                    //        {
                    //            if (accountId > 0 && !String.IsNullOrEmpty(forwardVNMessagesTo))
                    //            {
                    //                result += "Forwarding number detected.\r\n";
                    //                result += String.Format("Forwarding inbound message to {0} for account ID {1}. ", forwardVNMessagesTo, accountId);
                    //                string fromEmailAddress = String.Format("{0}@textport.com", userName);

                    //                //TextPort.Classes.Message message = new TextPort.Classes.Message("VNFORWARD", "173", forwardVNMessagesTo, fromEmailAddress, "SMS", true, messageIn.MobileTo, messageIn.Message, (int)accountId, TextPort.Classes.SourceTypes.Normal, true);
                    //                //if (message.PreSendChecksOK)
                    //                //{
                    //                //    if (TextPort.Common.SendSMSMessage(ref message))
                    //                //    {
                    //                //        message.UserMessage = "Your message was sent. Message ID: " + message.MessageID.ToString();
                    //                //        result += String.Format("Output from forwarding processing: {0}", message.ProcessingMessage);
                    //                //    }
                    //                //    else
                    //                //    {
                    //                //        result += String.Format("Sending of forward failed. Processing message: {0}. User message: {1}", message.ProcessingMessage, message.UserMessage);
                    //                //    }
                    //                //}
                    //                //else
                    //                //{
                    //                //    result += String.Format("Pre-send checks failed: {0}", message.ProcessingMessage);
                    //                //}
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        result += String.Format("Error detected. Details: {0}", ex.ToString());
                    //    }

                    //    writeXMLToDisk(result, "BandwidthInboundMessage");

                    //    return retVal;
                    //}
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public bool RouteMessageViaBandwidthDotComGateway(Message message)
        {
            try
            {
                var request = new RestRequest($"users/{userId}/messages/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new BandwidthOutboundMessage(message));
                IRestResponse response = _client.Execute(request);

                string gatewayMessageId = getMessageIdFromResponse(response);

                if (!string.IsNullOrEmpty(gatewayMessageId))
                {
                    message.GatewayMessageId = gatewayMessageId;
                    message.Price = defaultPrice;
                    message.ProcessingMessage += "Message delivered to Bandwidth gateway. ";
                    return true;
                }
                else
                {
                    message.ProcessingMessage += "Message delivery to Bandwidth gateway failed. Response processing failure. ";
                }
            }
            catch (Exception ex)
            {
                message.ProcessingMessage += "Bandwidth gateway delivery failed. Exception: " + ex.Message + ". ";
                EventLogging.WriteEventLogEntry("An error occurred in RouteMessageViaBandwidthDotComGateway. Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            return false;
        }

        private string getMessageIdFromResponse(IRestResponse response)
        {
            string messageId = string.Empty;

            try
            {
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK) // Created = HTTP 201. OK = HTTP 200
                {
                    if (response.Headers.Any(h => h.Name == "Location"))
                    {
                        string locationUrl = response.Headers.FirstOrDefault(h => h.Name == "Location").Value.ToString();
                        if (!string.IsNullOrEmpty(locationUrl))
                        {
                            messageId = locationUrl.Substring(locationUrl.LastIndexOf("/") + 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in  TextPortCore.Integrations.Bandwidth.getMessageIdFromResponse. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return messageId;
        }

        private void writeXMLToDisk(string xmlData, string filePrefix)
        {
            StreamWriter xmlFile;

            AppConfiguration config = new AppConfiguration();

            string fileName = $"{config.APILogFiles}{filePrefix}_{DateTime.Now:yyyy-MM-ddThh-mm-ss}.txt";

            try
            {
                using (xmlFile = new StreamWriter(fileName, true))
                {
                    xmlFile.WriteLine(xmlData);
                    xmlFile.Flush();
                    xmlFile.Close();
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
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