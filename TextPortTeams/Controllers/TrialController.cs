using System;
using System.Web.Mvc;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortTeams.Controllers
{
    public class TrialController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                //RegistrationData rd = new RegistrationData("FreeTrial", 0);
                //return View(rd);
                return RedirectToAction("signup", "account");
            }
            catch (Exception ex)
            {
                string foo = "error";
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegistrationData regData)
        {
            return RedirectToAction("signup", "account");
            //try
            //{
            //    regData.Success = false;
            //    regData.CompletionTitle = "Registration Failed";
            //    regData.CompletionMessage = "An error occurred while processing the request. We apologize fo any inconvenience. <a href=\"/home/support\">Please submit a support request to report this issue.</a>";
            //    regData.BrowserType = Request.Browser.Type;
            //    regData.IPAddress = Request.UserHostAddress;
            //    // Hard-code credit purchase amount and account enabled values to override what is sent form the form.
            //    // This is done to prevent hacks.
            //    //regData.CreditPurchaseAmount = (decimal)0.15;
            //    regData.CreditPurchaseAmount = 0;
            //    regData.AccountEnabled = false;

            //    using (TextPortDA da = new TextPortDA())
            //    {
            //        regData.AccountId = da.AddAccount(regData);
            //        if (regData.AccountId > 0)
            //        {
            //            if (!string.IsNullOrEmpty(regData.VirtualNumber))
            //            {
            //                string body = Rendering.RenderActivateAccountEmailBody(regData);
            //                using (EmailMessage message = new EmailMessage(regData.EmailAddress, "Activate your TextPort Account", body))
            //                {
            //                    if (message.Send())
            //                    {
            //                        regData.Success = true;
            //                        regData.CompletionTitle = "Account Registration Successful";
            //                        regData.CompletionMessage = "Your trial account was registered successfully.";
            //                        regData.BrowserType = Request.Browser.Type;
            //                    }
            //                    else
            //                    {
            //                        regData.Success = false;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                regData.CompletionMessage += " No number was specified.";
            //            }
            //        }
            //        else
            //        {
            //            regData.CompletionMessage += " Account creation failed.";
            //        }
            //    }
            //    return View("RegistrationComplete", regData);
            //}
            //catch (Exception ex)
            //{
            //    ErrorHandling eh = new ErrorHandling();
            //    eh.LogException("TrialController.Index_POST", ex);
            //}
            //return null;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult RegistrationComplete(RegistrationData regData)
        {
            try
            {
                //For testing layout
                //RegistrationData regData = new RegistrationData()
                //{
                //    VirtualNumber = "15055551234",
                //    CreditPurchaseAmount = 0.30M,
                //    UserName = "mytestacc",
                //    Success = true,
                //    CompletionMessage = "The number you chose could not be assigned to the account. Please try again.",
                //    CompletionTitle = "Registration Complete"
                //};
                return View(regData);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("TrialController.RegistrationComplete_GET", ex);
            }
            return null;
        }
    }
}