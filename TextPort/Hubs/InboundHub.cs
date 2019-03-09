using System;
using Microsoft.AspNet.SignalR;

namespace TextPort.Hubs
{
    public class InboundHub : Hub
    {
        //public void Send(string name, string message)
        ////public void SendInboundMessageNotification(string userName, string fromNumber, string toNumber, string messageText, string messageHtml)
        //{
        //    Clients.All.messageNotification(name, message);
        //    //this.Clients.User(userName).messageNotification(fromNumber, toNumber, messageText, messageHtml);
        //}

        //public void SendMessage(string user, string fromNumber, string toNumber, string messageText, string messageHtml)
        //{
        //    Clients.User(user).SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        //    //await Clients.Client(user).SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        //    // await Clients.All.SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        //}
    }
}