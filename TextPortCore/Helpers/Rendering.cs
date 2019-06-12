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
            string rHtml = "<div class=\"chat_people\"><div class=\"chat_img\"><img src=\"/content/images/user-profile.png\"></div><div class=\"chat_ib\">";
            rHtml += $"<h5>{Utilities.NumberToDisplayFormat(msg.MobileNumber, 22)}<span class=\"chat_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></h5>";
            rHtml += $"<p>{msg.MessageText}</p></div></div>";

            // Messge list item
            string mHtml = $"<div id=\"{msg.MessageId}\" class=\"msg_item incoming_msg\">";
            mHtml += $"<div class=\"incoming_msg_img\"><img src=\"/content/images/user-profile.png\" /></div>";
            mHtml += $"<div class=\"received_msg\"><div class=\"received_withd_msg\">";
            foreach (MMSFile mms in msg.MMSFiles)
            {
                mHtml += $"<div><img src=\"{MMSUtils.GetMMSFileURL(msg.AccountId, mms.StorageId, mms.FileName)}\" alt = \"@mms.FileName\" /></div>";
            }
            mHtml += $"<p>{msg.MessageText}</p>";
            mHtml += $"<span class=\"time_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></div></div></div>";

            return new NotificationHtmls(rHtml, mHtml);
        }

        public static string RenderMessageInEmail(Message msg)
        {
            string html = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}InboundMessageNotification.html");
            html = html.Replace("{{name}}", msg.Account.UserName);
            html = html.Replace("{{time}}", $"{TimeFunctions.GetUsersLocalTime(msg.TimeStamp, msg.Account.TimeZoneId):MM-dd-yyyy hh:mm tt}");
            html = html.Replace("{{to_number}}", msg.VirtualNumber);
            html = html.Replace("{{from_number}}", msg.MobileNumber);
            html = html.Replace("{{message}}", msg.MessageText);

            return html;
        }

        public static string RenderForgotPasswordEmailBody(ForgotPasswordRequest req)
        {
            string template = File.ReadAllText($"{ConfigurationManager.AppSettings["EmailTemplatesFolder"]}ResetPassword.html");
            template = template.Replace("{{name}}", req.UserName);
            template = template.Replace("{{action_url}}", req.ResetUrl);
            template = template.Replace("{{ip_address}}", req.IPAddress);
            template = template.Replace("{{browser_type}}", req.BrowserType);

            return template;
        }
    }
}