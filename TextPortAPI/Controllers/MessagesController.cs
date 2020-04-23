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

using Swashbuckle.Swagger.Annotations;

namespace TextPortAPI.Controllers
{
    /// <summary>
    /// Handles send message requests for the TextPort SMS API.
    /// </summary>
    [RoutePrefix("v1/messages")]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// Sends one or more messages.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /send
        ///     [
        ///         {
        ///            "From": "19195551212",
        ///            "To": "15055551212",
        ///            "MessageText": "Sample text message from TextPort SMS API"
        ///         }
        ///     ]
        ///
        /// </remarks>
        /// <param name="messages"></param>
        /// <returns>A message result object</returns>
        /// <response code="200">A MessageResult object</response>
        /// <response code="400">A MessageResult objecet with details in the ErrorMessage field</response>     
        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("send")]
        [SwaggerResponse(HttpStatusCode.OK, "Message confirmation object", typeof(IEnumerable<MessageResult>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Message confirmation object", typeof(IEnumerable<MessageResult>))]
        public IHttpActionResult Send([FromBody]List<Message> messages)
        {
            List<MessageResult> results = new List<MessageResult>();
            int failureCount = 0;
            decimal newBalance = 0;

            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        string validationMessage = string.Empty;
                        newBalance = da.GetAccountBalance(accountId);

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
                                        if (da.InsertMessage(msg, ref newBalance) > 0)
                                        {
                                            if (msg.Send())
                                            {
                                                results.Add(new MessageResult(message, "OK", msg.MessageId, string.Empty, newBalance));
                                            }
                                            else
                                            {
                                                results.Add(new MessageResult(message, "FAIL", 0, msg.ProcessingMessage, newBalance));
                                                failureCount++;
                                            }
                                        }
                                        else
                                        {
                                            results.Add(new MessageResult(message, "FAIL", 0, msg.ProcessingMessage, newBalance));
                                            failureCount++;
                                        }
                                    }
                                    else
                                    {
                                        results.Add(new MessageResult(message, "FAIL", 0, $"The number {message.From} is cancelled.", newBalance));
                                        failureCount++;
                                    }
                                }
                                else
                                {
                                    results.Add(new MessageResult(message, "FAIL", 0, $"The number {message.From} is invalid or is not associated with this application ID.", newBalance));
                                    failureCount++;
                                }
                            }
                            else
                            {
                                results.Add(new MessageResult(message, "FAIL", 0, validationMessage, newBalance));
                                failureCount++;
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

            if (failureCount > 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, results));
            }
            else
            {
                return Ok(results);
            }
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
                msg.To = Utilities.NumberToE164(msg.To);
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
