using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class MessagesController : Controller
    {
        // [Authorize(Roles = "Administrative User, Branch Manager, User")]
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);
            if (branchId > 0 && accountId > 0)
            {
                MessagingContainer mc = new MessagingContainer(branchId, accountId, role);
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
        public ActionResult GetNumbersForBranch(int bid)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<NumberDDItem> numbersList = new List<NumberDDItem>();

                    foreach (DedicatedVirtualNumber n in da.GetNumbersForBranch(bid, false))
                    {
                        numbersList.Add(new NumberDDItem()
                        {
                            VirtualNumberId = n.VirtualNumberId.ToString(),
                            Number = n.VirtualNumber,
                            NumberDisplay = n.NumberDisplayFormat
                        });
                    }

                    return Json(numbersList, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetRecentToNumbersForDedicatedVirtualNumber(int bid, int vnid)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<Recent> recentsList = new List<Recent>();
                    recentsList = da.GetRecentToNumbersForDedicatedVirtualNumber(bid, vnid);
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
        public ActionResult GetMessagesForNumber(int bid, int vnid, string num)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    MessageList messageList = new MessageList();
                    messageList.Messages = da.GetMessagesForBranchAndRecipient(bid, vnid, num);

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
        [Authorize]
        [HttpGet]
        public ActionResult Send()
        {
            return PartialView("_SendMessage", new Message() { MessageText = "TextPort sent message placeholder", TimeStamp = DateTime.UtcNow });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Receive()
        {
            return PartialView("_ReceiveMessage", new Message() { MessageText = "TextPort received message placeholder", TimeStamp = DateTime.UtcNow });
        }

        [Authorize]
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
                message.QueueStatus = (byte)QueueStatuses.NotProcessed;
                message.Ipaddress = Utilities.GetUserHostAddress();
                message.Segments = Utilities.GetSegmentCount(message.MessageText);

                if (message.AccountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        if (message.BranchId > 0 && message.Branch == null)
                        {
                            message.Branch = da.GetBranchByBranchId(message.BranchId);
                        }

                        if (da.IsNumberStopped(message.MobileNumber))
                        {
                            message.MessageText = $"OPT-OUT: The recipient at number {message.MobileNumber} has opted out of text notifications.";
                            MessageList messageList = new MessageList()
                            {
                                Messages = { message }
                            };
                            return PartialView("_MessageList", messageList);
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

        [Authorize]
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string responseHtml = string.Empty;
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
            int imageId = RandomString.RandomNumber();

            try
            {
                string fileName = Utilities.RemoveWhitespace(file.FileName);
                var fileHandler = new FileHandling();
                if (fileHandler.SaveMMSFile(file.InputStream, branchId, $"{imageId}_{fileName}", false))
                {
                    TempImage mi = new TempImage(branchId, imageId, fileName, MessageDirection.Outbound, ImageStorageRepository.Archive);
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
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);

            try
            {
                var fileHandler = new FileHandling();
                if (fileHandler.DeleteMMSFile(branchId, fileNameParam.FileName, false))
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

        [Authorize]
        [HttpPost]
        public ActionResult GetDeletePromptModal(DeleteMessageInfo deleteMessageInfo)
        {
            switch (deleteMessageInfo.DeleteType)
            {
                case "VN":
                    deleteMessageInfo.Title = "Delete Messages?";
                    deleteMessageInfo.Prompt = $"Delete ALL messages for virtual number<br/> {Utilities.NumberToDisplayFormat(deleteMessageInfo.VirtualNumber, deleteMessageInfo.CountryId)}?";
                    deleteMessageInfo.SubPrompt = "Warning: This action is permanent. Deleted messages cannot be recovered.";
                    break;

                case "VNAndMobile":
                    deleteMessageInfo.Title = "Delete Messages?";
                    deleteMessageInfo.Prompt = $"Delete all messages for number {Utilities.NumberToDisplayFormat(deleteMessageInfo.MobileNumber, deleteMessageInfo.CountryId)}?";
                    deleteMessageInfo.SubPrompt = string.Empty;
                    break;

                default: // Same as VNAndMobile
                    deleteMessageInfo.Title = "Delete Messages?";
                    deleteMessageInfo.Prompt = $"Delete all messages for number {Utilities.NumberToDisplayFormat(deleteMessageInfo.MobileNumber, deleteMessageInfo.CountryId)}?";
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

        public class NumberDDItem
        {
            public string VirtualNumberId { get; set; }
            public string Number { get; set; }
            public string NumberDisplay { get; set; }
        }

    }
}