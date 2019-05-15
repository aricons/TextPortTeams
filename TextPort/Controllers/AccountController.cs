using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    public class AccountController : Controller
    {
        private TextPortContext _context;

        public AccountController(TextPortContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult ValidateLogin(LoginCredentials model)
        {
            Account account = null;
            var result = new { success = "false", response = "Unable to validate credentials." };

            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    if (da.ValidateLogin(model.UserNameOrEmail, model.LoginPassword, ref account))
                    {
                        List<Claim> claims = new List<Claim> {
                        new Claim("AccountId", account.AccountId.ToString(), ClaimValueTypes.Integer),
                        new Claim(ClaimTypes.Name, account.UserName.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, account.UserName.ToString()),
                        new Claim(ClaimTypes.Email, account.Email.ToString()),
                        new Claim(ClaimTypes.Role, "User") };

                        ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        var context = Request.GetOwinContext();
                        var authManager = context.Authentication;

                        authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                        if (account.ComplimentaryNumber)
                        {
                            result = new { success = "true", response = Url.Action("ComplimentaryNumber", "Numbers") };
                        }
                        else
                        {
                            result = new { success = "true", response = Url.Action("Main", "Messages") };
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

        [Authorize]
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
                RegistrationData rd = new RegistrationData(_context, "VirtualNumberSignUp", 0);

                // For testing
                rd.UserName = "regley1";
                rd.Password = "Zealand!4";
                rd.ConfirmPassword = "Zealand!4";
                rd.EmailAddress = "richardtester@egleytest.com";

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
        public ActionResult PrePurchase(RegistrationData regData)
        {
            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    //regData.PurchaseTitle += $"Cost {regData.TotalCost}";
                    switch (regData.PurchaseType)
                    {
                        case "VirtualNumberSignUp":
                            // Add a temporary account (Enabled flag set to 0).
                            regData.AccountId = da.AddTemporaryAccount(regData);
                            //return PartialView("_Purchase", regData);
                            break;
                    }
                    return PartialView("_Purchase", regData);
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                //return null;
            }

            return PartialView("_PurchaseFailed", regData);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostPurchase([System.Web.Http.FromBody] RegistrationData regData)
        {
            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    switch (regData.PurchaseType)
                    {
                        case "VirtualNumberSignUp":
                            if (da.EnableTemporaryAccount(regData))
                            {
                                if (da.AddNumberToAccount(regData))
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

                                    if (!string.IsNullOrEmpty(regData.VirtualNumber))
                                    {
                                        using (Bandwidth bw = new Bandwidth(_context))
                                        {
                                            //bw.PurchaseVirtualNumber(regData);
                                        }
                                    }

                                    regData.CompletionTitle = "Registration Complete";
                                    regData.CompletionMessage = "Your account and number were successfully registered.";

                                    return PartialView("_RegistrationComplete", regData);
                                }
                            }
                            break;

                        case "VirtualNumber":
                            if (!string.IsNullOrEmpty(regData.VirtualNumber))
                            {
                                using (Bandwidth bw = new Bandwidth(_context))
                                {
                                    if (bw.PurchaseVirtualNumber(regData))
                                    {
                                        if (da.AddNumberToAccount(regData))
                                        {
                                            regData.CompletionTitle = "Number Successfully Assigned";
                                            regData.CompletionMessage = $"The number {regData.VirtualNumber} has been sucessfully assigned to your account.";
                                        }
                                    }
                                }
                            }

                            return PartialView("_RegistrationComplete", regData);

                        case "ComplimentaryNumber":
                            if (!string.IsNullOrEmpty(regData.VirtualNumber))
                            {
                                using (Bandwidth bw = new Bandwidth(_context))
                                {
                                    if (bw.PurchaseVirtualNumber(regData))
                                    {
                                        if (da.AddNumberToAccount(regData))
                                        {
                                            regData.CompletionTitle = "Number Successfully Assigned";
                                            regData.CompletionMessage = $"The number {regData.VirtualNumber} has been sucessfully assigned to your account.";
                                        }
                                    }
                                }
                            }

                            return PartialView("_RegistrationComplete", regData);

                        case "VirtualNumberRenew":
                            if (!string.IsNullOrEmpty(regData.VirtualNumber))
                            {
                                DedicatedVirtualNumber vn = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == regData.VirtualNumberId);
                                if (vn != null)
                                {
                                    vn.ExpirationDate = vn.ExpirationDate.AddMonths(regData.LeasePeriod);
                                    vn.RenewalCount = vn.RenewalCount + 1;
                                    vn.Fee = regData.TotalCost;
                                    vn.SevenDayReminderSent = null;
                                    vn.TwoDayReminderSent = null;
                                    _context.SaveChanges();
                                }
                            }

                            regData.CompletionTitle = "Number Renewal Complete";
                            regData.CompletionMessage = $"The number {regData.VirtualNumber} has been sucessfully renewed for {regData.LeasePeriod} {regData.LeasePeriodWord}.";

                            return PartialView("_RegistrationComplete", regData);

                        case "Credits":
                            if (regData != null)
                            {
                                string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
                                AccountView av = new AccountView(_context, Convert.ToInt32(accountId));

                                Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == Convert.ToInt32(accountId));
                                if (acc != null)
                                {
                                    acc.Credits += Convert.ToInt32(regData.CreditPurchaseAmount);
                                    _context.SaveChanges();
                                }
                            }

                            regData.CompletionTitle = "Credit PurchaseComplete";
                            regData.CompletionMessage = $"{regData.CreditPurchaseAmount:C2} credit was sucessfully added to your account";

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

        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            AccountView av = new AccountView(_context, Convert.ToInt32(accountId));
            if (av != null)
            {
                return View(av);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Balance()
        {
            string accountId = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            RegistrationData regData = new RegistrationData(_context, "Credits", Convert.ToInt32(accountId));
            if (regData != null)
            {
                return View(regData);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ProfileRegComplete()
        {
            string accountId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            AccountView av = new AccountView(_context, Convert.ToInt32(accountId));
            if (av != null)
            {
                return View("Profile", av);
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Profile(AccountView av)
        {
            using (TextPortDA da = new TextPortDA(_context))
            {
                da.UpdateAccount(av.Account);
                av.TimeZones = da.GetTimeZones();

                return View(av);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult VerifyUsername(string username)
        {
            using (TextPortDA da = new TextPortDA(_context))
            {
                if (!da.IsUsernameAvailable(username))
                {
                    return Json($"The username {username} is already taken.", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult VerifyEmail(string email)
        {
            using (TextPortDA da = new TextPortDA(_context))
            {
                if (!da.IsUsernameAvailable(email))
                {
                    return Json($"Email {email} is already registered to an account.", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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
        //[ValidateAntiForgeryToken]
        public ActionResult ForgotPassword([System.Web.Http.FromBody] ForgotPasswordRequest request)
        {
            request.Status = RequestStatus.Failed;

            Account acc = _context.Accounts.FirstOrDefault(x => x.Email == request.EmailAddress);
            if (acc != null)
            {
                request.UserName = acc.UserName;
                request.BrowserType = Request.Browser.Type;
                request.IPAddress = Request.UserHostAddress;

                string passwordResetToken = RandomString.GenerateRandomToken(30);
                request.ResetUrl = $"http://textport.com/account/resetpassword/{passwordResetToken}";

                acc.PasswordResetToken = passwordResetToken;
                _context.SaveChanges();

                string body = Rendering.RenderForgotPasswordEmailBody(request);
                using (EmailMessage message = new EmailMessage(_context, request.EmailAddress, "TextPort Password Reset", body))
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
                Account acc = _context.Accounts.FirstOrDefault(x => x.PasswordResetToken == id);
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
            else
            {
                fpr.ConfirmationMessage = "Missing reset token.";
            }
            return View(fpr);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword([System.Web.Http.FromBody] ForgotPasswordRequest request)
        {
            request.Status = RequestStatus.Failed;
            if (!string.IsNullOrEmpty(request.Password) && !string.IsNullOrEmpty(request.ConfirmPassword))
            {
                Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
                if (acc != null)
                {
                    try
                    {
                        acc.Password = AESEncryptDecrypt.Encrypt(request.Password, TextPortCore.Helpers.Constants.RC4Key);
                        int changes = _context.SaveChanges();
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
            else
            {
                request.ConfirmationMessage = "A password and password confirmation were not entered.";
            }
            return View(request);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            string accountId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            request.AccountId = Convert.ToInt32(accountId);
            return View(request);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword([System.Web.Http.FromBody] ChangePasswordRequest request)
        {
            request.Status = RequestStatus.Failed;
            if (!string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.NewPassword) && !string.IsNullOrEmpty(request.ConfirmPassword))
            {
                Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
                if (acc != null)
                {
                    try
                    {
                        string oldPasswordFromDB = AESEncryptDecrypt.Decrypt(acc.Password, TextPortCore.Helpers.Constants.RC4Key);
                        if (oldPasswordFromDB == request.OldPassword)
                        {
                            acc.Password = AESEncryptDecrypt.Encrypt(request.NewPassword, TextPortCore.Helpers.Constants.RC4Key);
                            int changes = _context.SaveChanges();
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
            else
            {
                request.ConfirmationMessage = "An old password, new password or password confirmation were not entered.";
            }
            return View(request);
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult PasswordResetEmail()
        {
            //ForgotPasswordRequest req = new ForgotPasswordRequest()
            //{
            //    EmailAddress = "richard@egley.com",
            //    UserName = "regley",
            //    ConfirmationMessage = string.Empty,

            //}
            return View();
        }

        //
        // POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        //
        // GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

    }
}


//using System;
//using System.Globalization;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using TextPort.Models;

//namespace TextPort.Controllers
//{
//    [Authorize]
//    public class AccountController : Controller
//    {
//        private ApplicationSignInManager _signInManager;
//private ApplicationUserManager _userManager;

//        public AccountController()
//        {
//        }

//        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
//        {
//            UserManager = userManager;
//            SignInManager = signInManager;
//        }

//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set 
//            { 
//                _signInManager = value; 
//            }
//        }

//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        //
//        // GET: /Account/Login
//        [AllowAnonymous]
//        public ActionResult Login(string returnUrl)
//        {
//            ViewBag.ReturnUrl = returnUrl;
//            return View();
//        }

//        //
//        // POST: /Account/Login
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // This doesn't count login failures towards account lockout
//            // To enable password failures to trigger account lockout, change to shouldLockout: true
//            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return RedirectToLocal(returnUrl);
//                case SignInStatus.LockedOut:
//                    return View("Lockout");
//                case SignInStatus.RequiresVerification:
//                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
//                case SignInStatus.Failure:
//                default:
//                    ModelState.AddModelError("", "Invalid login attempt.");
//                    return View(model);
//            }
//        }

//        //
//        // GET: /Account/VerifyCode
//        [AllowAnonymous]
//        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
//        {
//            // Require that the user has already logged in via username/password or external login
//            if (!await SignInManager.HasBeenVerifiedAsync())
//            {
//                return View("Error");
//            }
//            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
//        }

//        //
//        // POST: /Account/VerifyCode
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // The following code protects for brute force attacks against the two factor codes. 
//            // If a user enters incorrect codes for a specified amount of time then the user account 
//            // will be locked out for a specified amount of time. 
//            // You can configure the account lockout settings in IdentityConfig
//            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return RedirectToLocal(model.ReturnUrl);
//                case SignInStatus.LockedOut:
//                    return View("Lockout");
//                case SignInStatus.Failure:
//                default:
//                    ModelState.AddModelError("", "Invalid code.");
//                    return View(model);
//            }
//        }

//        //
//        // GET: /Account/Register
//        [AllowAnonymous]
//        public ActionResult Register()
//        {
//            return View();
//        }

//        //
//        // POST: /Account/Register
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Register(RegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
//                var result = await UserManager.CreateAsync(user, model.Password);
//                if (result.Succeeded)
//                {
//                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

//                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
//                    // Send an email with this link
//                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
//                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
//                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

//                    return RedirectToAction("Index", "Home");
//                }
//                AddErrors(result);
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }

//        //
//        // GET: /Account/ConfirmEmail
//        [AllowAnonymous]
//        public async Task<ActionResult> ConfirmEmail(string userId, string code)
//        {
//            if (userId == null || code == null)
//            {
//                return View("Error");
//            }
//            var result = await UserManager.ConfirmEmailAsync(userId, code);
//            return View(result.Succeeded ? "ConfirmEmail" : "Error");
//        }

//        //
//        // GET: /Account/ForgotPassword
//        [AllowAnonymous]
//        public ActionResult ForgotPassword()
//        {
//            return View();
//        }

//        //
//        // POST: /Account/ForgotPassword
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await UserManager.FindByNameAsync(model.Email);
//                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
//                {
//                    // Don't reveal that the user does not exist or is not confirmed
//                    return View("ForgotPasswordConfirmation");
//                }

//                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
//                // Send an email with this link
//                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
//                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
//                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
//                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }

//        //
//        // GET: /Account/ForgotPasswordConfirmation
//        [AllowAnonymous]
//        public ActionResult ForgotPasswordConfirmation()
//        {
//            return View();
//        }

//        //
//        // GET: /Account/ResetPassword
//        [AllowAnonymous]
//        public ActionResult ResetPassword(string code)
//        {
//            return code == null ? View("Error") : View();
//        }

//        //
//        // POST: /Account/ResetPassword
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }
//            var user = await UserManager.FindByNameAsync(model.Email);
//            if (user == null)
//            {
//                // Don't reveal that the user does not exist
//                return RedirectToAction("ResetPasswordConfirmation", "Account");
//            }
//            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
//            if (result.Succeeded)
//            {
//                return RedirectToAction("ResetPasswordConfirmation", "Account");
//            }
//            AddErrors(result);
//            return View();
//        }

//        //
//        // GET: /Account/ResetPasswordConfirmation
//        [AllowAnonymous]
//        public ActionResult ResetPasswordConfirmation()
//        {
//            return View();
//        }

//        //
//        // POST: /Account/ExternalLogin
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public ActionResult ExternalLogin(string provider, string returnUrl)
//        {
//            // Request a redirect to the external login provider
//            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
//        }

//        //
//        // GET: /Account/SendCode
//        [AllowAnonymous]
//        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
//        {
//            var userId = await SignInManager.GetVerifiedUserIdAsync();
//            if (userId == null)
//            {
//                return View("Error");
//            }
//            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
//            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
//            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
//        }

//        //
//        // POST: /Account/SendCode
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> SendCode(SendCodeViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View();
//            }

//            // Generate the token and send it
//            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
//            {
//                return View("Error");
//            }
//            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
//        }

//        //
//        // GET: /Account/ExternalLoginCallback
//        [AllowAnonymous]
//        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
//        {
//            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
//            if (loginInfo == null)
//            {
//                return RedirectToAction("Login");
//            }

//            // Sign in the user with this external login provider if the user already has a login
//            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return RedirectToLocal(returnUrl);
//                case SignInStatus.LockedOut:
//                    return View("Lockout");
//                case SignInStatus.RequiresVerification:
//                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
//                case SignInStatus.Failure:
//                default:
//                    // If the user does not have an account, then prompt the user to create an account
//                    ViewBag.ReturnUrl = returnUrl;
//                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
//                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
//            }
//        }

//        //
//        // POST: /Account/ExternalLoginConfirmation
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                return RedirectToAction("Index", "Manage");
//            }

//            if (ModelState.IsValid)
//            {
//                // Get the information about the user from the external login provider
//                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
//                if (info == null)
//                {
//                    return View("ExternalLoginFailure");
//                }
//                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
//                var result = await UserManager.CreateAsync(user);
//                if (result.Succeeded)
//                {
//                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
//                    if (result.Succeeded)
//                    {
//                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//                        return RedirectToLocal(returnUrl);
//                    }
//                }
//                AddErrors(result);
//            }

//            ViewBag.ReturnUrl = returnUrl;
//            return View(model);
//        }

//        //
//        // POST: /Account/LogOff
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult LogOff()
//        {
//            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            return RedirectToAction("Index", "Home");
//        }

//        //
//        // GET: /Account/ExternalLoginFailure
//        [AllowAnonymous]
//        public ActionResult ExternalLoginFailure()
//        {
//            return View();
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_userManager != null)
//                {
//                    _userManager.Dispose();
//                    _userManager = null;
//                }

//                if (_signInManager != null)
//                {
//                    _signInManager.Dispose();
//                    _signInManager = null;
//                }
//            }

//            base.Dispose(disposing);
//        }

//        #region Helpers
//        // Used for XSRF protection when adding external logins
//        private const string XsrfKey = "XsrfId";

//        private IAuthenticationManager AuthenticationManager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().Authentication;
//            }
//        }

//        private void AddErrors(IdentityResult result)
//        {
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error);
//            }
//        }

//        private ActionResult RedirectToLocal(string returnUrl)
//        {
//            if (Url.IsLocalUrl(returnUrl))
//            {
//                return Redirect(returnUrl);
//            }
//            return RedirectToAction("Index", "Home");
//        }

//        internal class ChallengeResult : HttpUnauthorizedResult
//        {
//            public ChallengeResult(string provider, string redirectUri)
//                : this(provider, redirectUri, null)
//            {
//            }

//            public ChallengeResult(string provider, string redirectUri, string userId)
//            {
//                LoginProvider = provider;
//                RedirectUri = redirectUri;
//                UserId = userId;
//            }

//            public string LoginProvider { get; set; }
//            public string RedirectUri { get; set; }
//            public string UserId { get; set; }

//            public override void ExecuteResult(ControllerContext context)
//            {
//                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
//                if (UserId != null)
//                {
//                    properties.Dictionary[XsrfKey] = UserId;
//                }
//                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
//            }
//        }
//        #endregion
//    }
//}