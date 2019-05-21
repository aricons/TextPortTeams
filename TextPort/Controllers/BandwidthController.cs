using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
//using System.Web.Mvc;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using System.Web.Http.Results;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;

using TextPort.Hubs;
using TextPort.Helpers;
using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Models.Bandwidth;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class BandwidthController : ApiController
    {
        private readonly TextPortContext _context;

        public BandwidthController(TextPortContext context)
        {
            this._context = context;
        }

        //private readonly ICompositeViewEngine _viewEngine;
        //private readonly IHubContext<InboundHub> _hubContext;

        //public BandwidthController(TextPortContext context, ICompositeViewEngine viewEngine, IHubContext<InboundHub> hubContext)
        //public BandwidthController(TextPortContext context, IHubContext<InboundHub> hubContext)
        //public BandwidthController(TextPortContext context)
        //{
        //    //_context = context;
        //    //_viewEngine = viewEngine;
        //    //_hubContext = hubContext;
        //}

        //private readonly static Lazy<BandwidthController> _instance = new Lazy<BandwidthController>(() => new BandwidthController(GlobalHost.ConnectionManager.GetHubContext<InboundHub>()));

        //private BandwidthController()
        //{
        //}

        //private IHubContext _hubContext;

        //private BandwidthController()
        //{
        //    _hubContext = GlobalHost.ConnectionManager.GetHubContext<InboundHub>();
        //}

        [HttpGet]
        //[Route("[action]")]
        [ActionName("ping")]
        public string Ping()
        {
            return String.Format("Bandwidth API alive at {0}", DateTime.Now);
        }

        [HttpGet]
        //[Route("[action]/{value}")]
        [ActionName("pingval")]
        public string PingVal(string value)
        {
            return String.Format("PingVal received {0}", value);
        }

        [HttpPost]
        //[Route("[action]")]
        public IHttpActionResult MessageIn([FromBody] List<BandwidthInboundMessage> messageData)
        {
            using (TextPortDA da = new TextPortDA(_context))
            {
                using (Bandwidth bw = new Bandwidth(_context))
                {
                    Message newMessage = bw.ProcessBandwidthInboundMessage(messageData);
                    MessageList msgList = new MessageList(newMessage);
                    Account account = da.GetAccountById(newMessage.AccountId);

                    if (!String.IsNullOrEmpty(account.UserName))
                    {
                        string messageHtml = Rendering.RenderMessageIn(newMessage);
                        using (HubFunctions hubFunctions = new HubFunctions())
                        {
                            hubFunctions.SendInboundMessageNotification(account.UserName, newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        }

                        //await _hubContext.Clients.User(account.UserName).SendAsync("MessageNotification", newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        //await _hubContext.Clients.All.SendAsync("MessageNotification", newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        //_hubContext.Clients.All.SendAsync("AddNewMessageToPage", "foo", "bar");

                        //_hubContext.Clients.All.addNewMessageToPage("foo", "bar"); // This works
                        //_hubContext.Clients.All.messageNotification("foo", "bar"); // This works
                        //using (InboundHub hub = new InboundHub())
                        //{
                        //    hub.SendInboundMessageNotification(account.UserName, newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        //}
                        // Line below works.
                        //_hubContext.Clients.User(account.UserName).messageNotification(newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        //_hubContext.Clients.All.messageNotification(newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        //using (HubFunctions hubFunctions = new HubFunctions())
                        //{
                        //    hubFunctions.SendInboundMessageNotification(account.UserName, newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                        //}
                    }
                }
            }
            return Ok();
        }

        // Sample inbound message JSON
        //{
        //"eventType"     : "sms",
        //"direction"     : "in",
        //"messageId"     : "12345678",
        //"messageUri"    : "https://api.catapult.inetwork.com/v1/users/{userId}/messages/{messageId}",
        //"from"          : "19492339386",
        //"to"            : "14154847947",
        //"text"          : "This is a test message inbound 3",
        //"applicationId" : "a-gp37trhojrbs2fgemvw5wzq",
        //"time"          : "2019-02-19T18:06:06.076Z",
        //"state"         : "received"
        //}

    }
}
