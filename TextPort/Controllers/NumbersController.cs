using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        public JsonResult GetAreaCodeName(string areaCode, bool tollFree)
        {
            using (TextPortDA da = new TextPortDA())
            {
                string description = da.GetAreaCodeName(areaCode, tollFree);
                return Json(description, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAvailableNumbers(string areaCode, bool tollFree, int count)
        {
            List<SelectListItem> numbersItems = new List<SelectListItem>();

            using (Bandwidth bw = new Bandwidth())
            {
                List<string> numbers = bw.GetVirtualNumbersList(areaCode, count, tollFree);
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
        public ActionResult Index()
        {
            bool showExpiredNumbers = Request.QueryString["exp"] != null && Request.QueryString["exp"].Equals("1");
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            using (TextPortDA da = new TextPortDA())
            {
                Account acc = da.GetAccountById(accountId);
                if (acc.ComplimentaryNumber == (byte)ComplimentaryNumberStatus.Eligible)
                {
                    return RedirectToAction("ComplimentaryNumber", new { id = accountId.ToString() });
                }
            }

            NumbersContainer nc = new NumbersContainer(accountId, showExpiredNumbers);
            return View(nc);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetNumber()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            using (TextPortDA da = new TextPortDA())
            {
                Account acc = da.GetAccountById(accountId);
                if (acc.ComplimentaryNumber == (byte)ComplimentaryNumberStatus.Eligible)
                {
                    return RedirectToAction("ComplimentaryNumber", new { id = accountId.ToString() });
                }
            }

            RegistrationData regData = new RegistrationData("VirtualNumber", accountId);
            return View(regData);
        }

        [Authorize]
        [HttpGet]
        public ActionResult RenewNumber(int id)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            RegistrationData regData = new RegistrationData("VirtualNumberRenew", accountId);

            using (TextPortDA da = new TextPortDA())
            {
                DedicatedVirtualNumber vn = da.GetVirtualNumberById(id);
                if (vn != null)
                {
                    regData.VirtualNumber = vn.VirtualNumber;
                    regData.VirtualNumberId = vn.VirtualNumberId;
                }
            }

            return View(regData);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ComplimentaryNumber(int id)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            if (id == accountId) // Compare the id passed against teh account id to check that there's not an attempt to wrongfully access the page.
            {
                using (TextPortDA da = new TextPortDA())
                {
                    Account acc = da.GetAccountById(accountId);
                    if (acc.ComplimentaryNumber > 0)
                    {
                        RegistrationData regData = new RegistrationData("ComplimentaryNumber", accountId);
                        regData.ShowAnnouncementBanner = (acc.ComplimentaryNumber == 1);
                        return View("ComplimentaryNumber", regData);
                    }
                }
            }
            return RedirectToAction("Profile", "Account");
        }

        [Authorize]
        [HttpGet]
        public ActionResult NumberHistory(int id)
        {
            MessageHistory history = new MessageHistory(id);

            return PartialView("_NumberHistory", history);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ApplyApi(int id)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            ApiApplicationsContainer apiApps = new ApiApplicationsContainer(accountId, 0, id);

            return PartialView("_ApplyApi", apiApps);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ApplyApi(ApiApplicationsContainer apiApps)
        {
            using (TextPortDA da = new TextPortDA())
            {
                if (apiApps.CurrentApplicationId == 0)
                {
                    da.UnAssignAPIApplicationFromVirtualNumber(apiApps.VirtualNumberId);
                }
                else
                {
                    da.AssignAPIApplicationToVirtualNumber(apiApps.CurrentApplicationId, apiApps.VirtualNumberId);
                }
            }
            NumbersContainer nc = new NumbersContainer(apiApps.AccountId, false);
            return View("Index", nc);
        }
    }
}