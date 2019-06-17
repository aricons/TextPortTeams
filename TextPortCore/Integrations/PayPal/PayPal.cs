using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

using RestSharp;
using RestSharp.Authenticators;

using TextPortCore.Models.PayPal;

namespace TextPortCore.Integrations
{
    public static class PayPalAPI
    {
        // Sandbox
        private static string PayPalAPIUrl = "https://api.sandbox.paypal.com";
        private static string accountId = "richard-facilitator@arionconsulting.com";
        private static string clientId = "Ac6TTAGnmNme-wJJSdgU6rm8SSyW5nSc757nHhsqNWDz3X7lOa8Yx3eE-96JK-Z2YbN3N54PE_oRRGbO";
        private static string secret = "EAIJ2JnRuYFVTi1_3mP6U4CXf1oKV00RXYlVQ7X7VS5QYdCUpCPcJhkJBkj4M3DiI1B_PwSXFU_Jf_dM";

        // Production
        //private static string PayPalAPIUrl = "https://api.paypal.com";
        //private static string accountId = "richard@arionconsulting.com";
        //private static string clientId = "AUjK3Zugk_dkKu2ScI-f1S-8Ibxu99MeiuS9MSzogMOiEyKUa8Q4kz5L-wHfMxqZhF3p7ZjIm_64Ju_q";
        //private static string secret = "EK8w3u2PUstKzs2Du8ioX07ErTEP1ZGD5iLwIN5zcueeiiZLfMKky6rD5Or733S1dnKliJD5cErtv17_";

        

        public static string SubmitPurchase(PurchaseDetail purchase)
        {
            var client = new RestClient(PayPalAPIUrl);
            
            string accessToken = getAccessToken();

            var request = new RestRequest("/v1/payments/payment", Method.POST);
            request.AddParameter("content-type", "application/json"); // adds to POST or URL querystring based on Method
            request.AddHeader("authorization", string.Format("bearer {0}", accessToken));

            // execute the request and get a plain string.
            //IRestResponse response = client.Execute(request);
            //var content = response.Content; // raw content as string

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            IRestResponse<AuthTokenResponse> token = client.Execute<AuthTokenResponse>(request);
            accessToken = token.Data.access_token;

            // easy async support
            //client.ExecuteAsync(request, response => {
            //    Console.WriteLine(response.Content);
            //});

            // async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
            //    Console.WriteLine(response.Data.Name);
            //});

            //// abort the request on demand
            //asyncHandle.Abort();

            return accessToken;
        }

        private static string getAccessToken()
        {
            string accessToken = string.Empty;
            var client = new RestClient(PayPalAPIUrl);
            client.Authenticator = new HttpBasicAuthenticator(clientId, secret);

            var request = new RestRequest("/v1/oauth2/token", Method.POST);
            request.AddParameter("grant_type", "client_credentials"); // adds to POST or URL querystring based on Method
            //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource
            request.AddHeader("content-type", "application/x-www-form-urlencoded");

            // execute the request and get a plain string.
            //IRestResponse response = client.Execute(request);
            //var content = response.Content; // raw content as string

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            IRestResponse<AuthTokenResponse> token = client.Execute<AuthTokenResponse>(request);
            accessToken = token.Data.access_token;

            // easy async support
            //client.ExecuteAsync(request, response => {
            //    Console.WriteLine(response.Content);
            //});

            // async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
            //    Console.WriteLine(response.Data.Name);
            //});

            //// abort the request on demand
            //asyncHandle.Abort();

            return accessToken;
        }





    }
}
