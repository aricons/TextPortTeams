using System;
using System.Linq;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Models;

namespace TextPort.Controllers
{
    public class HomeController : Controller
    {
        private readonly TextPortContext _context;

        public HomeController(TextPortContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
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

        public ActionResult About()
        {

            try
            {
                Account acct = _context.Accounts.FirstOrDefault(x => x.AccountId == 1);
                string bar = acct.UserName;
            }
            catch (Exception ex)
            {
                string baz = ex.Message;
            }
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}