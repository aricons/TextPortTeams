using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.API
{
    public class NumberDetail
    {
        [Required]
        /// <summary>Your TextPort virtual number</summary>
        public string Number { get; set; }

        [Required]
        /// <summary>Date the number was assigned to your account</summary>
        public DateTime DateAssigned { get; set; }

        [Required]
        /// <summary>Date that the number lease expires</summary>
        public DateTime ExpirationDate { get; set; }

        [Required]
        /// <summary>The number of times the leaase has been extended</summary>
        public int RenewalCount { get; set; }

        /// <summary>Name of the API application that the number is assigned to</summary>
        //public string APIApplicationName { get; set; }

        // Constructors
        public NumberDetail()
        {
            this.Number = string.Empty;
            this.DateAssigned = DateTime.MinValue;
            this.ExpirationDate = DateTime.MinValue;
            this.RenewalCount = 0;
            //this.APIApplicationName = string.Empty;
        }

        public NumberDetail(DedicatedVirtualNumber vn)
        {
            this.Number = vn.VirtualNumber;
            this.DateAssigned = vn.CreateDate;
            this.ExpirationDate = vn.ExpirationDate;
            this.RenewalCount = vn.RenewalCount;
            //this.APIApplicationName = vn.APIApplicationId.ToString();
        }
    }
}