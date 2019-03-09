using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class SupportRequest
    {
        public int SupportId { get; set; }

        [Display(Name = "Request Date")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Transaction Date")]
        public string Category { get; set; }

        [Display(Name = "Requestor's Email")]
        public string RequestorEmail { get; set; }

        [Display(Name = "Requestor's IP")]
        public string Ipaddress { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}