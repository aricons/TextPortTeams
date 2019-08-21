using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using APIEndpointSimulator.Helpers;
using TextPortCore.Models.API;

namespace APIEndpointSimulator.Controllers
{
    public class EventsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Ping()
        {
            return Ok("Pong");
        }

        [HttpPost]
        public IHttpActionResult MessageEvent([FromBody] MessageEvent message)
        {
            string logText = string.Empty;

            if (message != null)
            {
                switch (message.EventType)
                {
                    case "message-received":
                        logText += $"Processing API event {message.EventType}\r\n";
                        logText += $"Message ID: {message.MessageId}\r\n";
                        logText += $"From: {message.Message.From}\r\n";
                        logText += $"To: {message.Message.To}\r\n";
                        logText += $"Message: {message.Message.MessageText}\r\n";
                        logText += $"Notifications: {message.Notifications}\r\n";
                        Utilities.WriteLogTextToDisk(logText);
                        return Ok();

                    case "message-delivered":
                        logText += $"Processing API event {message.EventType}\r\n";
                        logText += $"Message ID: {message.MessageId}\r\n";
                        logText += $"From: {message.Message.From}\r\n";
                        logText += $"To: {message.Message.To}\r\n";
                        logText += $"Message: {message.Message.MessageText}\r\n";
                        logText += $"Notifications: {message.Notifications}\r\n";
                        Utilities.WriteLogTextToDisk(logText);
                        return Ok();
                }
            }
            return NotFound();
        }
    }
}
