using System;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class Recent
    {
        public int MessageId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Number { get; set; }

        public int ContactId { get; set; }

        public string ContactName { get; set; }

        public int CountryId { get; set; }

        public string NumberDisplayFormat
        {
            get
            {
                return (!string.IsNullOrEmpty(this.ContactName)) ? this.ContactName : Utilities.NumberToDisplayFormat(this.Number, this.CountryId);
            }
        }

        public string Message { get; set; }

        public bool IsActiveMessage { get; set; }
    }
}
