using System;
using Microsoft.AspNet.SignalR;

namespace TextPort.Hubs
{
    public class InboundHub : Hub
    {
        //public void SendMessage(string user, string fromNumber, string toNumber, string messageText, string messageHtml)
        //{
        //    Clients.User(user).SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        //    //await Clients.Client(user).SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        //    // await Clients.All.SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        //}
    }
}