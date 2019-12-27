using System;
using System.Configuration;
using System.Collections.Generic;

namespace TextPortCore.Integrations.Bandwidth
{
    public class SearchResult
    {
        public int ResultCount { get; set; }
        public string Error { get; set; }
        public List<string> TelephoneNumberList { get; set; }
        public List<string> TelephoneNumberDetailList { get; set; }

        public SearchResult()
        {
            this.Error = string.Empty;
            this.ResultCount = 0;
            this.TelephoneNumberList = new List<string>();
            this.TelephoneNumberDetailList = new List<string>();
        }
    }
}