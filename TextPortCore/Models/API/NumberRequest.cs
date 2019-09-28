using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.API
{
    public class NumberRequest
    {
        /// <summary>The number you wish to purchase.</summary>
        [Required]
        public string Number { get; set; }

        /// <summary>The number of months to lease the number for.</summary>
        [Required]
        public int LeasePeriod { get; set; }

        // Constructors
        public NumberRequest()
        {
            this.Number = string.Empty;
            this.LeasePeriod = 1;
        }
    }
}


