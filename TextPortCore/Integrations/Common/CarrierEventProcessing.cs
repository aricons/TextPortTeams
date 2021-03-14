using System;
using System.IO;
using System.Configuration;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.APICallback;
using API = TextPortCore.Models.API;

namespace TextPortCore.Integrations.Common
{
    public static class CarrierEventProcessing
    {
        public static Message ProcessInboundMessage(Bandwidth.BandwidthInboundMessage bwMessage)
        {
            return ProcessInboundMessage(new IntegrationMessageIn(bwMessage));
        }

        public static Message ProcessInboundMessage(Nexmo.NexmoInboundMessage nexmoMessage)
        {
            return ProcessInboundMessage(new IntegrationMessageIn(nexmoMessage));
        }

        public static Message ProcessInboundMessage(IntegrationMessageIn messageIn)
        {
            try
            {
                string log = string.Empty;
                bool canContinue = true;
                string forwardVNMessagesTo = String.Empty;
                string userName = String.Empty;
                string sessionId = null;
                DedicatedVirtualNumber dvn = null;

                log += $"Message received from {messageIn.CarrierName} on " + messageIn.TimeStamp.ToString() + "\r\n";
                log += "Carrier: " + messageIn.CarrierName + "\r\n";
                log += "Notification Type: " + messageIn.EventType + "\r\n";
                log += "From mobile number: " + messageIn.From + "\r\n";
                log += "To virtual number: " + messageIn.To + "\r\n";
                log += "Carrier Message ID: " + messageIn.CarrierMessageId + "\r\n";
                log += "Data Received: " + messageIn.DataFromVendor + "\r\n";
                log += $"Performing lookup on virtual number {messageIn.To}\r\n";

                using (TextPortDA da = new TextPortDA())
                {
                    dvn = da.GetVirtualNumberByNumber(messageIn.To, true);
                    if (dvn != null)
                    {
                        log += $"A match on virtual number {messageIn.To} was found. Virtual number ID is {dvn.VirtualNumberId}\r\n";
                    }
                    else
                    {
                        log += $"A match on virtual number {messageIn.To} was not found. Processing stops.\r\n";
                        canContinue = false;
                    }

                    if (!canContinue)
                    {
                        writeXMLToDisk(log, $"{messageIn.CarrierName}InboundMessage-NOT-FOUND");
                        return null;
                    }

                    Message tpMsgIn = new Message(messageIn, dvn, sessionId);

                    // Get the original message to determine the account Id.
                    Message originalMessage = da.GetMessageByBranchIdAndVirtualNumberAndMobileNumber(dvn.BranchId, dvn.VirtualNumberId, messageIn.From);
                    if (originalMessage != null)
                    {
                        log += $"Original message located. Message ID is {originalMessage.MessageId}\r\n";
                        log += "Account Id / Username: " + $"{originalMessage.AccountId} / {originalMessage.Account?.UserName}" + "\r\n";

                        tpMsgIn.AccountId = originalMessage.AccountId;
                        tpMsgIn.Account = originalMessage?.Account;
                    }

                    log += "Creating inbound message.\r\n";
                    log += "Branch Id: " + tpMsgIn.BranchId.ToString() + "\r\n";
                    log += "Account Id: " + tpMsgIn.AccountId.ToString() + "\r\n";
                    log += "Session Id: " + (tpMsgIn.SessionId ?? string.Empty) + "\r\n";
                    log += "Message: " + tpMsgIn.MessageText + "\r\n";
                    log += "Segments: " + tpMsgIn.Segments + "\r\n";

                    // Check that the sending number is not in the BlockedNumbers table as an inbound number.
                    // If a block is found, flush the log and stop processing.
                    //log += "Performing blocked number check.\r\n";
                    //if (da.NumberIsBlocked(tpMsgIn.MobileNumber, MessageDirection.Inbound))
                    //{
                    //    log += $"The sending number {tpMsgIn.MobileNumber} was found on the inbound blocked numbers list. Halting any further processing.\r\n";
                    //    writeXMLToDisk(log, $"{messageIn.CarrierName}InboundMessage-BLOCKED");
                    //    return null;
                    //}
                    //log += "The sending number is not on the blocked numbers list. Continuing.\r\n";

                    decimal newBalance = 0;
                    int messageId = da.InsertMessage(tpMsgIn, ref newBalance);
                    log += (messageId > 0) ? $"Message successfully added to messages table. Message ID is {messageId}{Environment.NewLine}" : $"Failure adding message to messages table.{Environment.NewLine}";

                    Account account = tpMsgIn.Account;

                    // Check to see if this message is a STOP request. If so, add it to the StopRequests table.
                    if (tpMsgIn.MessageText.Trim().Equals("stop", StringComparison.CurrentCultureIgnoreCase))
                    {
                        StopRequest sr = new StopRequest(tpMsgIn.MobileNumber);
                        da.AddNumberStop(sr);
                    }

                    // Check to see if this message is a response to an email-to-sms message.
                    // If so, send an email notification back to that address.
                    //bool isEmailToSMSResponse = false;
                    //string originatingEmailToSMSEmailAddress = da.GetOriginalSMSToEmailSenderAddressByAccountIdVirtualNumberIdAndMobileNumber(tpMsgIn.AccountId, tpMsgIn.VirtualNumberId, tpMsgIn.MobileNumber);
                    //if (!string.IsNullOrEmpty(originatingEmailToSMSEmailAddress))
                    //{
                    //    isEmailToSMSResponse = true;

                    //    log += $"Inbound message detected as response to an Email-to-SMS message. Sending email reply to {originatingEmailToSMSEmailAddress}. ";
                    //    string body = Rendering.RenderEmailToSMSResponseEmail(tpMsgIn, originatingEmailToSMSEmailAddress);

                    //    EmailMessage email = new EmailMessage(originatingEmailToSMSEmailAddress, $"TextPort - New Message From {Utilities.NumberToDisplayFormat(tpMsgIn.MobileNumber, tpMsgIn.DedicatedVirtualNumber.CountryId)}", body);
                    //    log += (email.Send()) ? "Email-to-SMS response notification email sent successfully.\r\n" : "Email send failed.\r\n";
                    //}

                    //if (!isEmailToSMSResponse)
                    //{
                    //    // Check for email forwarding.
                    //    if (account.EnableEmailNotifications && !string.IsNullOrEmpty(account.NotificationsEmailAddress))
                    //    {
                    //        log += $"Email forwarding enabled. Sending notification to {account.NotificationsEmailAddress}. ";
                    //        string body = Rendering.RenderMessageInEmail(tpMsgIn);

                    //        EmailMessage email = new EmailMessage(account.NotificationsEmailAddress, $"TextPort - New Message From {Utilities.NumberToDisplayFormat(tpMsgIn.MobileNumber, tpMsgIn.DedicatedVirtualNumber.CountryId)}", body);
                    //        log += (email.Send()) ? "Email sent successfully.\r\n" : "Email send failed.\r\n";
                    //    }

                    //    // Check for mobile forwarding.
                    //    if (account.EnableMobileForwarding && !string.IsNullOrEmpty(account.ForwardVnmessagesTo))
                    //    {
                    //        // Make sure the virtual number receiving the message and the forwarding number aren't the same, to avoid pushing a notification
                    //        // to the same number from which it came and creating a loop.
                    //        if (dvn.VirtualNumber != account.ForwardVnmessagesTo)
                    //        {
                    //            log += $"SMS forwarding enabled. Sending notification to {account.ForwardVnmessagesTo}. ";
                    //            // Check whether the user has a credit balance
                    //            if (account.Balance > 0.10M)
                    //            {
                    //                log += $"Balance is {account.Balance:C}. OK. ";
                    //                // Send the message from the same virtual number on which it was received.
                    //                string msg = $"TextPort message received from {Utilities.NumberToDisplayFormat(tpMsgIn.MobileNumber, tpMsgIn.DedicatedVirtualNumber.CountryId)}:{Environment.NewLine}";
                    //                msg += $"{tpMsgIn.MessageText}";

                    //                Message notificationMessage = new Message(account.AccountId, (byte)MessageTypes.Notification, tpMsgIn.VirtualNumberId, msg);
                    //                notificationMessage.MobileNumber = account.ForwardVnmessagesTo;

                    //                decimal newBalance2 = 0;
                    //                da.InsertMessage(notificationMessage, ref newBalance2);
                    //                log += (notificationMessage.Send()) ? "SMS sent successfully.\r\n" : "SMS send failed.\r\n";
                    //            }
                    //            else
                    //            {
                    //                log += $"Insufficient balance: {account.Balance:C}. SMS not sent.\r\n";
                    //            }
                    //        }
                    //    }
                    //}

                    // Check for an API application with the number. If one is found, perform an API callback.
                    //if (dvn.APIApplicationId != null && dvn.APIApplicationId > 0)
                    //{
                    //    log += $"An API application ID {dvn.APIApplicationId} was found for number {dvn.VirtualNumber}." + "\r\n";
                    //    APIApplication apiApp = da.GetAPIApplicationById((int)dvn.APIApplicationId);
                    //    if (apiApp != null)
                    //    {
                    //        log += $"API application name is {apiApp.ApplicationName}." + "\r\n";
                    //        if (!string.IsNullOrEmpty(apiApp.CallbackURL))
                    //        {
                    //            string callbackProcessingMessage = string.Empty;

                    //            log += $"A callback URL was found. URL: {apiApp.CallbackURL}. Processing API callback." + "\r\n";

                    //            API.MessageEvent msgEvent = new API.MessageEvent(tpMsgIn, messageIn.EventType);

                    //            if (CallbackProcessor.ProcessAPICallback(apiApp, msgEvent, ref callbackProcessingMessage))
                    //            {
                    //                log += "API callback successful.";
                    //            }
                    //            else
                    //            {
                    //                log += "API callback failed.";
                    //            }
                    //        }
                    //    }
                    //}

                    // Finally, write the log content and return the new message.
                    writeXMLToDisk(log, $"{messageIn.CarrierName}InboundMessage");
                    return tpMsgIn;
                }
            }
            catch (Exception ex)
            {
                string resultErr = $"An error occurred in InboundMessageProcessing.ProcessInboundMessage(). Message: {ex.ToString()}";
                writeXMLToDisk($"{resultErr}. Payload received: {messageIn.DataFromVendor}", "Error_InboundMessage");

                EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
            }
            return null;
        }

