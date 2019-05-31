using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    public class NumbersController : Controller
    {
        private readonly TextPortContext _context;

        public NumbersController(TextPortContext context)
        {
            this._context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmPurchase(RegistrationData regData)
        {
            try
            {
                return PartialView("_Purchase", regData);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAreaCodeName(string areaCode)
        {
            using (TextPortDA da = new TextPortDA())
            {
                string description = da.GetAreaCodeName(areaCode);
                return Json(description, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAvailableNumbers(string areaCode)
        {
            List<SelectListItem> numbersItems = new List<SelectListItem>();

            using (Bandwidth bw = new Bandwidth())
            {
                List<string> numbers = bw.GetVirtualNumbersList(areaCode);
                if (numbers.Any())
                {
                    foreach (string number in numbers)
                    {
                        numbersItems.Add(new SelectListItem() { Text = Utilities.NumberToDisplayFormat(number, 22), Value = number });
                    }
                }
                else
                {
                    numbersItems.Add(new SelectListItem() { Text = "No available numbers", Value = string.Empty });
                }
            }

            return Json(numbersItems, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Manage()
        {
            bool showExpiredNumbers = Request.QueryString["exp"] != null && Request.QueryString["exp"].Equals("1");
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            NumbersContainer nc = new NumbersContainer(accountId, showExpiredNumbers);
            return View(nc);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetNumber()
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            RegistrationData regData = new RegistrationData("VirtualNumber", accountId);
            return View(regData);
        }

        [Authorize]
        [HttpGet]
        public ActionResult RenewNumber(int id)
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            RegistrationData regData = new RegistrationData("VirtualNumberRenew", accountId);

            DedicatedVirtualNumber vn = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == id);
            if (vn != null)
            {
                regData.VirtualNumber = vn.VirtualNumber;
                regData.VirtualNumberId = vn.VirtualNumberId;
            }

            return View(regData);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ComplimentaryNumber()
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            RegistrationData regData = new RegistrationData("ComplimentaryNumber", accountId);

            return View("GetNumber", regData);
        }

        [Authorize]
        [HttpGet]
        public ActionResult NumberHistory(int id)
        {
            MessageHistory history = new MessageHistory(_context, id);

            return PartialView("_NumberHistory", history);
        }
    }
}