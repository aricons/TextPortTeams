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
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
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

        [HttpGet]
        public ActionResult IndexNew()
        {
            int accountId = 1;
            if (accountId > 0)
            {
                MessagingContainer mc = new MessagingContainer(accountId);
                Cookies.WriteBalance(mc.Account.Balance); // Get the updated balance for safety.
                return View(mc);
            }
            return View();
        }

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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

        [HttpGet]
        public ActionResult Test()
        {
            return View();
        }

        // For Google Analytics tracking. Send() and Receive()
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Send()
        {
            return PartialView("_SendMessage", new Message() { MessageText = "TextPort sent message placeholder", TimeStamp = DateTime.UtcNow });
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Receive()
        {
            return PartialView("_ReceiveMessage", new Message() { MessageText = "TextPort received message placeholder", TimeStamp = DateTime.UtcNow });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult SendMessage([System.Web.Http.FromBody] Message message)
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                message.AccountId = accountId;
                message.TimeStamp = DateTime.UtcNow;
                message.MessageType = (byte)MessageTypes.Normal;
                message.Direction = (byte)MessageDirection.Outbound;
                message.CarrierId = (int)Carriers.BandWidth;
                message.QueueStatus = (byte)QueueStatuses.NotProcessed;
                message.Ipaddress = Utilities.GetUserHostAddress();

                if (message.AccountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        if (da.NumberIsBlocked(message.MobileNumber, MessageDirection.Outbound))
                        {
                            message.MessageText = $"BLOCKED: The recipient at number {message.MobileNumber} has reported abuse from this account. We have blocked the number at their request. TextPort does not condone the exchange of abusive, harrassing or defamatory messages.";
                            MessageList messageList = new MessageList()
                            {
                                Messages = { message }
                            };
                            return PartialView("_MessageListNew", messageList);
                        }

                        decimal newBalance = 0;
                        if (da.InsertMessage(message, ref newBalance) > 0)
                        {
                            MessageList messageList = new MessageList()
                            {
                                Messages = { message }
                            };

                            message.Send();
                            message.ConvertTimeStampToLocalTimeZone();

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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult DeleteMMSFile([System.Web.Http.FromBody] FileNameParameter fileNameParam)
        {
            string responseMesssage = string.Empty;
            string accountIdStr = ClaimsPrincipal.Current.FindFirst("AccountId").Value;

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

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult DeleteMessagesForNumber(DeleteMessageInfo deleteMessageInfo)
        {
            string responseMesssage = string.Empty;
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            try
            {
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        int messagesDeleted = 0;
                        switch (deleteMessageInfo.DeleteType)
                        {
                            case "VN":
                                messagesDeleted = da.DeleteMessagesForVirtualNumber(accountId, deleteMessageInfo.VirtualNumberId);
                                break;

                            case "VNAndMobile":
                                messagesDeleted = da.DeleteMessagesForVirtualNumberAndMobileNumber(accountId, deleteMessageInfo.VirtualNumberId, deleteMessageInfo.MobileNumber);
                                break;
                        }
                        responseMesssage = $"{messagesDeleted} messaged deleted.";
                    }
                }
            }
            catch (Exception ex)
            {
                responseMesssage = $"Delete failed. {ex.Message}";
            }

            return Json(new
            {
                success = true,
                response = responseMesssage
            });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult GetDeletePromptModal(DeleteMessageInfo deleteMessageInfo)
        {
            switch (deleteMessageInfo.DeleteType)
            {
                case "VN":
                    deleteMessageInfo.Title = "Delete Messages?";
                    deleteMessageInfo.Prompt = $"Delete ALL messages for virtual number<br/> {Utilities.NumberToDisplayFormat(deleteMessageInfo.VirtualNumber, 22)}?";
                    deleteMessageInfo.SubPrompt = "Warning: This action is permanent. Deleted messages cannot be recovered.";
                    break;

                case "VNAndMobile":
                    deleteMessageInfo.Title = "Delete Messages?";
                    deleteMessageInfo.Prompt = $"Delete all messages for number {Utilities.NumberToDisplayFormat(deleteMessageInfo.MobileNumber, 22)}?";
                    deleteMessageInfo.SubPrompt = string.Empty;
                    break;

                default: // Same as VNAndMobile
                    deleteMessageInfo.Title = "Delete Messages?";
                    deleteMessageInfo.Prompt = $"Delete all messages for number {Utilities.NumberToDisplayFormat(deleteMessageInfo.MobileNumber, 22)}?";
                    deleteMessageInfo.SubPrompt = string.Empty;
                    break;
            }

            return PartialView("_ConfirmDeleteMessages", deleteMessageInfo);
        }

        [HttpPost]
        public ActionResult WriteEmailToMMSSemaphore([System.Web.Http.FromBody] Message message)
        {
            if (message.Send())
            {
                return Json(new
                {
                    success = true,
                    response = message.ProcessingMessage
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    response = message.ProcessingMessage
                });
            }
        }

        [Authorize(Roles = "User")]
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