using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.API
{
    public class NumbersResult
    {
        /// <summary>The area code of the numbers contained in the Numbers list.</summary>
        [Required]
        public string AreaCode { get; set; }

        /// <summary>The count of available numbers.</summary>
        [Required]
        public int NumberCount { get; set; }

        /// <summary>User notification.</summary>
        [Required]
        public string Message { get; set; }

        /// <summary>The list of availble numbers.</summary>
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
