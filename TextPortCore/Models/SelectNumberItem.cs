using System;

namespace TextPortCore.Models
{
    public class SelectNumberItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string CountryCode { get; set; }
        public string ImageUrl { get; set; }

        public SelectNumberItem()
        {
            this.Value = string.Empty;
            this.Text = string.Empty;
            this.CountryCode = string.Empty;
            this.ImageUrl = string.Empty;
        }
    }
}
