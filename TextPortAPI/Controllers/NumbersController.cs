using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using core = TextPortCore.Models;
using TextPortCore.Models.API;
using TextPortCore.Data;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;

using Swashbuckle.Swagger.Annotations;

namespace TextPortAPI.Controllers
{
    /// <summary>
    /// Handles number-related events for the TextPort SMS API.
    /// </summary>
    [RoutePrefix("v1/numbers")]
    public class NumbersController : ApiController
    {
        /// <summary>
        /// Gets a list of available numbers for the specified area code.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /numbers/909
        ///
        /// </remarks>
        /// <param name="areaCode">The area code for the numbers requested.</param>
        /// <returns>A NumbersResult object containing a list of up available numbers</returns>
        /// <response code="200">Returns a list of available numbers</response>
        /// <response code="400">If an error occurred</response>    
        [HttpGet]
        [Authorize]
        [Route("{areaCode}")]
        [SwaggerResponse(HttpStatusCode.OK, "Message confirmation object", typeof(IEnumerable<NumbersResult>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Numbers(string areaCode)
        {
            NumbersResult result = new NumbersResult(areaCode);

            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    string areaCodeName = da.GetAreaCodeName(areaCode, false);
                    result.Message = areaCodeName;

                    if (areaCodeName != "Invalid area code")
                    {
                        using (Bandwidth bw = new Bandwidth())
                        {
                            List<string> bwNumbers = bw.GetVirtualNumbersList(areaCode, Constants.NumberOfNumbersToPullFromBandwidthForAPI, false, 1);
                            if (bwNumbers != null)
                            {
                                result.NumberCount = bwNumbers.Count;
                                result.Message = $"{result.NumberCount} numbers found.";
                                foreach (string number in bwNumbers)
                                {
                                    result.Numbers.Add(number);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message += "API method GET Numbers(areaCode) failure. Exception: " + ex.Message + ". ";
                EventLogging.WriteEventLogEntry("An error occurred in API.NumbersController.GetNumbers(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, result));
            }

            return Ok(result);
        }

        /// <summary>
        /// Assign a new number to your account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /numbers
        /// {
        ///     "Number": "12025551212",
        ///     "LeasePeriod": "1"
        /// }
        /// 
        /// </remarks>
        /// <param name="numberRequest">A NumberRequest object.</param>
        /// <returns>A NumberRequestResult object which contains details of the purchased number</returns>
        /// <response code="200">Returns a NumberRequestResult object</response>
        /// <response code="400">If an error occurred</response>
        /// <response code="404">If the requested number cannot be assigned</response>    
        [HttpPost]
        [Authorize]
        [ActionName("Numbers")]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, "Message confirmation object", typeof(IEnumerable<NumberRequestResult>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult NumbersPost(NumberRequest numberRequest)
        {
            NumberRequestResult result = new NumberRequestResult();

            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                if (accountId > 0)
                {
                    if (numberRequest != null)
                    {
                        if (string.IsNullOrEmpty(numberRequest.Number))
                        {
                            result.ProcessingMessage = "The number requested is empty.";
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, result));
                        }

                        numberRequest.Number = Utilities.NumberToE164(numberRequest.Number);
                        if (numberRequest.Number.Length != 11)
                        {
                            result.ProcessingMessage = "The number requested is invalid. Please check the length. Numbers should be 10 or 11 digits.";
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, result));
                        }

                        result.Number = numberRequest.Number;

                        using (TextPortDA da = new TextPortDA())
                        {
                            if (numberRequest.LeasePeriod <= 0)
                            {
                                numberRequest.LeasePeriod = 1;
                                result.ProcessingMessage += "The lease period was not specified. Defaulting to the minimum of 1 month.";
                            }
                            core.RegistrationData regData = new core.RegistrationData()
                            {
                                AccountId = accountId,
                                PurchaseType = "VirtualNumber",
                                VirtualNumber = numberRequest.Number,
                                LeasePeriod = (short)numberRequest.LeasePeriod,
                            };

                            // Check the balance
                            decimal balance = da.GetAccountBalance(accountId);
                            decimal requiredAmount = Constants.MonthlyNumberRenewalCost * numberRequest.LeasePeriod;
                            if (balance < requiredAmount)
                            {
                                result.ProcessingMessage = $"The account balance is insufficient. A balance of at least {requiredAmount:C} is required."; ;
                                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, result));
                            }

                            using (Bandwidth bw = new Bandwidth())
                            {
                                if (bw.PurchaseVirtualNumber(regData))
                                {
                                    result.ProcessingMessage += $"Number {regData.VirtualNumber} was successfully purchased for a {regData.LeasePeriod} month period.";
                                    result.ExpirationDate = DateTime.UtcNow.AddMonths(regData.LeasePeriod);
                                    result.Success = true;
                                }
                                else
                                {
                                    result.ProcessingMessage += $"The request to purchase number {regData.VirtualNumber} failed.'";
                                    result.Success = false;

                                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, result));
                                }
                            }
                        }
                    }
                }
                else
                {
                    result.ProcessingMessage = "Could not determine TextPort account ID.";
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, result));
                }
            }
            catch (Exception ex)
            {
                result.ProcessingMessage += "API method POST Numbers() failure. Exception: " + ex.Message + ". ";
                EventLogging.WriteEventLogEntry("An error occurred in API.NumbersController.NumbersPost(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets a list of active numbers on the account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /active
        ///
        /// </remarks>
        /// <returns>A list of NumberDetail objects. One for each number assigned to the account.</returns>
        /// <response code="200">Returns a list of numbers assigned to the account</response>
        /// <response code="400">If an error occurred</response>    
        [HttpGet]
        [Authorize]
        [Route("active")]
        [SwaggerResponse(HttpStatusCode.OK, "Message confirmation object", typeof(IEnumerable<NumberDetail>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Active()
        {
            List<NumberDetail> result = new List<NumberDetail>();

            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        List<core.DedicatedVirtualNumber> dvns = da.GetNumbersForAccount(accountId, false);

                        foreach (core.DedicatedVirtualNumber vn in dvns)
                        {
                            NumberDetail nd = new NumberDetail(vn);
                            if (nd != null)
                            {
                                result.Add(nd);
                            }
                        }

                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in API.NumbersController.Active(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, result));
        }
    }
}
