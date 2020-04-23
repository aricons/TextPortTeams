using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class BlockRequest
    {
        public int BlockId { get; set; }

        [Required(ErrorMessage = "A number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number")]
        [Display(Name = "Number to Block")]
        public string MobileNumber { get; set; }

        public string MobileNumberE164
        {
            get { return this.MobileNumber.ToE164(); }
        }

        public MessageDirection Direction { get; set; }

        public RequestStatus SubmissionStatus { get; set; }

        public string SubmissionMessage { get; set; }


        // Constructor
        public BlockRequest()
        {
            this.BlockId = 0;
            this.MobileNumber = string.Empty;
            this.Direction = MessageDirection.Outbound;
            this.SubmissionStatus = RequestStatus.Pending;
            this.SubmissionMessage = string.Empty;
        }
    }
}
