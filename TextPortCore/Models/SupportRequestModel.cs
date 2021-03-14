using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class SupportRequestModel
    {
        public int SupportId { get; set; }

        public SupportRequestType RequestType { get; set; }

        [Display(Name = "Request Date")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Request Type")]
        public string Category { get; set; }

        [Required(ErrorMessage = "A name is required")]
        [Display(Name = "Your Name")]
        public string RequestorName { get; set; }

        [Required(ErrorMessage = "An email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Your Email")]
        public string RequestorEmail { get; set; }

        [Display(Name = "Requestor's IP")]
        public string Ipaddress { get; set; }

        [Display(Name = "Sending Number")]
        public string SendingNumber { get; set; }

        [Display(Name = "Receiving Number")]
        public string ReceivingNumber { get; set; }

        [Required(ErrorMessage = "A message is required")]
        [Display(Name = "Message")]
        public string Message { get; set; }

        public RequestStatus SubmissionStatus { get; set; }

        public string SubmissionMessage { get; set; }

        public IEnumerable<SelectListItem> CategoriesList { get; set; }


        /* Constructors */

        public SupportRequestModel()
        {
            this.SupportId = 0;
            this.RequestType = SupportRequestType.Contact;
            this.TimeStamp = DateTime.UtcNow;
            this.Category = string.Empty;
            this.RequestorName = string.Empty;
            this.RequestorEmail = string.Empty;
            this.Ipaddress = string.Empty;
            this.SendingNumber = string.Empty;
            this.ReceivingNumber = string.Empty;
            this.Message = string.Empty;
            this.SubmissionStatus = RequestStatus.Pending;
            this.SubmissionMessage = string.Empty;
            this.CategoriesList = new List<SelectListItem>();
        }

        public SupportRequestModel(SupportRequestType requestType)
        {
            this.SupportId = 0;
            this.RequestType = requestType;
            this.TimeStamp = DateTime.UtcNow;
            this.Category = string.Empty;
            this.RequestorName = string.Empty;
            this.RequestorEmail = string.Empty;
            this.Ipaddress = string.Empty;
            this.SendingNumber = string.Empty;
            this.ReceivingNumber = string.Empty;
            this.Message = string.Empty;
            this.SubmissionStatus = RequestStatus.Pending;
            this.SubmissionMessage = string.Empty;
            using (TextPortDA da = new TextPortDA())
            {
                this.CategoriesList = null; //da.GetSupportCategoriesList(requestType);
            }
        }
    }
}
