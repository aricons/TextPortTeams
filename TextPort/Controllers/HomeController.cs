using System;
using System.Linq;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Models;
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
            return View(processContactOrSupportRequest(request));
        }

        [Authorize]
        [HttpGet]
        public ActionResult Support()
        {
            SupportRequestModel supportRequestModel = new SupportRequestModel(SupportRequestType.Support);
            return View(supportRequestModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Support(SupportRequestModel request)
        {
            return View(processContactOrSupportRequest(request));
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

        private SupportRequestModel processContactOrSupportRequest(SupportRequestModel request)
        {
            SupportRequestModel response = new SupportRequestModel()
            {
                SupportId = 0,
                SubmissionStatus = RequestStatus.Failed,
                SubmissionMessage = "There was a problem processing the request. Please try again."
            };

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
    }
}