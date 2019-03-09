using System;
using System.Collections.Generic;
using System.Text;

namespace TextPortCore.Models.PayPal
{
    public partial class AuthTokenResponse
    {
        public string scope { get; set; }
        public string nonce { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public int expires_in { get; set; }
    }

    public class PurchaseDetail
    {
        public string intent { get; set; }
        public RedirectUrls redirect_urls { get; set; }
        public Payer payer { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class RedirectUrls
    {
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }

    public class Payer
    {
        public string payment_method { get; set; }
    }

    public class Amount
    {
        public string total { get; set; }
        public string currency { get; set; }
    }

    public class Transaction
    {
        public Amount amount { get; set; }
    }
}
   
