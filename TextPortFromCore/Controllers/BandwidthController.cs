using System;
using System.IO;
using System.Threading.Tasks;
//using System.Web.Mvc;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using System.Web.Http.Results;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;

using TextPort.Hubs;
using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Models.Bandwidth;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class BandwidthController : ApiController // ControllerBase
    {
        private readonly TextPortContext _context;
        //private readonly ICompositeViewEngine _viewEngine;
        private readonly IHubContext<InboundHub> _hubContext;

        //public BandwidthController(TextPortContext context, ICompositeViewEngine viewEngine, IHubContext<InboundHub> hubContext)
        public BandwidthController(TextPortContext context, IHubContext<InboundHub> hubContext)
        {
            _context = context;
            //_viewEngine = viewEngine;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("[action]")]
        public string Ping()
        {
            return String.Format("Bandwidth API alive at {0}", DateTime.Now);
        }

        [HttpGet]
        [Route("[action]/{value}")]
        public string PingVal(string value)
        {
            return String.Format("PingVal received {0}", value);
        }

        [HttpPost]
        [Route("[action]")]
        public IHttpActionResult MessageIn([System.Web.Http.FromBody] BandwidthInboundMessage messageData)
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
                        string messageHtml = renderPartialViewToString("_MessageList", msgList);
                        //await _hubContext.Clients.User(account.UserName).SendAsync("MessageNotification", newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                    }
                }
            }

            return Ok();
        }

        //private string renderMessageToHtml(Message msg)
        //{
        //    string html = $"<div id=\"{msg.MessageId}\" class=\"msg_item incoming_msg\">";
        //    html += "<div class=\"incoming_msg_img\"><img src=\"~/images/user-profile.png\" /></div>";
        //    html += "<div class=\"received_msg\"><div class=\"received_withd_msg\">";
        //    html += $"<p>{msg.MessageText}</p>";
        //    html += $"<span class=\"time_date\">{msg.TimeStamp:MMMM dd, yy | hh:mm tt}</span></div></div></div>";

        //    return html;
        //}

        private string renderPartialViewToString(string viewName, object model)
        {
            //if (string.IsNullOrEmpty(viewName))
            //    viewName = ControllerContext.ActionDescriptor.ActionName;

            //ViewData.Model = model;

            //using (var writer = new StringWriter())
            //{
            //    ViewEngineResult viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

            //    ViewContext viewContext = new ViewContext(
            //        ControllerContext,
            //        viewResult.View,
            //        ViewData,
            //        TempData,
            //        writer,
            //        new HtmlHelperOptions()
            //    );

            //    await viewResult.View.RenderAsync(viewContext);

            //    return writer.GetStringBuilder().ToString();


            //}
            return string.Empty;
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
