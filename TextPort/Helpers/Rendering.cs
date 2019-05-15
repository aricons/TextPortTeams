using System;
using System.IO;
using System.Configuration;

using TextPortCore.Models;

namespace TextPort.Helpers
{
    public static class Rendering
    {
        public static string RenderMessageIn(Message msg)
        {
            string html = $"<div id=\"{ msg.MessageId}\" class=\"msg_item incoming_msg\">";
            html += $"<div class=\"incoming_msg_img\"><img src=\"~/content/images/user-profile.png\" /></div>";
            html += $"<div class=\"received_msg\"><div class=\"received_withd_msg\"><p>{msg.MessageText}</p>";
            html += $"<span class=\"time_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></div></div></div>";
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