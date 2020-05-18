using System;
using System.Collections.Generic;
using System.Configuration;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

using TextPortCore.Integrations.Bandwidth;

namespace TextPortServices.Processes
{
    public static class VnExpirationsPoller
    {
        // Public methods
        public static void CheckForVirtualNumberExpirations()
        {
            CheckForVirtualNumberExpirationNotifications(7, NumberTypes.Regular);
            CheckForVirtualNumberExpirationNotifications(2, NumberTypes.Regular);

            CheckForVirtualNumberExpirationNotifications(7, NumberTypes.Pooled);
            CheckForVirtualNumberExpirationNotifications(2, NumberTypes.Pooled);

            CheckForAutoRenewNumberExpirtionLowBalanceNotifications(7, NumberTypes.Regular);
            CheckForAutoRenewNumberExpirtionLowBalanceNotifications(2, NumberTypes.Regular);

            CheckForAndCancelExpiredNumbers();

            CheckForAndRenewAutoRenewNumbers();

            CheckForAndCancelAutoRenewNumbersWithInsufficientBalance();
        }

        public static int GetPollingInterval()
        {
            const int secondsPerHour = 3600; // 60s * 60min

            int interPollingSleepSeconds = 0;
            int pollEveryXHours = 1;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["CheckForVNExpirationsEveryXHours"], out pollEveryXHours);
                interPollingSleepSeconds = pollEveryXHours * secondsPerHour;
            }
            catch (Exception)
            {
                interPollingSleepSeconds = 3 * secondsPerHour;
            }

