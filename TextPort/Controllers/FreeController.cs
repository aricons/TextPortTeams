using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.Owin.Security;

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
            FreeTextContainer ftc = null;
            if (User.Identity.IsAuthenticated && !String.IsNullOrEmpty(User.Identity.Name))
            {
                ftc = new FreeTextContainer(User.Identity.Name);
            }
            else
            {
                ftc = new FreeTextContainer();
            }
            return View(ftc);
        }

        //[Authorize(Roles = "Free")]
        [HttpPost]
        [ActionName("send-text")]
        public ActionResult SendText([System.Web.Http.FromBody] Message message)
        {
            int freeTextAccountId = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["FreeTextAccountId"]);
            int freeTextsPerIPLimit = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["MaxFreeTextsPerIP"]);
            int freeTextsPerMobileNumberLimit = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["MaxFreeTextsToMobileNumber"]);

            message.Ipaddress = Request.UserHostAddress;
            message.AccountId = freeTextAccountId;
            message.MessageType = (byte)MessageTypes.FreeTextSend;
            message.IsMMS = (message.MMSFiles.Count > 0);

            // Log the user in using the hub connection ID as the user name. This allows for messages to be sent back to
            // the same user if they leve the page, then come back during the same browser session.
            if (!User.Identity.IsAuthenticated && String.IsNullOrEmpty(User.Identity.Name) && !User.IsInRole("Free"))
            {
                FreeTextContainer ft = new FreeTextContainer(message.SessionId);

                List<Claim> claims = new List<Claim> {
                new Claim("AccountId", freeTextAccountId.ToString(), ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Name, ft.SessionId),
                new Claim(ClaimTypes.NameIdentifier, ft.SessionId),
                new Claim(ClaimTypes.Role, "Free") };

                ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                Request.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
            }

            FreeTextResult result = new FreeTextResult();

            if (!string.IsNullOrEmpty(message.Ipaddress))
            {
                using (TextPortDA da = new TextPortDA())
                {
                    result.Messages = da.GetMessagesForAccountAndRecipient(message.AccountId, message.VirtualNumberId, Utilities.NumberToE164(message.MobileNumber));

                    if (!da.NumberIsBlocked(message.MobileNumber, MessageDirection.Outbound))
                    {
                        int requestsFromIP = da.CheckFreeTextCountForIP(message.Ipaddress, freeTextsPerIPLimit);
                        if (requestsFromIP <= freeTextsPerIPLimit)
                        {
                            int freeTextsSentToNumber = da.CheckFreeSendCountForNumber(message.AccountId, Utilities.NumberToE164(message.MobileNumber));
                            if (freeTextsSentToNumber <= freeTextsPerMobileNumberLimit)
                            {
                                decimal newBalance = 0;

                                message.MessageText += "\r\nSent via TextPort.com.";
                                if (da.InsertMessage(message, ref newBalance) > 0)
                                {
                                    message.Send();
                                    result.Messages.Add(message);
                                    result.Status = "OK";
                                    result.SubmissionMessage = $"Your message was successfully sent to {Utilities.NumberToDisplayFormat(Utilities.NumberToE164(message.MobileNumber), 22)}.";
                                }
                            }
                            else
                            {
                                result.Messages.Add(createNotificationMessage(freeTextAccountId, $"Too many texts sent to {Utilities.NumberToDisplayFormat(Utilities.NumberToE164(message.MobileNumber), 22)}. To send more, you can <a href='/trial'>register a trial account</a> or <a href='/account/signup'>sign up for a full account</a>."));
                            }
                        }
                        else
                        {
                            result.Messages.Add(createNotificationMessage(freeTextAccountId, @"You have exceeded your free texting limit. To send more texts, you can <a href=""/trial"">register a trial account</a> or <a href=""/account/signup"">sign up for a full account</a>."));
                        }
                    }
                    else
                    {
                        result.Messages.Add(createNotificationMessage(freeTextAccountId, $"BLOCKED: The recipient at number {Utilities.NumberToDisplayFormat(Utilities.NumberToE164(message.MobileNumber), 22)} has reported abuse. We have blocked the number at their request. TextPort does not condone the exchange of abusive, harrassing or defamatory messages."));
                    }
                }
            }
            else
            {
                result.Messages.Add(createNotificationMessage(freeTextAccountId, "Unable to validate source IP address."));
                result.SubmissionMessage = "Unable to validate source IP.";
            }
            return PartialView("_MessageList_FreeTextResult", result);
        }

        [Authorize(Roles = "Free")]
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string responseHtml = string.Empty;
            int accountId = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["FreeTextAccountId"]);
            int imageId = RandomString.RandomNumber();

            try
            {
                string fileName = Utilities.RemoveWhitespace(file.FileName);
                var fileHandler = new FileHandling();
                if (fileHandler.SaveMMSFile(file.InputStream, accountId, $"{imageId}_{fileName}", false))
                {
                    TempImage mi = new TempImage(accountId, imageId, fileName, MessageDirection.Outbound, ImageStorageRepository.Archive);
                    return PartialView("_TempImage", mi);
                }
            }
            catch (Exception ex)
            {
                responseHtml = $"Upload failed. {ex.Message}";
            }

            responseHtml = "Failure uploading file";
            return Json(new
            {
                success = false,
                response = responseHtml
            });
        }

        [Authorize(Roles = "Free")]
        [HttpPost]
        public ActionResult DeleteMMSFile([System.Web.Http.FromBody] FileNameParameter fileNameParam)
        {
            string responseMesssage = string.Empty;
            int accountId = Conversion.StringToIntOrZero(WebConfigurationManager.AppSettings["FreeTextAccountId"]);

            try
            {
                var fileHandler = new FileHandling();
                if (fileHandler.DeleteMMSFile(accountId, fileNameParam.FileName, false))
                {
                    responseMesssage = "File deleted";
                    return Json(new
                    {
                        success = true,
                        response = responseMesssage
                    });
                }
            }
            catch (Exception ex)
            {
                responseMesssage = $"Delete failed. {ex.Message}";
            }

            responseMesssage = "Failure deleting file";
            return Json(new
            {
                success = true,
                response = responseMesssage
            });
        }

        private Message createNotificationMessage(int accountId, string messageText)
        {
            return new Message()
            {
                GatewayMessageId = "FT_Exceeded",
                AccountId = accountId,
                MessageText = messageText
            };
        }

        //public static CaptchaResponse ValidateCaptcha(string response)
        //{
        //    string secret = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];
        //    var client = new System.Net.WebClient();
        //    var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
        //    return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        //}

        public class FileNameParameter
        {
            public string FileName { get; set; }
        }
    }
}