        public static DeliveryReceipt ProcessDeliveryReceipt(Bandwidth.BandwidthInboundMessage bwMessage)
        {
            return ProcessDeliveryReceipt(new IntegrationMessageIn(bwMessage));
        }

        public static DeliveryReceipt ProcessDeliveryReceipt(Nexmo.NexmoDeliveryReceipt nexmoReceipt)
        {
            return ProcessDeliveryReceipt(new IntegrationMessageIn(nexmoReceipt));
        }

        public static DeliveryReceipt ProcessDeliveryReceipt(IntegrationMessageIn receipt)
        {
            string logText = string.Empty;
            HubNotification hubNotification = new HubNotification();

            try
            {
                switch (receipt.EventType)
                {
                    case EventTypes.MessageDelivered:
                        logText = $"Delivery receipt received from {receipt.CarrierName}\r\n";
                        logText += "Notification Type: delivered\r\n";
                        logText += "Error Code: " + receipt.ErrorCode + "\r\n";
                        logText += "From mobile number: " + receipt.From + "\r\n";
                        logText += "To virtual number: " + receipt.To + "\r\n";
                        logText += "Vendor Message Id: " + receipt.CarrierMessageId + "\r\n";
                        logText += "Data Received: " + receipt.DataFromVendor + "\r\n";

                        if (!string.IsNullOrEmpty(receipt.CarrierMessageId))
                        {
                            logText += $"Locating original message by vendor message ID {receipt.CarrierMessageId}\r\n";
                            using (TextPortDA da = new TextPortDA())
                            {
                                Message originalMessage = da.GetMessageByGatewayMessageId(receipt.CarrierMessageId);
                                if (originalMessage != null)
                                {
                                    logText += $"Original message located. Message ID is {originalMessage.MessageId}\r\n";
                                    originalMessage.QueueStatus = (byte)QueueStatuses.DeliveryConfirmed;
                                    originalMessage.Segments = receipt.SegmentCount;

                                    logText += "Account Id / Username: " + $"{originalMessage.AccountId} / {originalMessage.Account?.UserName}" + "\r\n";
                                    logText += "Starting balance: " + $"{originalMessage.Account?.Balance}" + "\r\n";

                                    logText += $"Segment count from carrier is {receipt.SegmentCount}\r\n";
                                    synchronizeEstimatedAndActualCost(ref originalMessage, receipt.SegmentCount, ref logText);

                                    logText += CheckForAPICallback(receipt, originalMessage);

                                    logText += $"Saving changes\r\n";
                                    da.SaveChanges();

                                    hubNotification = new HubNotification()
                                    {
                                        MessageId = originalMessage.MessageId,
                                        NotificationMessage = @"<div class='rcpt'><i class='fa fa-check'></i>Delivered</div>",
                                        HubClientId = getNotificationUserName(originalMessage)
                                    };
                                    logText += $"Hub notification will be deliverted to hub client ID {hubNotification.HubClientId}\r\n";
                                }
                                else
                                {
                                    logText += $"An original message with carrier message ID {originalMessage.MessageId} could not be located.\r\n";
                                }
                            }
                        }
                        break;

                    case EventTypes.MessageFailed:
                        logText = $"Delivery failure received from {receipt.CarrierName}\r\n";
                        logText += "Notification Type: failure\r\n";
                        logText += "Error Code: " + receipt.ErrorCode + "\r\n";
                        logText += "Failure Reason: " + receipt.Description + "\r\n";
                        logText += "From mobile number: " + receipt.From + "\r\n";
                        logText += "To virtual number: " + receipt.To + "\r\n";
                        logText += "Vendor Message Id: " + receipt.CarrierMessageId + "\r\n";
                        logText += "Data Received: " + receipt.DataFromVendor + "\r\n";

                        if (!string.IsNullOrEmpty(receipt.CarrierMessageId))
                        {
                            logText += $"Locating original message by vendor message ID {receipt.CarrierMessageId}\r\n";
                            using (TextPortDA da = new TextPortDA())
                            {
                                Message originalMessage = da.GetMessageByGatewayMessageId(receipt.CarrierMessageId);
                                if (originalMessage != null)
                                {
                                    logText += $"Original message located. Message ID is {originalMessage.MessageId}\r\n";
                                    originalMessage.QueueStatus = (byte)QueueStatuses.DeliveryFailed;
                                    originalMessage.Segments = receipt.SegmentCount;

                                    logText += $"Segment count from carrier is {receipt.SegmentCount}\r\n";
                                    processFailure(originalMessage, receipt, ref logText);

                                    logText += CheckForAPICallback(receipt, originalMessage);

                                    logText += $"Saving changes\r\n";
                                    da.SaveChanges();

                                    hubNotification = new HubNotification()
                                    {
                                        MessageId = originalMessage.MessageId,
                                        NotificationMessage = $"<div class='fail-reason'><i class='fa fa-exclamation-triangle'></i>Failed: {receipt.Description}</div>",
                                        HubClientId = getNotificationUserName(originalMessage)
                                    };
                                    logText += $"Hub notification will be deliverted to hub client ID {hubNotification.HubClientId}\r\n";
                                }
                                else
                                {
                                    logText += $"An original message with carrier message ID {originalMessage.MessageId} could not be located.\r\n";
                                }
                            }
                        }
                        writeXMLToDisk(logText, $"{receipt.CarrierName}DeliveryFailure");
                        break;

                    default:
                        logText = $"Other notificationreceived from {receipt.CarrierName}\r\n";
                        logText += "Notification Type: Other/Default\r\n";
                        logText += "Error Code: " + receipt.ErrorCode + "\r\n";
                        logText += "Failure Reason: " + receipt.Description + "\r\n";
                        logText += "From mobile number: " + receipt.From + "\r\n";
                        logText += "To virtual number: " + receipt.To + "\r\n";
                        logText += "Data Received: " + receipt.DataFromVendor + "\r\n";
                        writeXMLToDisk(logText, $"{receipt.CarrierName}OTHERReceipt");
                        break;
                }

                logText += $"Inbound event processing complete\r\n";
                writeXMLToDisk(logText, $"{receipt.CarrierName}DeliveryReceipt");
                return new DeliveryReceipt(receipt, hubNotification);
            }
            catch (Exception ex)
            {
                string resultErr = $"An error occurred in InboundMessagerProcessing.ProcessDeliveryReceipt(). Message: {ex.ToString()}";
                writeXMLToDisk(resultErr, "Error_ProcessDeliveryReceipt");

                EventLogging.WriteEventLogEntry(resultErr, System.Diagnostics.EventLogEntryType.Error);
            }
            return null;
        }

