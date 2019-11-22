using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Web.Mvc;

using TextPort.Helpers;
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
        [HttpPost]
        public ActionResult GetNumber(RegistrationData regData)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            regData.Success = false;
            regData.Status = "Failed";
            regData.CompletionTitle = "Number Assignment Failure";
            regData.CompletionMessage = $"An error occurred while attempting to assign the number {regData.VirtualNumber} to your account. Please try again. If the problem persists please contact support.";

            if (!string.IsNullOrEmpty(regData.VirtualNumber) && accountId > 0)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    using (Bandwidth bw = new Bandwidth())
                    {
                        //if (bw.PurchaseVirtualNumber(regData))
                        if (true)
                        {
                            if (da.AddNumberToAccount(regData))
                            {
                                regData.CompletionTitle = $"{regData.NumberDisplayFormat} Successfully Assigned";
                                regData.CompletionMessage = $"The number {regData.NumberDisplayFormat} has been sucessfully assigned to your account for a period of {regData.LeasePeriod} {regData.LeasePeriodWord}.";
                                regData.Status = "Complete";
                                regData.Success = true;

                                Account acc = da.GetAccountById(regData.AccountId);
                                if (acc != null)
                                {
                                    acc.Balance -= (regData.NumberCost);
                                    da.SaveChanges();

                                    Cookies.WriteBalance(acc.Balance);
                                }
                            }
                        }
                    }
                }
            }

            return View("TransactionComplete", regData);
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
        [HttpPost]
        public ActionResult RenewNumber(RegistrationData regData)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            regData.Success = false;
            regData.CompletionTitle = "Number Renewal Failure";
            regData.CompletionMessage = $"An error occurred while attempting to renew the number {regData.VirtualNumber}. Please try again. If the problem persists please contact support.";

            if (accountId > 0 && regData.VirtualNumberId > 0 && regData.NumberCost > 0)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    DedicatedVirtualNumber vn = da.GetVirtualNumberById(regData.VirtualNumberId);
                    if (vn != null)
                    {
                        vn.ExpirationDate = vn.ExpirationDate.AddMonths(regData.LeasePeriod);
                        vn.RenewalCount = vn.RenewalCount + 1;
                        vn.Cancelled = false;
                        vn.Fee = regData.NumberCost;
                        vn.SevenDayReminderSent = null;
                        vn.TwoDayReminderSent = null;
                        da.SaveChanges();

                        regData.CompletionTitle = "Number Renewal Complete";
                        regData.CompletionMessage = $"The number {regData.NumberDisplayFormat} has been sucessfully renewed for {regData.LeasePeriod} {regData.LeasePeriodWord}.";
                        regData.Status = "Complete";
                        regData.Success = true;

                        Account acc = da.GetAccountById(regData.AccountId);
                        if (acc != null)
                        {
                            acc.Balance -= regData.NumberCost;
                            da.SaveChanges();

                            Cookies.WriteBalance(acc.Balance);
                        }
                    }
                }
            }
            return View("TransactionComplete", regData);
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