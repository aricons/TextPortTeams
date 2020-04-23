using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security;

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
        public JsonResult GetAvailableNumbers(string areaCode, bool tollFree, int count, int page)
        {
            List<SelectListItem> numbersItems = new List<SelectListItem>();

            using (Bandwidth bw = new Bandwidth())
            {
                List<string> numbers = bw.GetVirtualNumbersList(areaCode, count, tollFree, page);
                if (numbers.Any())
                {
                    if (numbers.FirstOrDefault().Equals("No more numbers"))
                    {
                        numbersItems.Add(new SelectListItem() { Text = "No more numbers", Value = string.Empty });
                        return Json(numbersItems, JsonRequestBehavior.AllowGet);
                    }

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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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
                        if (bw.PurchaseVirtualNumber(regData))
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult RenewNumber(RegistrationData regData)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            regData.Success = false;
            regData.CompletionTitle = "Number Extension Failure";
            regData.CompletionMessage = $"An error occurred while attempting to extend the lease on {regData.VirtualNumber}. Please try again. If the problem persists please contact support.";

            if (accountId > 0 && regData.VirtualNumberId > 0 && regData.NumberCost > 0)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    DedicatedVirtualNumber vn = da.GetVirtualNumberById(regData.VirtualNumberId);
                    if (vn != null)
                    {
                        DateTime startDate = vn.ExpirationDate;
                        DateTime expirationDate = DateTime.UtcNow.AddDays(7);
                        DateTime? twoDayRenewalReminderDate = null;
                        DateTime? sevenDayRenewalReminderDate = null;
                        int leasePeriod = (regData.LeasePeriod == 0) ? 1 : regData.LeasePeriod;

                        switch (regData.LeasePeriodType)
                        {
                            case "D":
                                expirationDate = startDate.AddDays(leasePeriod);
                                break;
                            case "W":
                                expirationDate = startDate.AddDays(leasePeriod * 7);
                                break;
                            case "M":
                                expirationDate = startDate.AddMonths(leasePeriod).AddHours(-4);
                                break;
                            case "Y":
                                expirationDate = startDate.AddYears(leasePeriod).AddHours(-6);
                                break;
                        }

                        double daysUntilExpiration = (expirationDate - DateTime.UtcNow).TotalDays;
                        if (daysUntilExpiration <= 4)
                        {
                            twoDayRenewalReminderDate = DateTime.UtcNow;
                            sevenDayRenewalReminderDate = DateTime.UtcNow;
                        }
                        if (daysUntilExpiration <= 10)
                        {
                            sevenDayRenewalReminderDate = DateTime.UtcNow;
                        }

                        vn.LeasePeriod = (short)leasePeriod;
                        vn.LeasePeriodType = regData.LeasePeriodType;
                        vn.ExpirationDate = expirationDate;
                        vn.RenewalCount = vn.RenewalCount + 1;
                        vn.Cancelled = false;
                        vn.Fee = regData.NumberCost;
                        vn.SevenDayReminderSent = sevenDayRenewalReminderDate;
                        vn.TwoDayReminderSent = twoDayRenewalReminderDate;
                        da.SaveChanges();

                        regData.CompletionTitle = "Number Renewal Complete";
                        regData.CompletionMessage = $"The lease for number {regData.NumberDisplayFormat} has been extended for {regData.LeasePeriod} {regData.LeasePeriodWord}. It will expire on {expirationDate:MM/dd/yyyy} at {expirationDate:hh:mm tt}";
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
        [HttpPost]
        public JsonResult SetAutoRenew(AutoRenewSettings autoRenewSettings)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            autoRenewSettings.ConfirmationTitle = "Error setting auto-extend.";
            autoRenewSettings.ConfirmationDetail = "An error occurred while updating the auto-extend settings for this number.";

            if (accountId > 0 && autoRenewSettings != null)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    DedicatedVirtualNumber dvn = da.GetVirtualNumberById(autoRenewSettings.VirtualNumberId);
                    if (dvn != null)
                    {
                        dvn.AutoRenew = autoRenewSettings.AutoRenew;
                        int rowsUpdated = da.SaveChanges();

                        if (rowsUpdated > 0)
                        {
                            if (autoRenewSettings.AutoRenew)
                            {
                                autoRenewSettings.ConfirmationTitle = "Auto-Extend Enabled";
                                autoRenewSettings.ConfirmationDetail = $"<ul><li>Your lease for {dvn.NumberDisplayFormat} will be automatically extended on a monthly basis starting {dvn.ExpirationDate:MM/dd/yyyy}. A {Constants.MonthlyNumberRenewalCost:C2} fee will be debited from your TextPort balance.</li><li>The number will continue to be extended monthly provided your account carries a sufficient balnce.</li><li>You will be notified by email if your balance is not sufficient to cover an upcoming extension renewal.</li><li>You may disable auto-extend at any time by unchecking the auto-extend box.</li></ul>";
                            }
                            else
                            {
                                autoRenewSettings.ConfirmationTitle = "Auto-Extend Disabled";
                                autoRenewSettings.ConfirmationDetail = $"<ul><li>Auto-Extend has been disabled for number {dvn.NumberDisplayFormat}.</li><li>The number will auto-expire on {dvn.ExpirationDate:MM/dd/yyyy}.</li><li>To enable auto-extend, check the auto-extend box.</li></ul>";
                            }
                        }
                        else
                        {
                            autoRenewSettings.ConfirmationTitle = "Error setting auto-extend.";
                            autoRenewSettings.ConfirmationDetail = $"An error occurred while attempting to update the auto-extend setting on this number {dvn.NumberDisplayFormat}";
                        }
                    }
                }
            }

            return Json(autoRenewSettings, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult NumberHistory(int id)
        {
            MessageHistory history = new MessageHistory(id);

            return PartialView("_NumberHistory", history);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult ApplyApi(int id)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            ApiApplicationsContainer apiApps = new ApiApplicationsContainer(accountId, 0, id);

            return PartialView("_ApplyApi", apiApps);
        }

        [Authorize(Roles = "User")]
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Renew(string id)
        {
            List<int> parameterValues = RandomString.ExtractJDelimitedValues(id);
            if (parameterValues != null && parameterValues.Count >= 2)
            {
                int accountId = parameterValues[0];
                int virtualNumberId = parameterValues[1];

                if (accountId > 0 && virtualNumberId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        Account account = da.GetAccountById(parameterValues[0]);
                        if (account != null)
                        {
                            // As an extra security measure, get the virtual number record and check that the
                            // account Id assigned to the virtual number is the same as the account ID that was
                            // passed in as the account ID parameter. This requires that both the account ID parameter
                            // and the virtual number ID parameter are both associated with the same account.
                            DedicatedVirtualNumber dvn = da.GetVirtualNumberById(virtualNumberId);
                            if (dvn != null)
                            {
                                if (dvn.AccountId == accountId)
                                {
                                    List<Claim> claims = new List<Claim> {
                                        new Claim("AccountId", account.AccountId.ToString(), ClaimValueTypes.Integer),
                                        new Claim(ClaimTypes.Name, account.UserName.ToString()),
                                        new Claim(ClaimTypes.NameIdentifier, account.UserName.ToString()),
                                        new Claim(ClaimTypes.Email, account.Email.ToString()),
                                        new Claim(ClaimTypes.Role, "User")
                                    };

                                    ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                                    var context = Request.GetOwinContext();
                                    var authManager = context.Authentication;

                                    authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                                    Cookies.Write("balance", account.Balance.ToString(), 0);

                                    return RedirectToAction("Index", "Numbers");
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}