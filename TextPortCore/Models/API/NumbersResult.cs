using System.Collections.Generic;

namespace TextPortCore.Models.API
{
    public class NumbersResult
    {
        public string AreaCode { get; set; }
        public int NumberCount { get; set; }
        public string Message { get; set; }
        public List<string> Numbers { get; set; }

        // Constructors
        public NumbersResult()
        {
            this.AreaCode = string.Empty;
            this.Message = string.Empty;
            this.NumberCount = 0;
            this.Numbers = new List<string>();
        }

        public NumbersResult(string areaCode)
        {
            this.AreaCode = areaCode;
            this.NumberCount = 0;
            this.Message = string.Empty;
            this.Numbers = new List<string>();
        }
    }
}
