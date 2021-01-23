using System;
using System.IO;
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
using TextPortCore.Integrations.Common;

namespace TextPortCore.Integrations.Bandwidth
{
    public class Bandwidth : IDisposable
    {
        const int orderCheckPollingCycles = 12; // number of times to check on an order
        const int orderCheckWaitTime = 2000; // milliseconds to wait between each order status check

        private readonly string accountBaseUrl = $"https://dashboard.bandwidth.com/api/accounts/{ConfigurationManager.AppSettings["BandwidthAccountId"]}";
        private readonly string messageBaseUrl = $"https://messaging.bandwidth.com/api/v2/users/{ConfigurationManager.AppSettings["BandwidthAccountId"]}";

        private readonly RestClient _client;

        public Bandwidth()
        {
            this._client = new RestClient();
            this._client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthApiToken"], ConfigurationManager.AppSettings["BandwidthApiSecret"]);
        }

        public List<string> GetVirtualNumbersList(string areaCode, int numbersToReturn, bool tollFree, int page)
        {
            List<string> numbersOut = new List<string>();

            // Don't fetch any more than 6 pages.
            if (page > 6)
            {
                numbersOut.Add("No more numbers");
                return numbersOut;
            }

            try
            {
                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthUserName"], ConfigurationManager.AppSettings["BandwidthPassword"]);

                string requestString = string.Empty;
                if (tollFree)
                {
                    requestString = $"/availableNumbers?tollFreeWildCardPattern={areaCode.Substring(0, 2)}*&quantity={(numbersToReturn * page)}";
                }
                else
                {
                    requestString = $"/availableNumbers?areaCode={areaCode}&quantity={(numbersToReturn * page)}";
                }

                RestRequest request = new RestRequest(requestString, Method.GET);
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                SearchResult result = _client.Execute<SearchResult>(request).Data;

                int resultNumber = 1;
                int startAtRecord = (numbersToReturn * (page - 1)) + 1;
                foreach (string number in result.TelephoneNumberList)
                {
                    if (resultNumber >= startAtRecord)
                    {
                        numbersOut.Add($"1{number}"); // Add a leading 1 to Bandwidth numbers
                    }
                    resultNumber++;
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.GetVirtualNumbersList(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
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
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthUserName"], ConfigurationManager.AppSettings["BandwidthPassword"]);

                RestRequest request = new RestRequest("/orders", Method.POST)
                {
                    RequestFormat = DataFormat.Xml,
                    XmlSerializer = new DotNetXmlSerializer()
                };
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                Order ord = new Order(regData, ConfigurationManager.AppSettings["BandwidthSubAccountId"]);
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
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthUserName"], ConfigurationManager.AppSettings["BandwidthPassword"]);

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
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthUserName"], ConfigurationManager.AppSettings["BandwidthPassword"]);

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

        public string ReserveVirtualNumber(string number, ref string processingMessage)
        {
            try
            {
                string reservationId = string.Empty;
                processingMessage = string.Empty;

                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthUserName"], ConfigurationManager.AppSettings["BandwidthPassword"]);

                RestRequest request = new RestRequest("/tnreservation", Method.POST)
                {
                    RequestFormat = DataFormat.Xml,
                    XmlSerializer = new DotNetXmlSerializer()
                };
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                Reservation reservation = new Reservation(number);
                request.AddXmlBody(reservation);

                IRestResponse response = _client.Execute<ReservationResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    if (response.Headers != null)
                    {
                        Parameter locationHeader = response.Headers.FirstOrDefault(x => x.Name == "Location");
                        if (locationHeader != null)
                        {
                            if (locationHeader.Value != null)
                            {
                                reservationId = locationHeader.Value.ToString();
                                reservationId = reservationId.Substring(reservationId.LastIndexOf('/') + 1);
                            }
                        }
                    }
                    return reservationId;
                }
                else
                {
                    processingMessage += "Reservation request failed. ";
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.ReserveVirtualNumber(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return string.Empty;
            }
        }

        public bool AllocateReservedNumber(string reservationId)
        {
            try
            {
                _client.BaseUrl = new Uri(accountBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthUserName"], ConfigurationManager.AppSettings["BandwidthPassword"]);

                RestRequest request = new RestRequest("/tnreservation", Method.POST)
                {
                    RequestFormat = DataFormat.Xml,
                    XmlSerializer = new DotNetXmlSerializer()
                };
                request.AddHeader("Content-Type", "application/xml; charset=utf-8");

                Reservation reservation = new Reservation(reservationId);
                request.AddXmlBody(reservation);

                IRestResponse response = _client.Execute<ReservationResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    if (response.Headers != null)
                    {
                        Parameter locationHeader = response.Headers.FirstOrDefault(x => x.Name == "Location");
                        if (locationHeader != null)
                        {
                            if (locationHeader.Value != null)
                            {
                                reservationId = locationHeader.Value.ToString();
                                reservationId = reservationId.Substring(reservationId.LastIndexOf('/') + 1);
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in BandwidthCom.AllocateReservedNumber(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return false;
        }


        //public Message ProcessInboundMessage(BandwidthInboundMessage bwMessage)
        //{
        //    IntegrationMessageIn messageIn = new IntegrationMessageIn(bwMessage);
        //    return  InboundMessageProcessing.ProcessInboundMessage(messageIn);
        //}

        //public Message ProcessInboundMessage(BandwidthInboundMessage bwMessage)
        //{
        //    string jsonPayload = string.Empty;
        //    try
        //    {
        //        string resultMessage = String.Empty;
        //        string forwardVNMessagesTo = String.Empty;
        //        string userName = String.Empty;
        //        jsonPayload = JsonConvert.SerializeObject(bwMessage);

        //        if (bwMessage.type == "message-failed")
        //        {
        //            // Switch the from and to fields.
        //            string tempTo = bwMessage.to;
        //            bwMessage.to = bwMessage.message.from;
        //            bwMessage.message.from = tempTo;

        //            if (!string.IsNullOrEmpty(bwMessage.description))
        //            {
        //                switch (bwMessage.description)
        //                {
        //                    case "rejected-spam-detected":
        //                        bwMessage.message.text = $"DELIVERY FAILURE. Reason: {bwMessage.description}. The destination provider for number {tempTo.Replace("+", "")} detected this message as spam: {bwMessage.message.text}";
        //                        break;
        //                    default:
        //                        bwMessage.message.text = $"DELIVERY FAILURE. Reason: {bwMessage.description}. Destination number: {tempTo.Replace("+", "")}. Message: {bwMessage.message.text}";
        //                        break;
        //                }
        //            }
        //        }

        //        using (TextPortDA da = new TextPortDA())
        //        {
        //            int accountId = 0;
        //            int virtualNumberId = 0;
        //            string sessionId = null;
        //            string pooledNumberSearchResult = string.Empty;
        //            bool isEmailToSMSResponse = false;

        //            DedicatedVirtualNumber dvn = null;

        //            dvn = da.GetVirtualNumberByNumber(bwMessage.to.Replace("+", ""), true);
        //            if (dvn != null)
        //            {
        //                // First check whether this number is a free trial pool number. If so, perform and additional lookup for an outgoing 
        //                // message with a destination number that matches the sending number of the message being received.
        //                if (dvn.NumberType == (byte)NumberTypes.Pooled)
        //                {
        //                    if (bwMessage.message != null)
        //                    {
        //                        dvn = da.GetVirtualNumberByNumberAndOriginatingMobileNumber(bwMessage.to.Replace("+", ""), bwMessage.message.from.Replace("+", ""));
        //                        if (dvn != null)
        //                        {
        //                            accountId = dvn.AccountId;
        //                            virtualNumberId = dvn.VirtualNumberId;
        //                            pooledNumberSearchResult = $"A pooled number match on virtual number {bwMessage.to.Replace("+", "")} and mobile number {bwMessage.message.from.Replace("+", "")} was made.";
        //                        }
        //                        else
        //                        {
        //                            pooledNumberSearchResult = pooledNumberSearchResult = $"A pooled match search failed on virtual number {bwMessage.to.Replace("+", "")} and mobile number {bwMessage.message.from.Replace("+", "")}.";
        //                        }
        //                    }
        //                }
        //                else if (dvn.NumberType == (byte)NumberTypes.Free)
        //                {
        //                    if (bwMessage.message != null)
        //                    {
        //                        accountId = dvn.AccountId;
        //                        virtualNumberId = dvn.VirtualNumberId;
        //                        pooledNumberSearchResult = $"The virtual number {bwMessage.to.Replace("+", "")} is a free texting number. Looking for an associated outbound message to get the session ID.\r\n";
        //                        Message freeMessage = da.GetOriginatingMessageByVirtualNumberIdAndMobileNumberAndMessageType(dvn.VirtualNumberId, bwMessage.message.from.Replace("+", ""), MessageTypes.FreeTextSend);
        //                        if (freeMessage != null)
        //                        {
        //                            sessionId = freeMessage.SessionId;
        //                            pooledNumberSearchResult += $"An originating free message was found. The message ID is {freeMessage.MessageId}. The session ID is {freeMessage.SessionId}.\r\n";
        //                        }
        //                        else
        //                        {
        //                            pooledNumberSearchResult += $"An originating free message was not found found. Processing stops.\r\n";
        //                        }
        //                    }
        //                }
        //                else // Regular non-pooled number
        //                {
        //                    accountId = dvn.AccountId;
        //                    virtualNumberId = dvn.VirtualNumberId;
        //                }
        //            }

        //            Message messageIn = new Message(bwMessage, dvn, accountId, virtualNumberId, sessionId);

        //            string result = "Message received from Bandwidth on " + messageIn.TimeStamp.ToString() + "\r\n";
        //            result += "Notification Type: " + bwMessage.type + "\r\n";
        //            result += "From mobile number: " + messageIn.MobileNumber + "\r\n";
        //            result += "To virtual number: " + messageIn.VirtualNumber + "\r\n";
        //            result += "Virtual Number ID: " + messageIn.VirtualNumberId.ToString() + "\r\n";
        //            result += "Gateway Message ID: " + messageIn.GatewayMessageId + "\r\n";
        //            result += "Account Id: " + messageIn.AccountId.ToString() + "\r\n";
        //            result += "Session Id: " + (messageIn.SessionId ?? string.Empty) + "\r\n";
        //            result += "Message: " + messageIn.MessageText + "\r\n";
        //            result += "Segments: " + messageIn.Segments + "\r\n";
        //            if (!string.IsNullOrEmpty(pooledNumberSearchResult))
        //            {
        //                result += "Pooled Number Status: " + pooledNumberSearchResult + "\r\n";
        //            }
        //            else
        //            {
        //                result += "Pooled Number Status: Message not sent to a pooled number.\r\n";
        //            }
        //            result += "Data Received: " + jsonPayload + "\r\n";

        //            // Check that the sending number is not in the BlockedNumbers table as an inbound number.
        //            // If a block is found, flush the log and stop processing.
        //            if (da.NumberIsBlocked(messageIn.MobileNumber, MessageDirection.Inbound))
        //            {
        //                result += $"The sending number {messageIn.MobileNumber} was found on the inbound blocked numbers list. Halting any further processing.\r\n";
        //                InboundMessageProcessing.WriteXMLToDisk(result, "BandwidthInboundMessage-BLOCKED");
        //                return null;
        //            }

        //            decimal newBalance = 0;
        //            int messageId = da.InsertMessage(messageIn, ref newBalance);
        //            result += (messageId > 0) ? $"Message successfully added to messages table.{Environment.NewLine}" : $"Failure adding message to messages table.{Environment.NewLine}";

        //            // Check for forwarding and email-to-SMS message responses
        //            if (messageIn.AccountId > 0)
        //            {
        //                string nl = Environment.NewLine;
        //                Account account = da.GetAccountById(messageIn.AccountId);
        //                if (account != null)
        //                {
        //                    messageIn.Account = account;

        //                    // Check to see if this message is a response to an email-to-sms message.
        //                    // If so, send an email notification back to that address.
        //                    string originatingEmailToSMSEmailAddress = da.GetOriginalSMSToEmailSenderAddressByAccountIdVirtualNumberIdAndMobileNumber(messageIn.AccountId, messageIn.VirtualNumberId, messageIn.MobileNumber);
        //                    if (!string.IsNullOrEmpty(originatingEmailToSMSEmailAddress))
        //                    {
        //                        isEmailToSMSResponse = true;

        //                        result += $"Inbound message detected as response to an Email-to-SMS message. Sending email reply to {originatingEmailToSMSEmailAddress}. ";
        //                        string body = Rendering.RenderEmailToSMSResponseEmail(messageIn, originatingEmailToSMSEmailAddress);

        //                        EmailMessage email = new EmailMessage(originatingEmailToSMSEmailAddress, $"TextPort - New Message From {Utilities.NumberToDisplayFormat(messageIn.MobileNumber, messageIn.DedicatedVirtualNumber.CountryId)}", body);
        //                        result += (email.Send()) ? "Email-to-SMS response notification email sent successfully.\r\n" : "Email send failed.\r\n";
        //                    }

        //                    if (!isEmailToSMSResponse)
        //                    {
        //                        // Check for email forwarding.
        //                        if (account.EnableEmailNotifications && !string.IsNullOrEmpty(account.NotificationsEmailAddress))
        //                        {
        //                            result += $"Email forwarding enabled. Sending notification to {account.NotificationsEmailAddress}. ";
        //                            string body = Rendering.RenderMessageInEmail(messageIn);

        //                            EmailMessage email = new EmailMessage(account.NotificationsEmailAddress, $"TextPort - New Message From {Utilities.NumberToDisplayFormat(messageIn.MobileNumber, messageIn.DedicatedVirtualNumber.CountryId)}", body);
        //                            result += (email.Send()) ? "Email sent successfully.\r\n" : "Email send failed.\r\n";
        //                        }

        //                        // Check for mobile forwarding.
        //                        if (account.EnableMobileForwarding && !string.IsNullOrEmpty(account.ForwardVnmessagesTo))
        //                        {
        //                            // Make sure the virtual number receiving the message and the forwarding number aren't the same, to avoid pushing a notification
        //                            // to the same number from which it came and creating a loop.
        //                            if (messageIn.VirtualNumber != account.ForwardVnmessagesTo)
        //                            {
        //                                result += $"SMS forwarding enabled. Sending notification to {account.ForwardVnmessagesTo}. ";
        //                                // Check whether the user has a credit balance
        //                                if (account.Balance > 0.10M)
        //                                {
        //                                    result += $"Balance is {account.Balance:C}. OK. ";
        //                                    // Send the message from the same virtual number on which it was received.
        //                                    string msg = $"TextPort message received from {Utilities.NumberToDisplayFormat(messageIn.MobileNumber, messageIn.DedicatedVirtualNumber.CountryId)}:{nl}";
        //                                    msg += $"{messageIn.MessageText}";

        //                                    Message notificationMessage = new Message(account.AccountId, (byte)MessageTypes.Notification, messageIn.VirtualNumberId, msg);
        //                                    notificationMessage.MobileNumber = account.ForwardVnmessagesTo;

        //                                    decimal newBalance2 = 0;
        //                                    da.InsertMessage(notificationMessage, ref newBalance2);
        //                                    result += (notificationMessage.Send()) ? "SMS sent successfully.\r\n" : "SMS send failed.\r\n";
        //                                }
        //                                else
        //                                {
        //                                    result += $"Insufficient balance: {account.Balance:C}. SMS not sent.\r\n";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // Check for API forwarding
        //            if (dvn != null)
        //            {
        //                if (dvn.APIApplicationId != null && dvn.APIApplicationId > 0)
        //                {
        //                    result += $"An API application ID {dvn.APIApplicationId} was found for number {dvn.VirtualNumber}." + "\r\n";
        //                    APIApplication apiApp = da.GetAPIApplicationById((int)dvn.APIApplicationId);
        //                    if (apiApp != null)
        //                    {
        //                        result += $"API application name is {apiApp.ApplicationName}." + "\r\n";
        //                        if (!string.IsNullOrEmpty(apiApp.CallbackURL))
        //                        {
        //                            string callbackProcessingMessage = string.Empty;

        //                            result += $"A callback URL was found. URL: {apiApp.CallbackURL}. Processing API callback." + "\r\n";

        //                            API.MessageEvent msgEvent = new API.MessageEvent(messageIn, bwMessage.type);

        //                            if (CallbackProcessor.ProcessAPICallback(apiApp, msgEvent, ref callbackProcessingMessage))
        //                            {
        //                                result += "API callback successful.";
        //                            }
        //                            else
        //                            {
        //                                result += "API callback failed.";
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            InboundMessageProcessing.WriteXMLToDisk(result, "BandwidthInboundMessage");

        //            return messageIn;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string resultErr = $"An error occurred in BandwidthCom.ProcessInboundMessage(). Message: {ex.ToString()}";
        //        InboundMessageProcessing.WriteXMLToDisk($"{resultErr}. Payload received: {jsonPayload}", "Error_InboundMessage");

        //        EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
        //    }
        //    return null;
        //}

        //public bool ProcessDeliveryReceipt(BandwidthInboundMessage receipt)
        //{
        //    try
        //    {
        //        string jsonPayload = JsonConvert.SerializeObject(receipt);
        //        string resultDev = string.Empty;

        //        switch (receipt.type)
        //        {
        //            case "message-delivered":
        //                resultDev = "Delivery receipt received from Bandwidth\r\n";
        //                resultDev += "Notification Type: " + receipt.type + "\r\n";
        //                resultDev += "Error Code: " + receipt.errorCode + "\r\n";
        //                resultDev += "To virtual number: " + receipt.to + "\r\n";
        //                resultDev += "Data Received: " + jsonPayload + "\r\n";
        //                resultDev +=  checkForAPICallback(receipt);
        //                InboundMessageProcessing.WriteXMLToDisk(resultDev, "BandwidthDeliveryReceipt");
        //                break;

        //            default:
        //                resultDev = "Other notificationreceived from Bandwidth\r\n";
        //                resultDev += "Notification Type: " + receipt.type + "\r\n";
        //                resultDev += "Error Code: " + receipt.errorCode + "\r\n";
        //                resultDev += "To virtual number: " + receipt.to + "\r\n";
        //                resultDev += "Data Received: " + jsonPayload + "\r\n";
        //                InboundMessageProcessing.WriteXMLToDisk(resultDev, "BandwidthOTHERReceipt");
        //                break;
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string resultErr = $"An error occurred in BandwidthCom.ProcessDeliveryReceipt(). Message: {ex.ToString()}";
        //        InboundMessageProcessing.WriteXMLToDisk(resultErr, "Error_ProcessDeliveryReceipt");

        //        EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
        //    }
        //    return false;
        //}

        //public bool ProcessDeliveryFailure(BandwidthInboundMessage receipt)
        //{
        //    try
        //    {
        //        string resultMessage = String.Empty;
        //        string forwardVNMessagesTo = String.Empty;
        //        string userName = String.Empty;
        //        string jsonPayload = JsonConvert.SerializeObject(receipt);

        //        string resultDev = "Delivery failure received from Bandwidth\r\n";
        //        resultDev += "Notification Type: " + receipt.type + "\r\n";
        //        resultDev += "Error Code: " + receipt.errorCode + "\r\n";
        //        resultDev += "To virtual number: " + receipt.to + "\r\n";
        //        resultDev += "Data Received: " + jsonPayload + "\r\n";
        //        resultDev += InboundMessageProcessing.CheckForAPICallback(receipt);
        //        InboundMessageProcessing.WriteXMLToDisk(resultDev, "BandwidthDeliveryFailure");

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string resultErr = $"An error occurred in BandwidthCom.ProcessDeliveryFailure(). Message: {ex.ToString()}";
        //        InboundMessageProcessing.WriteXMLToDisk(resultErr, "Error_ProcessDeliveryFailure");

        //        EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
        //    }
        //    return false;
        //}

        public bool RouteMessageViaBandwidthDotComGateway(Message message)
        {
            try
            {
                _client.BaseUrl = new Uri(messageBaseUrl);
                _client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BandwidthApiToken"], ConfigurationManager.AppSettings["BandwidthApiSecret"]);

                RestRequest request = new RestRequest("/messages", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddHeader("Content-Type", "application/json; charset=utf-8");

                BandwidthOutboundMessage bwMessage = new BandwidthOutboundMessage(message, ConfigurationManager.AppSettings["BandwidthApplicationId"]);
                request.AddJsonBody(bwMessage);

                BandwidthMessageResponse response = _client.Execute<BandwidthMessageResponse>(request).Data;

                if (response != null)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        if (!string.IsNullOrEmpty(response.id))
                        {
                            da.UpdateMessageWithGatewayMessageId(message.MessageId, response.id, response.segmentCount, QueueStatuses.SentToProvider, "Message delivered to Bandwidth gateway. ");
                            return true;
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

        //private string CheckForAPICallback(BandwidthInboundMessage bwMessage)
        //{
        //    string result = string.Empty;
        //    DedicatedVirtualNumber dvn = null;

        //    try
        //    {
        //        using (TextPortDA da = new TextPortDA())
        //        {
        //            dvn = da.GetVirtualNumberByNumber(bwMessage.message.from.Replace("+", ""), true);
        //            if (dvn != null)
        //            {
        //                // Check for a pooled number
        //                if (dvn.NumberType == (byte)NumberTypes.Pooled)
        //                {
        //                    if (bwMessage.message != null)
        //                    {
        //                        dvn = da.GetVirtualNumberByNumberAndOriginatingMobileNumber(bwMessage.message.from.Replace("+", ""), bwMessage.to.Replace("+", ""));
        //                        if (dvn != null)
        //                        {
        //                            result += $"A pooled number match on virtual number {bwMessage.message.from.Replace("+", "")} and mobile number {bwMessage.to.Replace("+", "")} was made.";
        //                        }
        //                        else
        //                        {
        //                            result += $"A pooled match search failed on virtual number {bwMessage.message.from.Replace("+", "")} and mobile number {bwMessage.to.Replace("+", "")}.";
        //                        }
        //                    }
        //                }

        //                if (dvn != null)
        //                {
        //                    if (dvn.APIApplicationId != null && dvn.APIApplicationId > 0)
        //                    {
        //                        result += $"An API application ID {dvn.APIApplicationId} was found for number {dvn.VirtualNumber}." + "\r\n";
        //                        APIApplication apiApp = da.GetAPIApplicationById((int)dvn.APIApplicationId);
        //                        if (apiApp != null)
        //                        {
        //                            result += $"API application name is {apiApp.ApplicationName}." + "\r\n";
        //                            if (!string.IsNullOrEmpty(apiApp.CallbackURL))
        //                            {
        //                                string callbackProcessingMessage = string.Empty;

        //                                result += $"A callback URL was found. URL: {apiApp.CallbackURL}. Processing API callback." + "\r\n";

        //                                API.MessageEvent msgEvent = new API.MessageEvent(bwMessage);

        //                                if (CallbackProcessor.ProcessAPICallback(apiApp, msgEvent, ref callbackProcessingMessage))
        //                                {
        //                                    result += "API callback successful.";
        //                                }
        //                                else
        //                                {
        //                                    result += "API callback failed.";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result += $"Exception in Bandwidth.checkForAPICallback(). Message: {ex.Message}";
        //        EventLogging.WriteEventLogEntry(ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
        //    }
        //    return result;
        //}

        //private void writeXMLToDisk(string xmlData, string filePrefix)
        //{
        //    StreamWriter xmlFile;

        //    string baseFolder = ConfigurationManager.AppSettings["APILogFiles"];
        //    string fileName = $"{baseFolder}{filePrefix}_{DateTime.Now:yyyy-MM-ddThh-mm-ss}.txt";

        //    try
        //    {
        //        using (xmlFile = new StreamWriter(fileName, true))
        //        {
        //            xmlFile.WriteLine(xmlData);
        //            xmlFile.Flush();
        //            xmlFile.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string foo = ex.Message;
        //    }
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