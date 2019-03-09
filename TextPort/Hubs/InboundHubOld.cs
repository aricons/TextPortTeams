using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace TextPort.Hubs
{
    public class InboundHubOld : Hub
    {

        //public override Task OnConnectedAsync()
        //{
        //    //var connectionId = Context.ConnectionId;
        //    //string foo = connectionId.ToString();

        //    //string userName = Context.UserIdentifier;
        //    //foo = userName;
        //    //var username = Context.QueryString["username"]; //here you will receive naveed as username

        //    return base.OnConnectedAsync();
        //}

        public async Task SendMessage(string user, string fromNumber, string toNumber, string messageText, string messageHtml)
        {
            await Clients.User(user).SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
            //await Clients.Client(user).SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
            // await Clients.All.SendAsync("MessageNotification", fromNumber, toNumber, messageText, messageHtml);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
