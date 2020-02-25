using System;
using System.Web.Mvc;
using System.Web.Configuration;

using Newtonsoft.Json;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class FreeController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        [ActionName("send-text")]
        public ActionResult SendText()
        {
            FreeTextContainer ft = new FreeTextContainer();
            return View(ft);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("send-text")]
        public ActionResult SendText(FreeTextContainer request)
        {
            int freeTextAccountId = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["FreeTextAccountId"]);
            int freeTextsPerIPLimit = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["MaxFreeTextsPerIP"]);
            int freeTextsPerMobileNumberLimit = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["MaxFreeTextsToMobileNumber"]);
            request.IPAddress = Request.UserHostAddress;

            request.AccountId = freeTextAccountId;

            if (!string.IsNullOrEmpty(request.IPAddress))
            {
                CaptchaResponse captchaResponse = ValidateCaptcha(Request["g-recaptcha-response"]);

                if (captchaResponse.Success)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        int requestsFromIP = da.CheckFreeTextCountForIP(request.IPAddress, freeTextsPerIPLimit);
                        if (requestsFromIP <= freeTextsPerIPLimit)
                        {
                            int freeTextsSentToNumber = da.CheckFreeSendCountForNumber(request.AccountId, Utilities.NumberToE164(request.MobileNumber));
                            if (freeTextsSentToNumber <= freeTextsPerMobileNumberLimit)
                            {
                                decimal newBalance = 0;
                                Message freeMessage = new Message(request);
                                freeMessage.MessageText += "\r\n\r\nFree message sent via TextPort.com.";
                                if (da.InsertMessage(freeMessage, ref newBalance) > 0)
                                {
                                    freeMessage.Send();
                                    request.Result = "OK";
                                    request.SubmissionMessage = $"Your message was successfully sent to {Utilities.NumberToDisplayFormat(Utilities.NumberToE164(request.MobileNumber), 22)}.";
                                }
                            }
                            else
                            {
                                request.SubmissionMessage = $"Too many texts sent to {Utilities.NumberToDisplayFormat(Utilities.NumberToE164(request.MobileNumber), 22)}. To send more, you can <a href='/trial'>register a trial account</a> or <a href='/account/signup'>sign up for a full account</a>.";
                            }
                        }
                        else
                        {
                            request.SubmissionMessage = "Free text limit exceeded. To send more, you can <a href='/trial'>register a trial account</a> or <a href='/account/signup'>sign up for a full account</a>.";
                        }
                    }
                }
                else
                {
                    request.SubmissionMessage = "Request failed. The Captcha was not validated.";
                }
            }
            else
            {
                request.SubmissionMessage = "Unable to validate source IP.";
            }

            return View(request);
        }

        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];
            var client = new System.Net.WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }
    }
}