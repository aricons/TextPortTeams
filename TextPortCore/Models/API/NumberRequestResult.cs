using System;

namespace TextPortCore.Models.API
{
    public class NumberRequestResult
    {
        public string Number { get; set; }
        public bool Success { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ProcessingMessage { get; set; }

        // Constructors
        public NumberRequestResult()
        {
            this.Number = string.Empty;
            this.Success = false;
            this.ExpirationDate = null;
            this.ProcessingMessage = string.Empty;
        }
    }
}
