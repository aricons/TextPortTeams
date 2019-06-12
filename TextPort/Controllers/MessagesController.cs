using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{

    public class MessagesController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Main()
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);
            if (accountId > 0)
            {
                MessagingContainer mc = new MessagingContainer(accountId);
                Cookies.WriteBalance(mc.Account.Balance); // Get the updated balance for safety.
                return View(mc);
            }
            else
            {
                // TODO: Redirect to error page if account ID <= 0;
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetRecentToNumbersForDedicatedVirtualNumber(int aid, int vnid)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<Recent> recentsList = new List<Recent>();
                    recentsList = da.GetRecentToNumbersForDedicatedVirtualNumber(aid, vnid);
                    if (recentsList.Any())
                    {
                        recentsList.FirstOrDefault().IsActiveMessage = true;
                    }

                    return PartialView("_RecentsList", recentsList);
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetMessagesForNumber(int aid, int vnid, string num)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    MessageList messageList = new MessageList();
                    messageList.Messages = da.GetMessagesForAccountAndRecipient(aid, vnid, num);

                    return PartialView("_MessageList", messageList);
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SendMessage([System.Web.Http.FromBody] Message message)
        {
            try
            {
                string accountIdStr = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
                message.AccountId = Convert.ToInt32(accountIdStr);
                message.TimeStamp = DateTime.UtcNow;
                message.MessageType = (byte)MessageTypes.Normal;
                message.Direction = (byte)MessageDirection.Outbound;
                message.CarrierId = (int)Carriers.BandWidth;
                message.QueueStatus = (byte)QueueStatuses.Queued;
                message.Ipaddress = Utilities.GetUserHostAddress();
                if (message.MMSFiles.Count > 0)
                {
                    message.CustomerCost = Constants.BaseMMSMessageCost;
                }
                else
                {
                    message.CustomerCost = Constants.BaseSMSMessageCost;
                }

                if (message.AccountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        decimal newBalance = 0;
                        if (da.InsertMessage(message, ref newBalance) > 0)
                        {
                            Cookies.WriteBalance(newBalance);

                            MessageList messageList = new MessageList()
                            {
                                Messages = { message }
                            };

                            message.Send();

                            return PartialView("_MessageList", messageList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string responseHtml = string.Empty;
            string accountIdStr = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int imageId = RandomString.RandomNumber();

            try
            {
                string fileName = Utilities.RemoveWhitespace(file.FileName);
                var fileHandler = new FileHandling();
                if (fileHandler.SaveMMSFile(file.InputStream, Convert.ToInt32(accountIdStr), $"{imageId}_{fileName}", false))
                {
                    TempImage mi = new TempImage(Convert.ToInt32(accountIdStr), imageId, fileName, MessageDirection.Outbound, ImageStorageRepository.Archive);
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

        [Authorize]
        [HttpPost]
        public ActionResult DeleteMMSFile([System.Web.Http.FromBody] FileNameParameter fileNameParam)
        {
            string responseMesssage = string.Empty;
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;

            try
            {
                var fileHandler = new FileHandling();
                if (fileHandler.DeleteMMSFile(Convert.ToInt32(accountIdStr), fileNameParam.FileName, false))
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

        [Authorize]
        [HttpGet]
        public ActionResult SignalRTest()
        {
            return View();
        }

        public class FileNameParameter
        {
            public string FileName { get; set; }
        }
    }
}