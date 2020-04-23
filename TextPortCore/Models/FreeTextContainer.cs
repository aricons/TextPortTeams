using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class FreeTextContainer
    {
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

        public string SessionId { get; set; }

        public List<Message> MessageHistory { get; set; }

        public IEnumerable<SelectListItem> NumbersList { get; set; }

        public FreeTextContainer()
        {
            this.MobileNumber = string.Empty;
            this.MessageText = string.Empty;
            this.MessageHistory = new List<Message>();
            this.SessionId = string.Empty;

            using (TextPortDA da = new TextPortDA())
            {
                this.NumbersList = da.GetFreeNumbersList();
            }
        }

        public FreeTextContainer(string sessionId)
        {
            int freeTextAccountId = Conversion.StringToIntOrZero(ConfigurationManager.AppSettings["FreeTextAccountId"]);

            this.MobileNumber = string.Empty;
            this.MessageText = string.Empty;
            this.SessionId = sessionId;

            using (TextPortDA da = new TextPortDA())
            {
                this.MessageHistory = da.GetMessagesForAccountAndSessionId(freeTextAccountId, sessionId);
                if (this.MessageHistory.Count > 0)
                {
                    this.VirtualNumberId = this.MessageHistory.FirstOrDefault().VirtualNumberId;
                    this.MobileNumber = Utilities.NumberToDisplayFormat(this.MessageHistory.FirstOrDefault().MobileNumber, 22);
                }
                this.NumbersList = da.GetFreeNumbersList();
            }
        }

    }
}
