using System;
using System.Web.Http;

using api = TextPortCore.Models.API;

namespace TextPortAPI.Controllers
{
    [RoutePrefix("v1")]
    public class TestsController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("ping")]
        public string Ping()
        {
            return $"TextPort API alive at {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} UTC";
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ping/{id}")]
        public string Ping(string id)
        {
            return $"TextPort API alive. Received \"{id}\" from remote.";
        }

        [HttpPost]
        [Authorize]
        [Route("messagestub/")]
        public string MessageStub(api.Message messageIn)
        {
            return $"TextPort message stub received message {messageIn.MessageText}.";
        }
    }
}
