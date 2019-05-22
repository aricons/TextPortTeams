using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;

using RestSharp;
using RestSharp.Serializers;
using RestSharp.Authenticators;

using Newtonsoft.Json;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.AppConfig;
using TextPortCore.Models.Bandwidth;

namespace TextPortCore.Integrations.Bandwidth
{
    public class Bandwidth : IDisposable
    {
        const string userId = "u-imdmn6chhceskespwwwox7a";
        const string accountId = "3000006";
        const string subAccountId = "23092";
        const string applicationId = "5abf6fa7-5e0f-4f1c-828b-c01f0c9674c1"; // TextPortV2
        const string userName = "richard@arionconsulting.com"; // Use userName and password when retreiving numbers
        const string password = "Zealand!4";
        const string apiToken = "091c02aae3e8dd660fc2f99a328561790da68779e83aabd1"; // Use token and secret when sending messages
        const string apiSecret = "f015bb36ce195ed94610f2e1489dc3619e47bb5c8ffc37f1";
        const decimal defaultPrice = (decimal)0.0075;
        const int orderCheckPollingCycles = 5; // number of times to check on an order
        const int orderCheckWaitTime = 1500; // milliseconds to wait between each order status check

        private string accountBaseUrl = $"https://dashboard.bandwidth.com/api/accounts/{accountId}";
        private string messageBaseUrl = $"https://messaging.bandwidth.com/api/v2/users/{accountId}";

        private readonly TextPortContext _context;
        private readonly RestClient _client;

        public Bandwidth(TextPortContext context)
        {
            this._context = context;
            this._client = new RestClient();
            this._client.Authenticator = new HttpBasicAuthenticator(apiToken, apiSecret);
        }

