using System;

using TextPortCore.Models;

namespace TextPort.Helpers
{
    public static class Rendering
    {
        public static string RenderMessageIn(Message msg)
        {
            string html = $"<div id=\"{ msg.MessageId}\" class=\"msg_item incoming_msg\">";
            html += $"<div class=\"incoming_msg_img\"><img src=\"~/images/user-profile.png\" /></div>";
            html += $"<div class=\"received_msg\"><div class=\"received_withd_msg\"><p>{msg.MessageText}</p>";
            html += $"<span class=\"time_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></div></div></div>";
            return html;
        }
    }
}