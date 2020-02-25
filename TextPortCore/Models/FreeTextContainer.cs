using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class FreeTextContainer
    {
        public int AccountId { get; set; }

        public string IPAddress { get; set; }

        [Display(Name = "Send From")]
        [Required(ErrorMessage = "You must select a number to send the message from")]
        public int VirtualNumberId { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "A destination number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "A message is required")]
        public string MessageText { get; set; }

        public string Result { get; set; }

        public string SubmissionMessage { get; set; }

        public IEnumerable<SelectListItem> NumbersList { get; set; }

        public FreeTextContainer()
        {
            this.AccountId = 0;
            this.VirtualNumberId = 0;
            this.MobileNumber = string.Empty;
            this.MessageText = string.Empty;
            this.SubmissionMessage = string.Empty;
            this.Result = "ERROR";

            using (TextPortDA da = new TextPortDA())
            {
                this.NumbersList = da.GetFreeNumbersList();
            }
        }

    }
}
