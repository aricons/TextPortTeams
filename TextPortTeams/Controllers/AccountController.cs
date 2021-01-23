using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Coinbase;
using TextPortCore.Integrations.Bandwidth;

namespace TextPortTeams.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ValidateLogin(LoginCredentials model)
        {
            Account account = null;
            var result = new { success = "false", response = "Unable to validate credentials." };

            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    if (da.ValidateLogin(model.UserNameOrEmail, model.LoginPassword, ref account))
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

                        if (account.ComplimentaryNumber == (byte)ComplimentaryNumberStatus.Eligible)
                        {
                            result = new { success = "true", response = Url.Action($"ComplimentaryNumber/{account.AccountId}", "Numbers") };
                        }
                        else
                        {
                            result = new { success = "true", response = Url.Action("index", "messages") };
                        }
                    }
                    else
                    {
                        result = new { success = "false", response = "Invalid username, email or password" };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(result);
        }

        [Authorize(Roles = "User")]
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            try
            {
                RegistrationData rd = new RegistrationData("VirtualNumberSignUp", 0);
                string virtualNumber = Request.QueryString["vn"];
                string leasePeriod = Request.QueryString["p"];
                string leasePeriodParam = string.Empty;

                rd.VirtualNumber = (!string.IsNullOrEmpty(virtualNumber)) ? virtualNumber : string.Empty;
                if (!string.IsNullOrEmpty(leasePeriod) && leasePeriod.Length == 2)
                {
                    leasePeriodParam = $"{leasePeriod.Substring(0, 1)}|{leasePeriod.Substring(1, 1)}";

                    foreach (SelectListItem perItem in rd.LeasePeriodsList)
                    {
                        if (perItem.Value.StartsWith(leasePeriodParam))
                        {
                            rd.LeasePeriodCode = perItem.Value;
                            break;
                        }
                    }
                }

                return View(rd);
            }
            catch (Exception)
            {
                string foo = "error";
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PrePurchase(RegistrationData regData)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    switch (regData.PurchaseType)
                    {
                        case "VirtualNumberSignUp":
                        case "FreeTrial":
                            // Add a temporary account (Enabled flag set to 0).
                            regData.AccountId = da.AddTemporaryAccount(regData);
                            break;
                    }

                    switch (regData.PurchaseMethod)
                    {
                        case "Crypto":
                            using (Coinbase coinbase = new Coinbase())
                            {
                                ChargeResponse chargeResponse = coinbase.CreateCharge(regData);
                                if (chargeResponse != null)
                                {
                                    regData.PaymentUrl = chargeResponse.data.hosted_url;
                                    regData.PaymentTransactionId = chargeResponse.data.id;

                                    da.UpdateAccountCryptoTransactionDetails(regData);
                                }
                            }
                            return PartialView("_PurchaseCrypto", regData);

                        default:
                            return PartialView("_Purchase", regData);
                    }
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }

            return PartialView("_PurchaseFailed", regData);
        }

        [AllowAnonymous]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PostPurchase([System.Web.Http.FromBody] RegistrationData regData)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    switch (regData.PurchaseType)
                    {
                        case "VirtualNumberSignUp":
                            regData.CompletionTitle = "Operation Failed";
                            regData.CompletionMessage = "An error occurred while processing the request. We apologize fo any inconvenience. <a href=\"/home/support\">Please submit a support request to report this issue.</a>";

                            if (da.EnableTemporaryAccount(regData))
                            {
                                if (!string.IsNullOrEmpty(regData.VirtualNumber))
                                {
                                    // Log the user in
                                    List<Claim> claims = new List<Claim> {
                                            new Claim("AccountId", regData.AccountId.ToString(), ClaimValueTypes.Integer),
                                            new Claim(ClaimTypes.Name, regData.UserName.ToString()),
                                            new Claim(ClaimTypes.Email, regData.EmailAddress.ToString()),
                                            new Claim(ClaimTypes.Role, "User") };

                                    ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                                    var context = Request.GetOwinContext();
                                    var authManager = context.Authentication;

                                    authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                                    using (Bandwidth bw = new Bandwidth())
                                    {
                                        if (bw.PurchaseVirtualNumber(regData))
                                        {
                                            if (da.AddNumberToAccount(regData))
                                            {
                                                regData.CompletionTitle = "Registration Complete";
                                                regData.CompletionMessage = "Your account and number were successfully registered.";

                                                Account acc = da.GetAccountById(regData.AccountId);
                                                if (acc != null)
                                                {
                                                    acc.Balance += (Constants.InitialBalanceAllocation + regData.CreditPurchaseAmount);
                                                    da.SaveChanges();

                                                    if (regData.CreditPurchaseAmount > 0)
                                                    {
                                                        regData.CompletionMessage += $" {regData.CreditPurchaseAmount:C} was applied to your account.";
                                                    }

                                                    Cookies.WriteBalance(acc.Balance);

                                                    return PartialView("_RegistrationComplete", regData);
                                                }
                                            }
                                            else
                                            {
                                                regData.CompletionMessage += " The number was unable to be assigned to your account.";
                                            }
                                        }
                                        else
                                        {
                                            regData.CompletionTitle = "Registration Partially Complete";
                                            regData.CompletionMessage = $"Your account was registered, but there was a problem assigning a number to your account. <a href=\"/numbers/complimentarynumber/{regData.AccountId}\">Click here to select a new number.</a> You will not be charged for the replacement number.";

                                            da.SetComplimentaryNumberFlag(regData.AccountId, ComplimentaryNumberStatus.FailureEligible);

                                            return PartialView("_RegistrationComplete", regData);
                                        }
                                    }
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
                            break;

                        case "ComplimentaryNumber":
                            regData.CompletionTitle = "Number assignment failed";
                            regData.CompletionMessage = $"An error occurred while assigning your number. <a href=\"/numbers/complimentarynumber/{regData.AccountId}\">Click here to select a new number.</a> You will not be charged for the replacement number. ";

                            if (!string.IsNullOrEmpty(regData.VirtualNumber))
                            {
                                using (Bandwidth bw = new Bandwidth())
                                {
                                    if (bw.PurchaseVirtualNumber(regData))
                                    {
                                        if (da.AddNumberToAccount(regData))
                                        {
                                            regData.CompletionTitle = "Number Successfully Assigned";
                                            regData.CompletionMessage = $"The number {regData.NumberDisplayFormat} has been sucessfully assigned to your account.";

                                            da.SetComplimentaryNumberFlag(regData.AccountId, ComplimentaryNumberStatus.Claimed);

                                            Account acc = da.GetAccountById(regData.AccountId);
                                            if (acc != null)
                                            {
                                                acc.Balance += (Constants.InitialBalanceAllocation);
                                                da.SaveChanges();

                                                // For migration. Apply the new virtualnumber ID to all existing messages where the virtual number ID is 0.
                                                da.ApplyVirtualNumberIdToAllMessages(acc.AccountId, regData.VirtualNumberId);

                                                Cookies.WriteBalance(acc.Balance);
                                            }

                                            return PartialView("_RegistrationComplete", regData);
                                        }
                                    }
                                    else
                                    {
                                        da.SetComplimentaryNumberFlag(regData.AccountId, ComplimentaryNumberStatus.FailureEligible);
                                    }
                                }
                            }
                            break;

                        case "VirtualNumberRenew":
                            if (!string.IsNullOrEmpty(regData.VirtualNumber))
                            {
                                DedicatedVirtualNumber vn = da.GetVirtualNumberById(regData.VirtualNumberId);
                                if (vn != null)
                                {
                                    vn.ExpirationDate = vn.ExpirationDate.AddMonths(regData.LeasePeriod);
                                    vn.RenewalCount = vn.RenewalCount + 1;
                                    vn.Fee = regData.TotalCost;
                                    vn.SevenDayReminderSent = null;
                                    vn.TwoDayReminderSent = null;
                                    da.SaveChanges();

                                    regData.CompletionTitle = "Number Renewal Complete";
                                    regData.CompletionMessage = $"The number {regData.NumberDisplayFormat} has been sucessfully renewed for {regData.LeasePeriod} {regData.LeasePeriodWord}.";

                                    if (regData.CreditPurchaseAmount > 0)
                                    {
                                        Account acc = da.GetAccountById(regData.AccountId);
                                        if (acc != null)
                                        {
                                            acc.Balance += (regData.CreditPurchaseAmount);
                                            da.SaveChanges();

                                            if (regData.CreditPurchaseAmount > 0)
                                            {
                                                regData.CompletionMessage += $" {regData.CreditPurchaseAmount:C} was applied to your account.";
                                            }

                                            Cookies.WriteBalance(acc.Balance);
                                        }
                                    }
                                    return PartialView("_RegistrationComplete", regData);
                                }
                            }
                            break;

                        case "Credit":
                            regData.CompletionTitle = "Credit purchase failed";
                            regData.CompletionMessage = "An error occurred while assigning credit to your account. If the credit is not reflected on your account, <a href=\"/home/support\">please submit a support request.</a>";

                            if (regData != null)
                            {
                                if (regData.AccountId > 0)
                                {
                                    AccountView av = new AccountView(regData.AccountId);
                                    Account acc = da.GetAccountById(regData.AccountId);
                                    if (acc != null)
                                    {
                                        acc.Balance += Convert.ToDecimal(regData.CreditPurchaseAmount);
                                        da.SaveChanges();

                                        Cookies.WriteBalance(acc.Balance);
                                    }
                                }

                                regData.CompletionTitle = "Credit Purchase Complete";
                                regData.CompletionMessage = $"{regData.CreditPurchaseAmount:C2} credit was sucessfully added to your account";
                            }
                            return PartialView("_RegistrationComplete", regData);
                    }
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }

            return PartialView("_RegistrationFailed", regData);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Profile()
        {
            string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            AccountView av = new AccountView(Convert.ToInt32(accountId));
            if (av != null)
            {
                return View(av);
            }
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(AccountView av)
        {
            av.Status = RequestStatus.Failed;

            using (TextPortDA da = new TextPortDA())
            {
                if (da.UpdateAccount(av.Account))
                {
                    av.Status = RequestStatus.Success;
                    av.ConfirmationMessage = "Your settings have been updated.";
                }
                else
                {
                    av.ConfirmationMessage = "There was a problem updating your settings.";
                }
                av.TimeZones = da.GetTimeZones();

                return View(av);
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Balance()
        {
            string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            RegistrationData regData = new RegistrationData("Credit", Convert.ToInt32(accountId));
            if (regData != null)
            {
                return View(regData);
            }
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult ProfileRegComplete()
        {
            string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            AccountView av = new AccountView(Convert.ToInt32(accountId));
            if (av != null)
            {
                return View("Profile", av);
            }
            return View();
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult VerifyUsername(string username)
        {
            using (TextPortDA da = new TextPortDA())
            {
                if (!da.IsUsernameAvailable(username))
                {
                    return Json($"The username {username} is already registered.", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult VerifyEmail(string emailaddress)
        {
            using (TextPortDA da = new TextPortDA())
            {
                if (!da.IsEmailAvailable(emailaddress))
                {
                    return Json($"Email {emailaddress} is already registered to an account.", JsonRequestBehavior.AllowGet);
                }

                // Check to see if the user is attempting to use a temporary email domain to get a free account to send spam texts.
                string emailDomain = da.DoesEmailContainBadDomain(emailaddress);
                if (!string.IsNullOrEmpty(emailDomain))
                {
                    return Json($"The email domain {emailDomain} cannot be accepted for trial registrations.", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Activate(string id)
        {
            ActivateAccountRequest actReq = new ActivateAccountRequest(id);
            actReq.Success = false;
            actReq.CompletionTitle = "Account Activation Failed";

            if (!string.IsNullOrEmpty(actReq.ActivationKey))
            {
                using (TextPortDA da = new TextPortDA())
                {
                    Account acc = da.GetAccountByAccountValidationKey(actReq.ActivationKey);
                    if (acc != null)
                    {
                        if (!acc.Enabled && !acc.AccountValidated)
                        {
                            actReq.AccountId = acc.AccountId;
                            actReq.UserName = acc.UserName;
                            actReq.EmailAddress = acc.Email;
                            actReq.VirtualNumber = acc.RegistrationVirtualNumber;

                            if (da.SetAccountActivationAndEnabledFlags(acc.AccountId, true))
                            {
                                // Log the user in
                                List<Claim> claims = new List<Claim> {
                                        new Claim("AccountId", actReq.AccountId.ToString(), ClaimValueTypes.Integer),
                                        new Claim(ClaimTypes.Name, actReq.UserName.ToString()),
                                        new Claim(ClaimTypes.Email, actReq.EmailAddress.ToString()),
                                        new Claim(ClaimTypes.Role, "User") };

                                ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                                var context = Request.GetOwinContext();
                                var authManager = context.Authentication;

                                authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                                if (da.AddTrialNumberToAccount(actReq))
                                {
                                    actReq.Success = true;
                                    actReq.CompletionTitle = "Account Successfully Activated!";
                                    actReq.CreditAmount = Constants.InitialFreeTrialBalanceAllocation;
                                    Cookies.WriteBalance(actReq.CreditAmount);
                                }
                                else
                                {
                                    actReq.CompletionMessage += " The number was unable to be assigned to your account.";
                                }
                            }
                        }
                        else
                        {
                            actReq.CompletionTitle = "Account Already Activated";
                            actReq.CompletionMessage = $"The activation key {actReq.ActivationKey} has already been used to activate an account with username {acc.UserName}. It is not necessary to re-activate the account.";
                        }
                    }
                    else
                    {
                        actReq.CompletionMessage = "Invalid activation token.";
                    }
                }
            }
            else
            {
                actReq.CompletionMessage = "Missing activation token.";
            }
            return View(actReq);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordRequest request = new ForgotPasswordRequest();
            return View(request);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword([System.Web.Http.FromBody] ForgotPasswordRequest request)
        {
            request.Status = RequestStatus.Failed;

            using (TextPortContext ctxt = new TextPortContext())
            {
                Account acc = ctxt.Accounts.FirstOrDefault(x => x.Email == request.EmailAddress);
                if (acc != null)
                {
                    request.UserName = acc.UserName;
                    request.BrowserType = Request.Browser.Type;
                    request.IPAddress = Request.UserHostAddress;

                    string passwordResetToken = RandomString.GenerateRandomToken(30);
                    request.ResetUrl = $"http://textport.com/account/resetpassword/{passwordResetToken}";

                    acc.PasswordResetToken = passwordResetToken;
                    ctxt.SaveChanges();

                    string body = Rendering.RenderForgotPasswordEmailBody(request);
                    using (EmailMessage message = new EmailMessage(request.EmailAddress, "TextPort Password Reset", body))
                    {
                        if (message.Send())
                        {
                            request.Status = RequestStatus.Success;
                            request.ConfirmationMessage = $"An email has been sent to {request.EmailAddress}. Please check your inbox for a link to reset your password.";
                        }
                        else
                        {
                            request.ConfirmationMessage = $"There was a problem trying to send a password reset notification to {request.EmailAddress}. The request wss not sent.";
                        }
                    }
                }
                else
                {
                    request.ConfirmationMessage = $"The email address {request.EmailAddress} was not found.";
                }
            }
            return View(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {
            ForgotPasswordRequest fpr = new ForgotPasswordRequest();
            fpr.Status = RequestStatus.Failed;

            if (!string.IsNullOrEmpty(id))
            {
                using (TextPortContext ctxt = new TextPortContext())
                {
                    Account acc = ctxt.Accounts.FirstOrDefault(x => x.PasswordResetToken == id);
                    if (acc != null)
                    {
                        fpr.Status = RequestStatus.Pending;
                        fpr.AccountId = acc.AccountId;
                        fpr.UserName = acc.UserName;
                    }
                    else
                    {
                        fpr.ConfirmationMessage = "Invalid reset token.";
                    }
                }
            }
            else
            {
                fpr.ConfirmationMessage = "Missing reset token.";
            }
            return View(fpr);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword([System.Web.Http.FromBody] ForgotPasswordRequest request)
        {
            request.Status = RequestStatus.Failed;
            if (!string.IsNullOrEmpty(request.Password) && !string.IsNullOrEmpty(request.ConfirmPassword))
            {
                using (TextPortContext ctxt = new TextPortContext())
                {
                    Account acc = ctxt.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
                    if (acc != null)
                    {
                        try
                        {
                            acc.Password = AESEncryptDecrypt.Encrypt(request.Password, TextPortCore.Helpers.Constants.RC4Key);
                            int changes = ctxt.SaveChanges();
                            if (changes > 0)
                            {
                                request.Status = RequestStatus.Success;
                                request.ConfirmationMessage = "Your password was successfully reset. ";
                            }
                            else
                            {
                                request.ConfirmationMessage = "An error occurred while resetting the password. The password reset failed.";
                            }
                        }
                        catch (Exception)
                        {
                            request.ConfirmationMessage = "An error occurred while resetting the password. The password reset failed.";
                        }
                    }
                    else
                    {
                        request.ConfirmationMessage = "An account was not found. The password reset failed.";
                    }
                }
            }
            else
            {
                request.ConfirmationMessage = "A password and password confirmation were not entered.";
            }
            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult ChangePassword()
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            request.AccountId = Convert.ToInt32(accountId);
            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([System.Web.Http.FromBody] ChangePasswordRequest request)
        {
            request.Status = RequestStatus.Failed;
            if (!string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.NewPassword) && !string.IsNullOrEmpty(request.ConfirmPassword))
            {
                using (TextPortContext ctxt = new TextPortContext())
                {
                    Account acc = ctxt.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
                    if (acc != null)
                    {
                        try
                        {
                            string oldPasswordFromDB = AESEncryptDecrypt.Decrypt(acc.Password, TextPortCore.Helpers.Constants.RC4Key);
                            if (oldPasswordFromDB == request.OldPassword)
                            {
                                acc.Password = AESEncryptDecrypt.Encrypt(request.NewPassword, TextPortCore.Helpers.Constants.RC4Key);
                                int changes = ctxt.SaveChanges();
                                if (changes > 0)
                                {
                                    request.Status = RequestStatus.Success;
                                    request.ConfirmationMessage = "Your password was successfully changed. ";
                                }
                                else
                                {
                                    request.ConfirmationMessage = "An error occurred while resetting the password. The password reset failed.";
                                }
                            }
                            else
                            {
                                request.ConfirmationMessage = "The old password entered is incorrect. The password reset failed.";
                            }
                        }
                        catch (Exception)
                        {
                            request.ConfirmationMessage = "An error occurred while resetting the password. The password reset failed.";
                        }
                    }
                    else
                    {
                        request.ConfirmationMessage = "An account was not found. The password reset failed.";
                    }
                }
            }
            else
            {
                request.ConfirmationMessage = "An old password, new password or password confirmation were not entered.";
            }
            return View(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TopUp(string id)
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

                                    return RedirectToAction("Balance", "Account");
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PasswordResetEmail()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Test()
        {
            return View();
        }
    }
}