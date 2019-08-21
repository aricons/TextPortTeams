using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TextPortCore.Models.API;
using TextPortCore.Helpers;
using TextPortCore.Data;
using Core = TextPortCore.Models;

namespace TextPortAPI.Controllers
{
    [RoutePrefix("v1/messages")]
    public class MessagesController : ApiController
    {
        [HttpPost]
        [Authorize]
        [Route("send")]
        public HttpResponseMessage Send([FromBody]List<Message> messages)
        {
            // IHttpActionResult
            List<MessagesResult> results = new List<MessagesResult>();

            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        string validationMessage = string.Empty;
                        foreach (Message message in messages)
                        {
                            validationMessage = string.Empty;
                            if (validateMessage(message, ref validationMessage))
                            {
                                Core.DedicatedVirtualNumber virtualNumber = da.GetVirtualNumberByNumber(message.From, true);
                                if (virtualNumber != null)
                                {
                                    if (!virtualNumber.Cancelled)
                                    {
                                        Core.Message msg = new Core.Message(message, accountId, virtualNumber.VirtualNumberId);
                                        if (msg.Send())
                                        {
                                            results.Add(new MessagesResult(message, "OK", msg.MessageId, string.Empty));
                                        }
                                        else
                                        {
                                            results.Add(new MessagesResult(message, "FAIL", 0, msg.ProcessingMessage));
                                        }
                                    }
                                    else
                                    {
                                        results.Add(new MessagesResult(message, "FAIL", 0, $"The number {message.From} is cancelled."));
                                    }
                                }
                                else
                                {
                                    results.Add(new MessagesResult(message, "FAIL", 0, $"The number {message.From} is invalid or is not associated with this application ID."));
                                }
                            }
                            else
                            {
                                results.Add(new MessagesResult(message, "FAIL", 0, validationMessage));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //result.Message += "API method GetNumbers() failure. Exception: " + ex.Message + ". ";
                EventLogging.WriteEventLogEntry("An error occurred in API.NumbersController.Send(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        private bool validateMessage(Message msg, ref string validationMessage)
        {
            validationMessage = string.Empty;

            // Check From
            if (string.IsNullOrEmpty(msg.From))
            {
                validationMessage = "The From number is missing.";
                return false;
            }
            else
            {
                msg.From = Utilities.NumberToE164(msg.From);
                if (msg.From.Length != 11)
                {
                    validationMessage = "The From number is invalid.";
                    return false;
                }
            }

            // Check To
            if (string.IsNullOrEmpty(msg.To))
            {
                validationMessage = "The To number is missing.";
                return false;
            }
            else
            {
                msg.From = Utilities.NumberToE164(msg.To);
                if (msg.To.Length != 11)
                {
                    validationMessage = "The To number is invalid.";
                    return false;
                }
            }

            // Check To
            if (string.IsNullOrEmpty(msg.MessageText))
            {
                validationMessage = "The MessageText is empty.";
                return false;
            }

            validationMessage = "OK";
            return true;
        }
    }
}
