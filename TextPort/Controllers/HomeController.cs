using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Loginm()
        {
            try
            {
                LoginCredentials creds = new LoginCredentials();
                return PartialView("Loginm", creds);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Loginm(LoginCredentials model)
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
                        new Claim(ClaimTypes.Role, "User") };

                        ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        var context = Request.GetOwinContext();
                        var authManager = context.Authentication;

                        authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                        Cookies.Write("balance", account.Balance.ToString(), 0);

                        if (account.ComplimentaryNumber == (byte)ComplimentaryNumberStatus.Eligible)
                        {
                            //result = new { success = "true", response = Url.Action($"ComplimentaryNumber/{account.AccountId}", "Numbers") };
                            return RedirectToAction($"ComplimentaryNumber/{account.AccountId}", "Numbers");
                        }
                        else
                        {
                            //result = new { success = "true", response = Url.Action("index", "messages") };
                            return RedirectToAction("index", "messages");
                        }
                    }
                    else
                    {
                        ModelState.Clear();
                        model.Result = "Invalid username, email or password";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(model);
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

        [ActionName("virtual-mobile-numbers")]
        public ActionResult DedicatedVirtualNumbers()
        {
            RegistrationData rd = new RegistrationData("VirtualNumberSignUp", 0);
            return View(rd);
        }

        [ActionName("email-to-sms-gateway")]
        public ActionResult EmailToSMSGateway()
        {
            return View();
        }

        [ActionName("bulk-texting")]
        public ActionResult BulkTexting()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Contact()
        {
            SupportRequestModel supportRequestModel = new SupportRequestModel(SupportRequestType.Contact);
            return View(supportRequestModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(SupportRequestModel request)
        {
            CaptchaResponse captchaResponse = ValidateCaptcha(Request["g-recaptcha-response"]);
            return View(processContactOrSupportRequest(request, captchaResponse));
        }

        [HttpGet]
        public ActionResult Support()
        {
            SupportRequestModel supportRequestModel = new SupportRequestModel(SupportRequestType.Support);
            return View(supportRequestModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Support(SupportRequestModel request)
        {
            CaptchaResponse captchaResponse = ValidateCaptcha(Request["g-recaptcha-response"]);
            return View(processContactOrSupportRequest(request, captchaResponse));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Terms()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Privacy()
        {
            return View();
        }

        private SupportRequestModel processContactOrSupportRequest(SupportRequestModel request, CaptchaResponse captchaResponse)
        {
            SupportRequestModel response = new SupportRequestModel()
            {
                SupportId = 0,
                SubmissionStatus = RequestStatus.Failed,
                SubmissionMessage = "There was a problem processing the request. Please try again."
            };

            if (captchaResponse.Success)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    int supportId = da.AddSupportRequest(request);

                    if (supportId > 0)
                    {
                        string emailBody = $"Request Category: {request.Category}{Environment.NewLine}{Environment.NewLine}";
                        emailBody += (!string.IsNullOrEmpty(request.SendingNumber) ? $"Sending Number: {request.SendingNumber}{Environment.NewLine}" : string.Empty);
                        emailBody += (!string.IsNullOrEmpty(request.ReceivingNumber) ? $"Receiving Number: {request.ReceivingNumber}{Environment.NewLine}" : string.Empty);
                        emailBody += $"Message: {request.Message}{Environment.NewLine}";

                        EmailMessage email = new EmailMessage()
                        {
                            From = request.RequestorEmail,
                            FromName = request.RequestorName,
                            To = "support@textport.com",
                            Subject = $"TextPort {request.RequestType} Request # {supportId}",
                            Body = emailBody
                        };

                        if (email.Send(false))
                        {
                            ModelState.Clear(); // Reset the request fields.
                            response.SupportId = supportId;
                            response.SubmissionStatus = RequestStatus.Success;
                            response.SubmissionMessage = getContactSupportResponseText(request.Category, supportId);
                        }
                    }
                    response.CategoriesList = da.GetSupportCategoriesList(request.RequestType);
                }
            }
            else
            {
                ModelState.Clear();
                response.SubmissionMessage = "Request failed. The Captcha was not validated.";
            }

            return response;
        }

        private string getContactSupportResponseText(string requestCatrgory, int supportId)
        {
            string message = string.Empty;
            switch (requestCatrgory)
            {

                case "General":
                    message = "Thank you for your interest in TextPort. We will review your request and respond shortly.";
                    break;

                case "Feedback":
                    message = "Thank you. We appreciate you taking the time to provide your feedback. We will review your submission and respond via email if necessary.";
                    break;

                case "Abuse":
                    message = "Thank you for your submission. We take abuse cases seriously. We will research your request and respond shortly.";
                    break;

                default:
                    message = "Thank you for your submission. We will review your request and respond shortly.";
                    break;
            }

            message += $" Your request reference ID is <b>{supportId}</b>.";

            return message;
        }

        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];
            var client = new System.Net.WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }
    }
}