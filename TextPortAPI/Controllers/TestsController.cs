using System;
using System.Web.Http;
using System.Net;
using System.Security.Claims;
using System.Collections.Generic;

using api = TextPortCore.Models.API;

using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Data;

using Swashbuckle.Swagger.Annotations;

namespace TextPortAPI.Controllers
{
    /// <summary>
    /// TextPort API connectivity tests controller.
    /// </summary>
    [RoutePrefix("v1")]
    public class TestsController : ApiController
    {
        /// <summary>
        /// Ping test to verify basic connectivity.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /ping
        ///
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [Route("ping")]
        public string Ping()
        {
            return $"TextPort API alive at {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} UTC";
        }

        /// <summary>
        /// Ping test to verify basic connectivity and echo back the parameter passed in.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /ping "echo"
        ///
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [Route("ping/{id}")]
        public string Ping(string id)
        {
            return $"TextPort API alive. Received \"{id}\" from remote.";
        }

        /// <summary>
        /// Tests basic authorization.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /checkauth echo
        ///
        /// </remarks>
        /// <param name="id">A string to be echoed back to the calling client.</param>
        /// <returns>A message indicating the success of the authorization attempt.</returns>
        /// <response code="200">A message confirming whehter the authentication passed.</response>
        /// <response code="400">If an error occurs</response>   
        /// <response code="401">An unauthorized response</response>   
        [HttpGet]
        [Authorize]
        [Route("checkauth/{id}")]
        public IHttpActionResult CheckAuth(string id)
        {
            try
            {
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        Account acc = da.GetAccountById(accountId);
                        if (acc != null)
                        {
                            if (acc.AccountId == accountId)
                            {
                                return Ok($"Authorization successful. Value passed was {id}");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest("An error occurred");
            }

            return Unauthorized();
        }

        /// <summary>
        /// Test method used to confirm messages can be successfully passed. Requires authentication.
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
        /// <param name="messages">A list of messages</param>
        /// <returns>A list of MessageResult objects. One for each message passed in.</returns>
        /// <response code="200">A list of MessageResult objects</response>
        /// <response code="400">If an error occurred</response>    
        [HttpPost]
        [Authorize]
        [Route("messagestub/")]
        [SwaggerResponse(HttpStatusCode.OK, "Message confirmation object", typeof(IEnumerable<api.MessageResult>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult MessageStub([FromBody]List<api.Message> messages)
        {
            if (messages != null)
            {
                List<api.MessageResult> results = new List<api.MessageResult>();

                foreach (api.Message msg in messages)
                {
                    api.MessageResult resItem = new api.MessageResult(msg, "OK", 0, "Test message item received OK. Test only. Not delivered.", 0);
                    results.Add(resItem);
                }

                return Ok(results);
            }

            return BadRequest();
        }
    }
}
