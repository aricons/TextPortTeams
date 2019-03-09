using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
//using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.SignalR;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;

using TextPort.Models;
using TextPort.Hubs;
using TextPortCore.Models;


namespace TextPort.Controllers
{
    public class HomeController : Controller
    {
        //private CompositeViewEngine _viewEngine;
        //private readonly IHubContext<InboundHub> _hubContext;

        ////public HomeController(ICompositeViewEngine viewEngine, IHubContext<InboundHub> hubContext)
        //public HomeController(IHubContext<InboundHub> hubContext)
        //{
        //    //_viewEngine = viewEngine;
        //    _hubContext = hubContext;
        //}

        //public HomeController()
        //{
        //    //_viewEngine = viewEngine;
        //    //_hubContext = hubContext;
        //}



        //public HomeController(IHubContext<InboundHub> hubContext)
        //{
        //    _hubContext = hubContext;
        //}

        //[AllowAnonymous]
        public ActionResult Index()
        {
            string foo = "1212";
            return View();
        }

        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        //[Authorize]
        //public async Task<ActionResult> Test()
        //{
        //    ViewData["Message"] = "Test Page";

        //    Message msg = new Message()
        //    {
        //        MessageId = 99912345,
        //        MessageText = "This is an inbound message 1",
        //        Direction = 1,
        //        TimeStamp = DateTime.Now,
        //        MobileNumber = "19492339386",
        //        Ipaddress = "0.0.0.0",
        //        AccountId = 1
        //    };

        //    MessageList msgList = new MessageList();
        //    msgList.Messages = new List<Message>();
        //    msgList.Messages.Add(msg);

        //    string messageHtml = await RenderPartialViewToString("_MessageList", msgList);
        //    await _hubContext.Clients.User("regley").SendAsync("MessageNotification", "19492339386", "19492366068", msg.MessageText, messageHtml);

        //    return View();
        //}

        public ActionResult Error()
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                LoginCredentials creds = new LoginCredentials();
                return PartialView("_Login", creds);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        private string RenderPartialViewToString(string viewName, object model)
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

            return string.Empty;
        }
    }
}
