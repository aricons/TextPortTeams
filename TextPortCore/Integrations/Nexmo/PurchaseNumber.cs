namespace TextPortCore.Integrations.Nexmo
{
    public class PurchaseNumber
    {
        public string country { get; set; }
        public string msisdn { get; set; }
        public string target_api_key { get; set; }

        public PurchaseNumber()
        {
            this.country = string.Empty;
            this.msisdn = string.Empty;
            this.target_api_key = string.Empty;
        }

        public PurchaseNumber(string countryCode, string number)
        {
            this.country = countryCode;
            this.msisdn = number;
            this.target_api_key = string.Empty;
        }
    }
}