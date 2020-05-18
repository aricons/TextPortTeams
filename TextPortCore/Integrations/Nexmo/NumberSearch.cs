namespace TextPortCore.Integrations.Nexmo
{
    public class NumberSearch
    {
        string country { get; set; }
        string type { get; set; }
        string features { get; set; }
        string pattern { get; set; }
        string search_pattern { get; set; }

        public NumberSearch()
        {
            this.country = "GB";
            this.type = "mobile-lvn";
            this.features = "SMS";
            this.pattern = string.Empty;
            this.search_pattern = string.Empty;
        }

        public NumberSearch(string countryCode)
        {
            this.country = countryCode;
            this.type = "mobile-lvn";
            this.features = "SMS";
            this.pattern = string.Empty;
            this.search_pattern = string.Empty;
        }
    }
}
