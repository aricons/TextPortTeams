using System;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class EmailToSMSContainer
    {
        public int AccountId { get; set; }

        public List<SelectListItem> VirtualNumbersList { get; set; }

        public List<EmailToSMSAddress> EmailAddressList { get; set; }

        public EmailToSMSAddress NewAddress { get; set; }

        // Constructors
        public EmailToSMSContainer()
        {
            this.AccountId = 0;
            this.VirtualNumbersList = new List<SelectListItem>();
            this.EmailAddressList = new List<EmailToSMSAddress>();
            this.NewAddress = new EmailToSMSAddress();
        }

        public EmailToSMSContainer(int accountId)
        {
            this.AccountId = accountId;
            this.VirtualNumbersList = new List<SelectListItem>();
            this.NewAddress = new EmailToSMSAddress();

            using (TextPortDA da = new TextPortDA())
            {
                List<DedicatedVirtualNumber> dvns = da.GetNumbersForAccount(accountId, true);
                foreach (DedicatedVirtualNumber dvn in dvns)
                {
                    this.VirtualNumbersList.Add(new SelectListItem()
                    {
                        Value = dvn.VirtualNumberId.ToString(),
                        Text = dvn.NumberDisplayFormat
                    });
                };

                this.EmailAddressList = da.GetEmailToSMSAddressesForAccount(accountId);
            }
        }
    }
}
