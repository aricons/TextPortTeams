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
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.AppConfig;

namespace TextPortCore.Integrations.Bandwidth
{
    public class Bandwidth : IDisposable
    {
        const int orderCheckPollingCycles = 5; // number of times to check on an order
        const int orderCheckWaitTime = 1500; // milliseconds to wait between each order status check

        private string accountBaseUrl = $"https://dashboard.bandwidth.com/api/accounts/{Constants.Bandwidth.AccountId}";
        private string messageBaseUrl = $"https://messaging.bandwidth.com/api/v2/users/{Constants.Bandwidth.AccountId}";

        //private readonly TextPortContext _context;
        private readonly RestClient _client;

        public Bandwidth()
        {
            //this._context = context;
            this._client = new RestClient();
            this._client.Authenticator = new HttpBasicAuthenticator(Constants.Bandwidth.ApiToken, Constants.Bandwidth.ApiSecret);
        }

        public List<string> GetVirtualNumbersList(string areaCode, bool tollFree)
        {
            List<String> numbersOut = new List<String>();

            try
            {
                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(Constants.Bandwidth.UserName, Constants.Bandwidth.Password);

                string requestString = string.Empty;
                if (tollFree)
                {
                    requestString = $"/availableNumbers?tollFreeWildCardPattern={areaCode.Substring(0, 2)}*&quantity={Constants.NumberOfNumbersToPullFromBandwidth}";
                }
                else
                {
                    requestString = $"/availableNumbers?areaCode={areaCode}&quantity={Constants.NumberOfNumbersToPullFromBandwidth}";
                }

                RestRequest request = new RestRequest(requestString, Method.GET);
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
                _client.Authenticator = new HttpBasicAuthenticator(Constants.Bandwidth.UserName, Constants.Bandwidth.Password);

                RestRequest request = new RestRequest("/orders", Method.POST)
                {
                    RequestFormat = DataFormat.Xml,
                    XmlSerializer = new DotNetXmlSerializer()
                };
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                Order ord = new Order(regData, Constants.Bandwidth.SubAccountId);
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
                _client.Authenticator = new HttpBasicAuthenticator(Constants.Bandwidth.UserName, Constants.Bandwidth.Password);

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
                _client.Authenticator = new HttpBasicAuthenticator(Constants.Bandwidth.UserName, Constants.Bandwidth.Password);

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

                using (TextPortDA da = new TextPortDA())
                {
                    int accountId = 0;
                    int virtualNumberId = 0;

                    DedicatedVirtualNumber dvn = da.GetVirtualNumberByNumber(bwMessage.to.Replace("+", ""), true);
                    if (dvn != null)
                    {
                        accountId = dvn.AccountId;
                        virtualNumberId = dvn.VirtualNumberId;
                    }

                    Message messageIn = new Message(bwMessage, accountId, virtualNumberId);

                    string result = "Message received from Bandwidth on " + messageIn.TimeStamp.ToString() + "\r\n";
                    result += "Notification Type: " + bwMessage.type + "\r\n";
                    result += "From mobile number: " + messageIn.MobileNumber + "\r\n";
                    result += "To virtual number: " + messageIn.VirtualNumber + "\r\n";
                    result += "Virtual Number ID: " + messageIn.VirtualNumberId.ToString() + "\r\n";
                    result += "Gateway Message ID: " + messageIn.GatewayMessageId + "\r\n";
                    result += "Account Id: " + messageIn.AccountId.ToString() + "\r\n";
                    result += "Message: " + messageIn.MessageText + "\r\n";
                    result += "Data Received: " + jsonPayload + "\r\n";

                    decimal newBalance = 0;
                    int messageId = da.InsertMessage(messageIn, ref newBalance);
                    result += (messageId > 0) ? $"Message successfully added to messages table.{Environment.NewLine}" : $"Failure adding message to messages table.{Environment.NewLine}";

                    // Check for forwarding
                    if (messageIn.AccountId > 0)
                    {
                        string nl = Environment.NewLine;
                        Account account = da.GetAccountById(messageIn.AccountId);
                        if (account != null)
                        {
                            messageIn.Account = account;
                            // Check for email forwarding.
                            if (account.EnableEmailNotifications && !string.IsNullOrEmpty(account.NotificationsEmailAddress))
                            {
                                result += $"Email forwarding enabled. Sending notification to {account.NotificationsEmailAddress}. ";
                                string body = Rendering.RenderMessageInEmail(messageIn);

                                EmailMessage email = new EmailMessage(account.NotificationsEmailAddress, $"TextPort - New Message From {Utilities.NumberToDisplayFormat(messageIn.MobileNumber, 22)}", body);
                                result += (email.Send()) ? "Email sent successfully.\r\n" : "Email send failed.\r\n";
                            }

                            // Check for mobile forwarding.
                            if (account.EnableMobileForwarding && !string.IsNullOrEmpty(account.ForwardVnmessagesTo))
                            {
                                // Make sure the virtual number receiving the message and the forwarding number aren't the same, to avoid pushing a notification
                                // to the same number from which it came and creating a loop.
                                if (messageIn.VirtualNumber != account.ForwardVnmessagesTo)
                                {
                                    result += $"SMS forwarding enabled. Sending notification to {account.ForwardVnmessagesTo}. ";
                                    // Check whether the user has a credit balance
                                    if (account.Balance > 1)
                                    {
                                        result += $"Balance is {account.Balance:C}. OK. ";
                                        // Send the message from the same virtual number on which it was received.
                                        string msg = $"TextPort message received from {Utilities.NumberToDisplayFormat(messageIn.MobileNumber, 22)}:{nl}";
                                        msg += $"{messageIn.MessageText}";

                                        Message notificationMessage = new Message(account.AccountId, (byte)MessageTypes.Notification, messageIn.VirtualNumberId, msg);
                                        notificationMessage.MobileNumber = account.ForwardVnmessagesTo;

                                        decimal newBalance2 = 0;
                                        da.InsertMessage(notificationMessage, ref newBalance2);
                                        result += (notificationMessage.Send()) ? "SMS sent successfully.\r\n" : "SMS send failed.\r\n";
                                    }
                                    else
                                    {
                                        result += $"Insufficient balance: {account.Balance:C}. SMS not sent.\r\n";
                                    }
                                }
                            }
                        }
                    }

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
                string jsonPayload = JsonConvert.SerializeObject(receipt);
                string resultDev = string.Empty;

                switch (receipt.type)
                {
                    case "message-delivered":
                        resultDev = "Delivery receipt received from Bandwidth\r\n";
                        resultDev += "Notification Type: " + receipt.type + "\r\n";
                        resultDev += "To virtual number: " + receipt.to + "\r\n";
                        resultDev += "Data Received: " + jsonPayload + "\r\n";
                        writeXMLToDisk(resultDev, "BandwidthDeliveryReceipt");
                        break;

                    default:
                        resultDev = "Other notificationreceived from Bandwidth\r\n";
                        resultDev += "Notification Type: " + receipt.type + "\r\n";
                        resultDev += "To virtual number: " + receipt.to + "\r\n";
                        resultDev += "Data Received: " + jsonPayload + "\r\n";
                        writeXMLToDisk(resultDev, "BandwidthOTHERReceipt");
                        break;
                }

                return true;
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

                string resultDev = "Delivery failure received from Bandwidth\r\n";
                resultDev += "Notification Type: " + receipt.type + "\r\n";
                resultDev += "To virtual number: " + receipt.to + "\r\n";
                resultDev += "Data Received: " + jsonPayload + "\r\n";
                writeXMLToDisk(resultDev, "BandwidthDeliveryFailure");

                return true;
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
                _client.Authenticator = new HttpBasicAuthenticator(Constants.Bandwidth.ApiToken, Constants.Bandwidth.ApiSecret);

                RestRequest request = new RestRequest("/messages", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddHeader("Content-Type", "application/json; charset=utf-8");

                BandwidthOutboundMessage bwMessage = new BandwidthOutboundMessage(message, Constants.Bandwidth.ApplicationId);
                request.AddJsonBody(bwMessage);

                BandwidthMessageResponse response = _client.Execute<BandwidthMessageResponse>(request).Data;

                if (response != null)
                {
                    if (!string.IsNullOrEmpty(response.id))
                    {
                        message.ProcessingMessage += "Message delivered to Bandwidth gateway. ";
                        using (TextPortDA da = new TextPortDA())
                        {
                            da.UpdateMessageWithGatewayMessageId(message.MessageId, response.id, Constants.BaseSMSMessageCharge, message.ProcessingMessage);
                        };

                        return true;
                    }
                    else
                    {
                        message.ProcessingMessage += "Message delivery to Bandwidth gateway failed. Response processing failure. ";
                        using (TextPortDA da = new TextPortDA())
                        {
                            da.UpdateMessageWithGatewayMessageId(message.MessageId, null, 0, message.ProcessingMessage);
                        };
                    }
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