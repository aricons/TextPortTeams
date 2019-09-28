using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.API
{
    public class NumberRequestResult
    {
        /// <summary>The requested number</summary>
        [Required]
        public string Number { get; set; }

        /// <summary>Boolean flag indicating whether the request was successful or otherwise.</summary>
        [Required]
        public bool Success { get; set; }

        /// <summary>Expiration date of the new number.</summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>User notification message.</summary>
        [Required]
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
