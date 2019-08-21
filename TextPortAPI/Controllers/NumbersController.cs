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

namespace TextPortAPI.Controllers
{
    [RoutePrefix("v1")]
    public class NumbersController : ApiController
    {
        [HttpGet]
        [Authorize]
        [Route("numbers/{areaCode}")]
        public HttpResponseMessage Numbers(string areaCode)
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
                            List<string> bwNumbers = bw.GetVirtualNumbersList(areaCode, Constants.NumberOfNumbersToPullFromBandwidthForAPI, false);
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
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Numbers")]
        [Route("numbers")]
        public HttpResponseMessage NumbersPost(NumberRequest numberRequest)
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
                            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                        }

                        numberRequest.Number = Utilities.NumberToE164(numberRequest.Number);
                        if (numberRequest.Number.Length != 11)
                        {
                            result.ProcessingMessage = "The number requested is invalid. Please check the length. Numbers should be 10 or 11 digits.";
                            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
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
                                LeasePeriod = numberRequest.LeasePeriod,
                            };

                            // Check the balance
                            decimal balance = da.GetAccountBalance(accountId);
                            decimal requiredAmount = Constants.BaseNumberCost * numberRequest.LeasePeriod;
                            if (balance < requiredAmount)
                            {
                                result.ProcessingMessage = $"The account balance is insufficient. A balance of at least {requiredAmount:C} is required."; ;
                                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                            }

                            using (Bandwidth bw = new Bandwidth())
                            {
                                //if (bw.PurchaseVirtualNumber(regData))
                                //{
                                result.ProcessingMessage += $"Number {regData.VirtualNumber} was successfully purchased for a {regData.LeasePeriod} month period.";
                                result.ExpirationDate = DateTime.UtcNow.AddMonths(regData.LeasePeriod);
                                result.Success = true;
                                //}
                                //else
                                //{
                                //result.ProcessingMessage += $"The request to purchase number {regData.VirtualNumber} failed.'";
                                //result.Success = false;

                                // return Request.CreateResponse(HttpStatusCode.NotFound, result);
                                //}
                            }
                        }
                    }
                }
                else
                {
                    result.ProcessingMessage = "Could not determin TextPort account ID.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                }
            }
            catch (Exception ex)
            {
                result.ProcessingMessage += "API method POST Numbers() failure. Exception: " + ex.Message + ". ";
                EventLogging.WriteEventLogEntry("An error occurred in API.NumbersController.NumbersPost(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
