using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.Owin.Security;
//using System.Net.Http;
//using System.Web.Http;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;

using TextPortCore.Models;
using TextPortCore.Data;

namespace TextPort.Controllers
{
    public class AccountController : Controller
    {
        private TextPortContext _context = new TextPortContext();

        //public AccountController(TextPortContext context)
        //{
        //    _context = context;
        //}

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
                        new Claim(ClaimTypes.Role, "User")
                    };

                        ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        //HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        var context = Request.GetOwinContext();
                        var authManager = context.Authentication;

                        authManager.SignIn(new AuthenticationProperties
                        { IsPersistent = false }, identity);

                        result = new { success = "true", response = Url.Action("Main", "Messages") };
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
            //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Request.GetOwinContext().Authentication.SignOut();

            //Request.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

            //HttpContext.Current.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            try
            {
                RegistrationData rd = new RegistrationData(_context);

                // For testing
                rd.UserName = "regley1";
                rd.Password = "Zealand!4";
                rd.ConfirmPassword = "Zealand!4";
                rd.EmailAddress = "richardtester@egleytest.com";

                return View(rd);
            }
            catch (Exception)
            {
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
                    // Add a temporary account (Enabled flag set to 0).
                    regData.AccountId = da.AddTemporaryAccount(regData);
                    return PartialView("_Purchase", regData);
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostPurchase([System.Web.Http.FromBody] RegistrationData regData, int id)
        {
            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    regData.AccountId = id;
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

                            //ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                            //HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                            var context = Request.GetOwinContext();
                            var authManager = context.Authentication;

                            authManager.SignIn(new AuthenticationProperties
                            { IsPersistent = false }, identity);

                            return PartialView("_RegistrationComplete", regData);
                        }
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

            string accountId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            //string accountId = principal.FindFirst("AccountId").Value;

            //AccountView av = new AccountView(_context, Convert.ToInt32(User.FindFirstValue("AccountId")));
            AccountView av = new AccountView(_context, Convert.ToInt32(accountId));
            if (av != null)
            {
                return View(av);
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
                //av.TimeZones = da.GetTimeZones();

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
                    return Json($"The username {username} is already taken.");
                }
            }
            return Json(data: true);
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult VerifyEmail(string email)
        {
            using (TextPortDA da = new TextPortDA(_context))
            {
                if (!da.IsUsernameAvailable(email))
                {
                    return Json($"Email {email} is already registered to an account.");
                }
            }
            return Json(data: true);
        }

    }
}