            return interPollingSleepSeconds;
        }

        // Private methods
        public static void CheckForVirtualNumberExpirationNotifications(int days, NumberTypes numberType)
        {
            using (TextPortDA da = new TextPortDA())
            {
                List<NumberExpirationData> expirations = da.GetNumberExpirationNotifications(days, numberType, $"{days}DayExpirationNotification", "numbers/renew");
                if (expirations != null)
                {
                    foreach (NumberExpirationData expiration in expirations)
                    {
                        //expiration.Email = "richard@arionconsulting.com"; // For testing
                        if (sendVirtualNumberExpirationEmail(expiration))
                        {
                            da.SetVirtualNumberXDayReminderSentFlag(expiration.VirtualNumberID, days);
                        }
                    }
                }
            }
        }

        public static void CheckForAutoRenewNumberExpirtionLowBalanceNotifications(int days, NumberTypes numberType)
        {
            using (TextPortDA da = new TextPortDA())
            {
                List<NumberExpirationData> expirations = da.GetAutoRenewBalanceWarningNotifications(days, numberType, $"{days}DayLowBalanceNotification", "account/topup");
                if (expirations != null)
                {
                    foreach (NumberExpirationData expiration in expirations)
                    {
                        //expiration.Email = "richard@arionconsulting.com"; // For testing
                        if (sendAutoRenewNumberLowBalanceNotificationEmail(expiration))
                        {
                            da.SetVirtualNumberXDayReminderSentFlag(expiration.VirtualNumberID, days);
                        }
                    }
                }
            }
        }

        public static void CheckForAndCancelExpiredNumbers()
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<NumberExpirationData> expirations = da.GetExpiredNumbers();
                    if (expirations != null)
                    {
                        foreach (NumberExpirationData expiredNumber in expirations)
                        {
                            if (expiredNumber.NumberType == (int)NumberTypes.Regular)
                            {
                                if (disconnectVirtualNumberFromProvider(expiredNumber))
                                {
                                    da.SetVirtualNumberCancelledFlag(expiredNumber.VirtualNumberID, true);
                                }
                                else
                                {
                                    sendAdminEmailNotification("VnExpirationsPoller", "CheckForAndCancelExpiredNumbers", $"The call to disconnectVirtualNumberFromProvider for carrier {expiredNumber.CarrierId} failed when attempting to cancel the virtual number {expiredNumber.VirtualNumber} with virtual number ID {expiredNumber.VirtualNumberID}.");
                                    da.IncrementVirtualNumberCancellationFailureCount(expiredNumber.VirtualNumberID);
                                }
                            }
                            else
                            {
                                // Don't disconnect pooled numbers. Just set the cancelled flag.
                                da.SetVirtualNumberCancelledFlag(expiredNumber.VirtualNumberID, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sendAdminEmailNotification("VnExpirationsPoller", "CheckForAndCancelExpiredNumbers", $"An excpetion occurred with error message: {ex.Message}. Stack trace: {ex.StackTrace}");
            }
        }

        public static void CheckForAndRenewAutoRenewNumbers()
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<NumberExpirationData> renewals = da.GetAutoRenewNumbers();
                    if (renewals != null)
                    {
                        foreach (NumberExpirationData autoRenewNumber in renewals)
                        {
                            if (da.AutoRenewNumber(autoRenewNumber.VirtualNumberID))
                            {
                                da.DebitFeeFromAccount(autoRenewNumber.AccountID, autoRenewNumber.Fee);
                            }
                            else
                            {
                                sendAdminEmailNotification("VnExpirationsPoller", "CheckForAndRenewAutoRenewNumbers.da.AutoRenewNumber", $"The call to auto-renew the number {autoRenewNumber.VirtualNumber} with virtual number ID {autoRenewNumber.VirtualNumberID} failed.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sendAdminEmailNotification("VnExpirationsPoller", "CheckForAndRenewAutoRenewNumbers", $"An excpetion occurred with error message {ex.Message}. Stack trace: {ex.StackTrace}");
            }
        }

        public static void CheckForAndCancelAutoRenewNumbersWithInsufficientBalance()
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<NumberExpirationData> cancellations = da.GetAutoRenewInsufficientBalanceExpirations();
                    if (cancellations != null)
                    {
                        foreach (NumberExpirationData autoRenewNumber in cancellations)
                        {
                            if (disconnectVirtualNumberFromProvider(autoRenewNumber))
                            {
                                da.SetVirtualNumberCancelledFlag(autoRenewNumber.VirtualNumberID, true);
                            }
                            else
                            {
                                sendAdminEmailNotification("VnExpirationsPoller", "CheckForAndCancelAutoRenewNumbersWithInsufficientBalance", $"The call to CheckForAndCancelAutoRenewNumbersWithInsufficientBalance for provider {autoRenewNumber.CarrierId} failed when attempting to cancel the virtual number {autoRenewNumber.VirtualNumber} with virtual number ID {autoRenewNumber.VirtualNumberID}.");
                                da.IncrementVirtualNumberCancellationFailureCount(autoRenewNumber.VirtualNumberID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sendAdminEmailNotification("VnExpirationsPoller", "CheckForAndCancelAutoRenewNumbersWithInsufficientBalance", $"An excpetion occurred with error message {ex.Message}. Stack trace: {ex.StackTrace}");
            }
        }

        private static bool disconnectVirtualNumberFromProvider(NumberExpirationData expiredNumber)
        {
            switch (expiredNumber.CarrierId)
            {
                case (int)Carriers.BandWidth:
                    using (Bandwidth bw = new Bandwidth())
                    {
                        string processingMessage = string.Empty;
                        return bw.DisconnectVirtualNumber(new DedicatedVirtualNumber(expiredNumber), ref processingMessage);
                    }

                    //if (BwApi.CancelVirtualNumber(expData.VirtualNumber))
                    //{
                    //    DataAccess.SetVirtualNumberCancelledFlag(expData.VirtualNumberID, true);
                    //}
                    //else
                    //{
                    //    sendAdminEmailNotification("VnExpirationsPoller", "checkForAndCancelExpiredNumbers", "The call to Bandwidth.CancelVirtualNumber failed when attempting to cancel the virtual number " + expData.VirtualNumber + " with virtual number ID " + expData.VirtualNumberID.ToString() + ".");
                    //    Common.WriteEventLogEntry("An error occurred in VnExpirationsPoller.checkForAndCancelExpiredNumbers. The call to Bandwidth.CancelVirtualNumber failed when attempting to cancel the virtual number " + expData.VirtualNumber + " with virtual number ID " + expData.VirtualNumberID.ToString() + ".", System.Diagnostics.EventLogEntryType.Error);
                    //    DataAccess.SetVirtualNumberFailureFlag(expData.VirtualNumberID, "Cancellation");
                    //}
                    //break;
                    //case "Nexmo":
                    //    if (Nexmo.CancelVirtualNumber(expData.CountryCode, expData.VirtualNumber))
                    //    {
                    //        DataAccess.SetVirtualNumberCancelledFlag(expData.VirtualNumberID, true);
                    //    }
                    //    else
                    //    {
                    //        sendAdminEmailNotification("VnExpirationsPoller", "checkForAndCancelExpiredNumbers", "The call to Nexmo.CancelVirtualNumber failed when attempting to cancel the virtual number " + expData.VirtualNumber + " with virtual number ID " + expData.VirtualNumberID.ToString() + ".");
                    //        Common.WriteEventLogEntry("An error occurred in VnExpirationsPoller.checkForAndCancelExpiredNumbers. The call to Nexmo.CancelVirtualNumber failed when attempting to cancel the virtual number " + expData.VirtualNumber + " with virtual number ID " + expData.VirtualNumberID.ToString() + ".", System.Diagnostics.EventLogEntryType.Error);
                    //        DataAccess.SetVirtualNumberFailureFlag(expData.VirtualNumberID, "Cancellation");
                    //    }
                    //    break;
            }

            return false;
        }

        private static bool sendVirtualNumberExpirationEmail(NumberExpirationData expirationData)
        {
            try
            {
                string body = Rendering.RenderVirtualNumberExpirationEmailBody(expirationData);
                string subjectLine = "TextPort - Virtual Number Expiration Notification";
                subjectLine += (expirationData.NotificationType == "2DayExpirationNotification") ? " - Final Notice" : string.Empty;

                using (EmailMessage message = new EmailMessage(expirationData.Email, subjectLine, body))
                {
                    return message.Send();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("VNExpirationsPoller.sendVirtualNumberExpirationEmail", ex);
            }

            return false;
        }

        private static bool sendAutoRenewNumberLowBalanceNotificationEmail(NumberExpirationData expirationData)
        {
            try
            {
                string body = Rendering.RenderAutoRenewNumberLowBalanceNotificationBody(expirationData);
                string subjectLine = "TextPort - Virtual Number Auto-Renew - Low Balance";
                subjectLine += (expirationData.NotificationType == "2DayExpirationNotification" || expirationData.NotificationType == "2DayLowBalanceNotification") ? " - Final Notice" : string.Empty;

                using (EmailMessage message = new EmailMessage(expirationData.Email, subjectLine, body))
                {
                    return message.Send();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("VNExpirationsPoller.sendVirtualNumberExpirationEmail", ex);
            }

            return false;
        }

        private static void sendAdminEmailNotification(string moduleName, string methodName, string messageText)
        {
            try
            {
                string emailBody = $"An error was encountered on the TextPort system.{Environment.NewLine}{Environment.NewLine}";
                emailBody += $"Module: {moduleName}{Environment.NewLine}";
                emailBody += $"Method: {methodName}{Environment.NewLine}";
                emailBody += $"Detail: {messageText}{Environment.NewLine}{Environment.NewLine}";
                emailBody += "End of message.";

                EmailMessage email = new EmailMessage()
                {
                    From = "support@textport.com",
                    FromName = "TextPort Notifications",
                    To = "support@textport.com",
                    Subject = "TextPort - Error Notification",
                    Body = emailBody
                };

                email.Send(false);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("VNExpirationsPoller.sendAdminEmailNotification", ex);
            }
        }
    }
}