        public List<string> GetVirtualNumbersList(string areaCode)
        {
            List<String> numbersOut = new List<String>();

            try
            {
                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(userName, password);

                RestRequest request = new RestRequest($"/availableNumbers?areaCode={areaCode}&quantity=20", Method.GET);
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                SearchResult result = _client.Execute<SearchResult>(request).Data;
                foreach (string number in result.TelephoneNumberList)
                {
                    numbersOut.Add($"1{number}"); // Add a leading 1 to Bandwidth numbers
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
            return numbersOut;
        }

        public bool PurchaseVirtualNumber(RegistrationData regData)
        {
            try
            {
                string orderId = placeOrderForNumber(regData);

                if (!orderId.Contains("FAILED"))
                {
                    for (int x = 0; x < orderCheckPollingCycles; x++)
                    {
                        string orderStatus = string.Empty;
                        string errorMessage = string.Empty;

                        Thread.Sleep(orderCheckWaitTime);

                        orderStatus = CheckOrderStatus(orderId, ref errorMessage);

                        if (orderStatus.Equals("COMPLETE", StringComparison.CurrentCultureIgnoreCase))
                        {
                            regData.OrderingMessage += $"Order complete. Number successfully assigned to account ID {regData.AccountId}.";
                            return true;
                        }
                        else if (orderStatus.Equals("FAILED", StringComparison.CurrentCultureIgnoreCase))
                        {
                            regData.OrderingMessage += errorMessage;
                            return false;
                        }

                        // Otherwise wait and keep checking.
                        regData.OrderingMessage += $"Poll {x + 1}. ";
                    }
                }
                else
                {
                    regData.OrderingMessage = "Order placement failed.";
                }

                return false;
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.PurchaseVirtualNumber(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
        }

        private string placeOrderForNumber(RegistrationData regData)
        {
            try
            {
                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(userName, password);

                RestRequest request = new RestRequest("/orders", Method.POST)
                {
                    RequestFormat = DataFormat.Xml,
                    XmlSerializer = new DotNetXmlSerializer()
                };
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                Order ord = new Order(regData, subAccountId);
                request.AddXmlBody(ord);

                // Disable for development and testing.
                BandwidthOrderResponse result = _client.Execute<BandwidthOrderResponse>(request).Data;

                if (result != null)
                {
                    if (result.ErrorList.Count > 0)
                    {
                        if (result.ErrorList.FirstOrDefault() != null)
                        {
                            regData.OrderingMessage = $"Failure ordering number {regData.VirtualNumber}. Error: {result.ErrorList.FirstOrDefault().Description}. ";
                            return "FAILED";
                        }
                    }
                    else if (result.Order != null)
                    {
                        if (result.OrderStatus.Equals("RECEIVED", StringComparison.CurrentCultureIgnoreCase) && !String.IsNullOrEmpty(result.Order.id))
                        {
                            regData.OrderingMessage = $"Number {regData.VirtualNumber} ordered successfully. Order ID {result.Order.id}. ";
                            return result.Order.id;
                        }
                        else
                        {
                            regData.OrderingMessage = $"Failure ordering number {regData.VirtualNumber}. Order status returned was {result.OrderStatus}. Order ID {result.Order.id}. ";
                            return "FAILED";
                        }
                    }
                }

                regData.OrderingMessage = $"Failure ordering number {regData.VirtualNumber}. No error response. ";
                return "FAILED";
            }
            catch (Exception ex)
            {
                regData.OrderingMessage = $"Failure ordering number {regData.VirtualNumber}. Exception: {ex.ToString()}. ";
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.placeOrderForNumber(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return "FAILED";
            }
        }

        public string CheckOrderStatus(string bwOrderid, ref string errorDescription)
        {
            try
            {
                errorDescription = string.Empty;

                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(userName, password);

                RestRequest request = new RestRequest($"/orders/{bwOrderid}", Method.GET);
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                BandwidthOrderResponse result = _client.Execute<BandwidthOrderResponse>(request).Data;

                if (result != null)
                {
                    if (result.OrderStatus.Equals("FAILED", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (result.ErrorList != null)
                        {
                            if (result.ErrorList.FirstOrDefault() != null)
                            {
                                errorDescription = $"The order failed with response: {result.ErrorList.FirstOrDefault().Description}. ";
                            }
                        }
                    }

                    return result.OrderStatus;
                }

                return "FAILED";
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.CheckOrderStatus(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return "FAILED";
            }
        }

        public bool DisconnectVirtualNumber(DedicatedVirtualNumber number, ref string processingMessage)
        {
            try
            {
                processingMessage = string.Empty;

                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(userName, password);

                RestRequest request = new RestRequest("/disconnects", Method.POST)
                {
                    RequestFormat = DataFormat.Xml,
                    XmlSerializer = new DotNetXmlSerializer()
                };
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                DisconnectTelephoneNumberOrder ord = new DisconnectTelephoneNumberOrder(number);
                request.AddXmlBody(ord);

                DisconnectTelephoneNumberOrderResponse result = _client.Execute<DisconnectTelephoneNumberOrderResponse>(request).Data;

                if (result != null)
                {
                    if (result.ErrorList != null)
                    {
                        if (result.ErrorList.Count > 0)
                        {
                            int i = 1;
                            processingMessage += "Disconnect request failed with error(s): ";
                            foreach (Error err in result.ErrorList)
                            {
                                processingMessage += $"{i}. {err.Description}. ";
                                i++;
                            }
                            return false;
                        }
                    }

                    if (result.OrderRequest != null)
                    {
                        if (!string.IsNullOrEmpty(result.OrderRequest.id))
                        {
                            processingMessage += $"Disconnect request accepted. Request ID {result.OrderRequest.id}.";
                            return true;
                        }
                    }
                }

                processingMessage += "Disconnect request failed. ";
                return false;
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.DisconnectVirtualNumber(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
        }

        public Message ProcessInboundMessage(BandwidthInboundMessage bwMessage)
        {
            try
            {
                string resultMessage = String.Empty;
                string forwardVNMessagesTo = String.Empty;
                string userName = String.Empty;
                string jsonPayload = JsonConvert.SerializeObject(bwMessage);

                using (TextPortDA da = new TextPortDA(_context))
                {
                    Message messageIn = new Message(bwMessage);

                    DedicatedVirtualNumber dvn = da.GetVirtualNumberByNumber(messageIn.VirtualNumber, true);
                    if (dvn != null)
                    {
                        messageIn.AccountId = dvn.AccountId;
                        messageIn.VirtualNumberId = dvn.VirtualNumberId;
                    }

                    string result = "Message received from Bandwidth on " + messageIn.TimeStamp.ToString() + "\r\n";
                    result += "Notification Type: " + bwMessage.type + "\r\n";
                    result += "From mobile number: " + messageIn.MobileNumber + "\r\n";
                    result += "To virtual number: " + messageIn.VirtualNumber + "\r\n";
                    result += "Virtual Number ID: " + messageIn.VirtualNumberId.ToString() + "\r\n";
                    result += "Gateway Message ID: " + messageIn.GatewayMessageId + "\r\n";
                    result += "Account Id: " + messageIn.AccountId.ToString() + "\r\n";
                    result += "Message: " + messageIn.MessageText + "\r\n";
                    result += "Data Received: " + jsonPayload + "\r\n";

                    int messageId = da.InsertMessage(messageIn);
                    result += (messageId > 0) ? $"Message successfully added to messages table.{Environment.NewLine}" : $"Failure adding message to messages table.{Environment.NewLine}";

                    writeXMLToDisk(result, "BandwidthInboundMessage");

                    return messageIn;
                }
            }
            catch (Exception ex)
            {
                string resultErr = $"An error occurred in BandwidthCom.ProcessInboundMessage(). Message: {ex.ToString()}";
                writeXMLToDisk(resultErr, "Error_InboundMessage");

                EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
            }
            return null;
        }

        public bool ProcessDeliveryReceipt(BandwidthInboundMessage receipt)
        {
            try
            {
                string resultMessage = String.Empty;
                string forwardVNMessagesTo = String.Empty;
                string userName = String.Empty;
                string jsonPayload = JsonConvert.SerializeObject(receipt);

                using (TextPortDA da = new TextPortDA(_context))
                {
                    string resultDev = "Delivery receipt received from Bandwidth\r\n";
                    resultDev += "Notification Type: " + receipt.type + "\r\n";
                    resultDev += "To virtual number: " + receipt.to + "\r\n";
                    resultDev += "Data Received: " + jsonPayload + "\r\n";
                    writeXMLToDisk(resultDev, "BandwidthDeliveryReceipt");
                    return true;
                }
            }
            catch (Exception ex)
            {
                string resultErr = $"An error occurred in BandwidthCom.ProcessDeliveryReceipt(). Message: {ex.ToString()}";
                writeXMLToDisk(resultErr, "Error_ProcessDeliveryReceipt");

                EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
            }
            return false;
        }

        public bool ProcessDeliveryFailure(BandwidthInboundMessage receipt)
        {
            try
            {
                string resultMessage = String.Empty;
                string forwardVNMessagesTo = String.Empty;
                string userName = String.Empty;
                string jsonPayload = JsonConvert.SerializeObject(receipt);

                using (TextPortDA da = new TextPortDA(_context))
                {
                    string resultDev = "Delivery failure received from Bandwidth\r\n";
                    resultDev += "Notification Type: " + receipt.type + "\r\n";
                    resultDev += "To virtual number: " + receipt.to + "\r\n";
                    resultDev += "Data Received: " + jsonPayload + "\r\n";
                    writeXMLToDisk(resultDev, "BandwidthDeliveryFailure");
                    return true;
                }
            }
            catch (Exception ex)
            {
                string resultErr = $"An error occurred in BandwidthCom.ProcessDeliveryFailure(). Message: {ex.ToString()}";
                writeXMLToDisk(resultErr, "Error_ProcessDeliveryFailure");

                EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
            }
            return false;
        }

        public bool RouteMessageViaBandwidthDotComGateway(Message message)
        {
            try
            {
                _client.BaseUrl = new Uri(messageBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(apiToken, apiSecret);

                RestRequest request = new RestRequest("/messages", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddHeader("Content-Type", "application/json; charset=utf-8");

                BandwidthOutboundMessage bwMessage = new BandwidthOutboundMessage(message, applicationId);
                request.AddJsonBody(bwMessage);

                BandwidthMessageResponse response = _client.Execute<BandwidthMessageResponse>(request).Data;

                if (response != null)
                {
                    if (!string.IsNullOrEmpty(response.id))
                    {
                        message.GatewayMessageId = response.id;
                        message.Price = defaultPrice;
                        message.ProcessingMessage += "Message delivered to Bandwidth gateway. ";
                        return true;
                    }
                    else
                    {
                        message.ProcessingMessage += "Message delivery to Bandwidth gateway failed. Response processing failure. ";
                    }
                }
                //IRestResponse response = _client.Execute(request);

                //string gatewayMessageId = getMessageIdFromResponse(response);

                //if (!string.IsNullOrEmpty(gatewayMessageId))
                //{
                //    message.GatewayMessageId = gatewayMessageId;
                //    message.Price = defaultPrice;
                //    message.ProcessingMessage += "Message delivered to Bandwidth gateway. ";
                //    return true;
                //}
                //else
                //{
                //    message.ProcessingMessage += "Message delivery to Bandwidth gateway failed. Response processing failure. ";
                //}
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

            string baseFolder = ConfigurationManager.AppSettings["APILogFiles"];
            string fileName = $"{baseFolder}{filePrefix}_{DateTime.Now:yyyy-MM-ddThh-mm-ss}.txt";

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