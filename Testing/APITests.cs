using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;
using TextPortServices.Processes;
using EmailToSMSGateway;

using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Testing
{
    [TestClass]
    public class APITests
    {
        [TestMethod]
        public async Task SendSMSUsingHttpClient()
        {
            string jsonData = @"[ { 'From': '19493174450', 'To': '19492339386', 'MessageText': 'Hello world' } ]";

            // Initialize an instance of the HTTP client
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.textport.com");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/messages/send");

                // Add the credentials using basic authentication. This will generate an HTTP header like:
                string apiToken = "1-oqY49CEG8s";
                string apiSecret = "WjNMrkkzfqKnt0Wg5mWd";
                byte[] byteArray = new UTF8Encoding().GetBytes($"{apiToken}:{apiSecret}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Add the JSON content to the request
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);

                string jsonResult = await response.Content.ReadAsStringAsync();
            }
        }

        [TestMethod]
        public void SendSMSUsingWebClient()
        {
            string jsonData = @"[ { 'From': '19493174450', 'To': '19492339386', 'MessageText': 'Hello world.' } ]";
            string apiToken = "1-oqY49CEG8s";
            string apiSecret = "WjNMrkkzfqKnt0Wg5mWd";

            using (WebClient client = new WebClient())
            {
                // Add the apiToken and apiSecret as credentials
                client.Credentials = new NetworkCredential(apiToken, apiSecret);
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                // Send the request
                string response = client.UploadString(new Uri("https://api.textport.com/v1/messages/send"), "POST", jsonData);
            }
        }

    }
}