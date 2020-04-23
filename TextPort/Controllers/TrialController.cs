using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.Owin.Security;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class TrialController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                RegistrationData rd = new RegistrationData("FreeTrial", 0);
                return View(rd);
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
            try
            {
                regData.Success = false;
                regData.CompletionTitle = "Registration Failed";
                regData.CompletionMessage = "An error occurred while processing the request. We apologize fo any inconvenience. <a href=\"/home/support\">Please submit a support request to report this issue.</a>";
                regData.BrowserType = Request.Browser.Type;
                regData.IPAddress = Request.UserHostAddress;

                using (TextPortDA da = new TextPortDA())
                {
                    regData.AccountId = da.AddAccount(regData);
                    if (regData.AccountId > 0)
                    {
                        if (!string.IsNullOrEmpty(regData.VirtualNumber))
                        {
                            string body = Rendering.RenderActivateAccountEmailBody(regData);
                            using (EmailMessage message = new EmailMessage(regData.EmailAddress, "Activate your TextPort Account", body))
                            {
                                if (message.Send())
                                {
                                    regData.Success = true;
                                    regData.CompletionTitle = "Account Registration Successful";
                                    regData.CompletionMessage = "Your trial account was registered successfully.";
                                    regData.BrowserType = Request.Browser.Type;
                                }
                                else
                                {
                                    regData.Success = false;
                                }
                            }

                            //// Log the user in
                            //List<Claim> claims = new List<Claim> {
                            //                new Claim("AccountId", regData.AccountId.ToString(), ClaimValueTypes.Integer),
                            //                new Claim(ClaimTypes.Name, regData.UserName.ToString()),
                            //                new Claim(ClaimTypes.Email, regData.EmailAddress.ToString()),
                            //                new Claim(ClaimTypes.Role, "User") };

                            //ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                            //var context = Request.GetOwinContext();
                            //var authManager = context.Authentication;

                            //authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                            //if (da.AddNumberToAccount(regData))
                            //{
                            //    Cookies.WriteBalance(regData.CreditPurchaseAmount);
                            //    regData.CompletionTitle = "Registration Complete";
                            //    regData.Success = true;
                            //}
                            //else
                            //{
                            //    regData.CompletionMessage += " The number was unable to be assigned to your account.";
                            //}
                        }
                        else
                        {
                            regData.CompletionMessage += " No number was specified.";
                        }
                    }
                    else
                    {
                        regData.CompletionMessage += " Account creation failed.";
                    }
                }
                return View("RegistrationComplete", regData);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("TrialController.Index_POST", ex);
            }
            return null;
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