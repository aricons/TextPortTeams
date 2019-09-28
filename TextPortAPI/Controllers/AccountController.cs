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
    /// Handles send account-related requests for the TextPort SMS API.
    /// </summary>
    [RoutePrefix("v1/account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// Gets the current balance on the account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /balance
        ///
        /// </remarks>
        /// <returns>A BalanceResult object containing the current balance on the account.</returns>
        /// <response code="200">A BalanceResult object containing the current balance on the account.</response>
        /// <response code="400">If an error occurred</response>    
        [HttpGet]
        [Authorize]
        [Route("balance")]
        [SwaggerResponse(HttpStatusCode.OK, "Message confirmation object", typeof(BalanceResult))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Balance()
        {
            BalanceResult result = new BalanceResult();

            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        core.Account acc = da.GetAccountById(accountId);
                        result = new BalanceResult(acc);

                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in API.AccountController.Balance(). Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            return BadRequest();
        }
    }
}