        public static string CheckForAPICallback(IntegrationMessageIn msgIn, Message originalMessage)
        {
            string result = string.Empty;
            DedicatedVirtualNumber dvn = null;

            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    dvn = da.GetVirtualNumberByNumber(msgIn.From, true);
                    if (dvn != null)
                    {
                        // Check for a pooled number
                        if (dvn.NumberType == (byte)NumberTypes.Pooled)
                        {
                            dvn = da.GetVirtualNumberByNumberAndOriginatingMobileNumber(msgIn.From, msgIn.To);
                            if (dvn != null)
                            {
                                result += $"A pooled number match on virtual number {msgIn.From} and mobile number {msgIn.To} was made.";
                            }
                            else
                            {
                                result += $"A pooled match search failed on virtual number {msgIn.From} and mobile number {msgIn.To}.";
                            }
                        }

                        //if (dvn != null)
                        //{
                        //if (dvn.APIApplicationId != null && dvn.APIApplicationId > 0)
                        //{
                        //    result += $"An API application ID {dvn.APIApplicationId} was found for number {dvn.VirtualNumber}." + "\r\n";
                        //    APIApplication apiApp = da.GetAPIApplicationById((int)dvn.APIApplicationId);
                        //    if (apiApp != null)
                        //    {
                        //        result += $"API application name is {apiApp.ApplicationName}." + "\r\n";
                        //        if (!string.IsNullOrEmpty(apiApp.CallbackURL))
                        //        {
                        //            string callbackProcessingMessage = string.Empty;

                        //            result += $"A callback URL was found. URL: {apiApp.CallbackURL}. Processing API callback." + "\r\n";

                        //            API.MessageEvent msgEvent = new API.MessageEvent(msgIn, originalMessage);

                        //            if (CallbackProcessor.ProcessAPICallback(apiApp, msgEvent, ref callbackProcessingMessage))
                        //            {
                        //                result += "API callback successful.";
                        //            }
                        //            else
                        //            {
                        //                result += "API callback failed.";
                        //            }
                        //        }
                        //    }
                        //}
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                result += $"Exception in InboundMessageProcessing.CheckForAPICallback(). Message: {ex.Message}";
                EventLogging.WriteEventLogEntry(ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            return result;
        }

        private static void writeXMLToDisk(string xmlData, string filePrefix)
        {
            StreamWriter xmlFile;

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

        private static bool synchronizeEstimatedAndActualCost(ref Message originalMessage, int segmentCount, ref string logText)
        {
            if (segmentCount == 0)
            {
                segmentCount = 1;
            }

            decimal messageRate;
            if (originalMessage.IsMMS)
            {
                messageRate = (originalMessage.Account.MMSSegmentCost > 0) ? originalMessage.Account.MMSSegmentCost : Constants.BaseMMSSegmentCost;
            }
            else
            {
                messageRate = (originalMessage.Account.SMSSegmentCost > 0) ? originalMessage.Account.SMSSegmentCost : Constants.BaseSMSSegmentCost;
            }

            decimal originalMessageCost = (originalMessage?.CustomerCost != null) ? (decimal)originalMessage.CustomerCost : 0;
            decimal actualMessageCost = messageRate * segmentCount;

            logText += $"Pre-calculated message cost: {originalMessageCost}\r\n";
            logText += $"Actual message cost: {actualMessageCost}\r\n";

            // Only update if actual cost is higher.
            if (actualMessageCost > originalMessageCost)
            {
                decimal costDifferential = actualMessageCost - originalMessageCost;
                logText += $"Actual message cost {actualMessageCost} is higher then original cost {originalMessageCost} by {costDifferential}. Subtracting differential from balance.\r\n";
                originalMessage.Segments = segmentCount;
                originalMessage.CustomerCost = actualMessageCost;
                originalMessage.Account.Balance -= costDifferential;
                logText += $"Updated balance: {originalMessage.Account.Balance}\r\n";
            }
            else
            {
                logText += $"Actual message cost {actualMessageCost} matches originally calculated cost {originalMessageCost}. No cost adjustments.\r\n";
            }

            return true;
        }

        private static bool processFailure(Message originalMessage, IntegrationMessageIn receipt, ref string logText)
        {
            // Log the error reason
            if (!string.IsNullOrEmpty(receipt.ErrorCode) && !string.IsNullOrEmpty(receipt.Description))
            {
                originalMessage.FailureReason = receipt.Description;
            }

            // If the failure is valid (not spam), then credit the message cost back to the account
            decimal originalMessageCost = (originalMessage?.CustomerCost != null) ? (decimal)originalMessage.CustomerCost : 0;
            if (originalMessageCost > 0)
            {
                if (shouldIssueCreditForError(receipt.ErrorCode))
                {
                    logText += $"A creditable error code {receipt.ErrorCode} was found. Applying credit of {originalMessageCost} to account.\r\n";
                    originalMessage.Account.Balance += originalMessageCost;
                }
            }

            logText += $"Account balance: {originalMessage.Account.Balance}\r\n";

            return true;
        }

        private static string getNotificationUserName(Message msg)
        {
            if (msg.MessageType == (byte)MessageTypes.FreeTextSend && !string.IsNullOrEmpty(msg.SessionId))
            {
                return msg.SessionId;
            }
            else
            {
                return msg.Account.UserName;
            }
        }

        private static bool shouldIssueCreditForError(string errorCode)
        {
            switch (errorCode)
            {
                case "1234":
                case "3456":
                    return true;

                case "4321": // SPAM codes
                    return false;
            }
            return false;
        }
    }
}
