using System;
using System.IO;
using System.Configuration;

using TextPortCore.Models;

namespace TextPortCore.Helpers
{
    public static class Rendering
    {
        public static NotificationHtmls RenderMessageIn(Message msg)
        {
            // Recents list item
            string rHtml = "<div class=\"chat_people\"><div class=\"chat_ib\">";
            rHtml += $"<h5>{Utilities.NumberToDisplayFormat(msg.MobileNumber, 22)}<span class=\"chat_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></h5>";
            rHtml += $"<p>{msg.MessageText}</p></div></div>";

            // Messge list item
            string mHtml = $"<div id=\"{msg.MessageId}\" class=\"msg_item\">";
            mHtml += $"<div class=\"received_msg\">";
            foreach (MMSFile mms in msg.MMSFiles)
            {
                mHtml += $"<div><img src=\"{MMSUtils.GetMMSFileURL(msg.AccountId, mms.StorageId, mms.FileName)}\" alt = \"@mms.FileName\" /></div>";
            }
            mHtml += $"<p>{msg.MessageText}</p>";
            mHtml += $"<span class=\"time_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></div></div>";

            return new NotificationHtmls(rHtml, mHtml);
        }

        public static string RenderMessageInEmail(Message msg)
        {
            string html = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}InboundMessageNotification.html");
            html = html.Replace("{{name}}", msg.Account.UserName);
            html = html.Replace("{{time}}", $"{msg.TimeStamp:MM-dd-yyyy hh:mm tt}");
            html = html.Replace("{{to_number}}", msg.VirtualNumber);
            html = html.Replace("{{from_number}}", msg.MobileNumber);
            html = html.Replace("{{message}}", msg.MessageText);
            html = html.Replace("{{copy_year}}", $"{DateTime.Now.Year}");

            return html;
        }

        public static string RenderEmailToSMSResponseEmail(Message msg, string emailAddress)
        {
            string salutationName = emailAddress.Substring(0, emailAddress.IndexOf("@"));
            string html = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}EmailToSMSResponseNotification.html");
            html = html.Replace("{{name}}", salutationName);
            html = html.Replace("{{time}}", $"{msg.TimeStamp:MM-dd-yyyy hh:mm tt}");
            html = html.Replace("{{to_number}}", msg.VirtualNumber);
            html = html.Replace("{{from_number}}", msg.MobileNumber);
            html = html.Replace("{{message}}", msg.MessageText);
            html = html.Replace("{{copy_year}}", $"{DateTime.Now.Year}");

            return html;
        }

        public static string RenderForgotPasswordEmailBody(ForgotPasswordRequest req)
        {
            string template = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}ResetPassword.html");
            template = template.Replace("{{name}}", req.UserName);
            template = template.Replace("{{action_url}}", req.ResetUrl);
            template = template.Replace("{{ip_address}}", req.IPAddress);
            template = template.Replace("{{browser_type}}", req.BrowserType);
            template = template.Replace("{{copy_year}}", $"{DateTime.Now.Year}");

            return template;
        }

        public static string RenderActivateAccountEmailBody(RegistrationData regData)
        {
            string template = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}ActivateAccount.html");
            string activationUrl = $"https://textport.com/account/activate/{regData.AccountValidationKey}";
            template = template.Replace("{{name}}", regData.UserName);
            template = template.Replace("{{action_url}}", activationUrl);
            template = template.Replace("{{ip_address}}", regData.IPAddress);
            template = template.Replace("{{browser_type}}", regData.BrowserType);
            template = template.Replace("{{copy_year}}", $"{DateTime.Now.Year}");

            return template;
        }

        public static string RenderVirtualNumberExpirationEmailBody(NumberExpirationData expData)
        {
            string template = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}NumberExpirationNotification.html");
            template = template.Replace("{{name}}", expData.UserName);
            template = template.Replace("{{virtual_number}}", expData.VirtualNumberDisplay);
            template = template.Replace("{{time_remaining}}", expData.ExpirationDaysAndHours.ToString());
            template = template.Replace("{{expiration_date}}", $"{expData.ExpirationDate:MM/dd/yyyy}");
            template = template.Replace("{{renewal_fee}}", $"{expData.Fee:C2}");
            template = template.Replace("{{action_url}}", expData.ActionUrl);
            template = template.Replace("{{copy_year}}", $"{DateTime.Now.Year}");

            return template;
        }

        public static string RenderAutoRenewNumberLowBalanceNotificationBody(NumberExpirationData expData)
        {
            string template = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}AutoRenewNumberLowBalanceNotification.html");
            template = template.Replace("{{name}}", expData.UserName);
            template = template.Replace("{{virtual_number}}", expData.VirtualNumberDisplay);
            template = template.Replace("{{remaining_balance}}", $"{expData.Balance:C2}");
            template = template.Replace("{{time_remaining}}", expData.ExpirationDaysAndHours.ToString());
            template = template.Replace("{{expiration_date}}", $"{expData.ExpirationDate:MM/dd/yyyy}");
            template = template.Replace("{{renewal_fee}}", $"{expData.Fee:C2}");
            template = template.Replace("{{action_url}}", expData.ActionUrl);
            template = template.Replace("{{copy_year}}", $"{DateTime.Now.Year}");

            return template;
        }
    }
}