using System.Web.Mvc;

namespace TextPort.Controllers
{
    [Route("api")]
    public class SmsApiController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Setup()
        {
            return View();
        }

        [ActionName("add-application")]
        public ActionResult AddApplication()
        {
            return View();
        }

        [ActionName("assign-number")]
        public ActionResult AssignNumber()
        {
            return View();
        }

        public ActionResult Samples()
        {
            return View();
        }

        [ActionName("inbound-events")]
        public ActionResult InboundEvents()
        {
            return View();
        }

        public ActionResult Ping()
        {
            return View();
        }

        [ActionName("search-numbers")]
        public ActionResult SearchNumbers()
        {
            return View();
        }

        [ActionName("get-number")]
        public ActionResult GetNumber()
        {
            return View();
        }

        [ActionName("send-message")]
        public ActionResult SendMessage()
        {
            return View();
        }

        [ActionName("message-received")]
        public ActionResult MessageReceived()
        {
            return View();
        }

        [ActionName("delivery-receipt")]
        public ActionResult DeliveryReceipt()
        {
            return View();
        }

        [ActionName("account-balance")]
        public ActionResult AccountBalance()
        {
            return View();
        }

        [ActionName("active-numbers")]
        public ActionResult ActiveNumbers()
        {
            return View();
        }
    }
}