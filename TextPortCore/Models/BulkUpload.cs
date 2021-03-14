using System;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class BulkUpload
    {
        public int AccountId { get; set; }
        public int VirtualNumberId { get; set; }
        public decimal Balance { get; set; }
        public string SubmitOperation { get; set; }
        public string ProcessingState { get; set; }
        public string BalanceAlert { get; set; }
        public List<SelectListItem> VirtualNumbers { get; set; }
        public List<BulkUploadMessageItem> Messages { get; set; }

        public BulkUpload()
        {
            this.AccountId = 0;
            this.VirtualNumberId = 0;
            this.Balance = 0;
            this.SubmitOperation = string.Empty;
            this.ProcessingState = string.Empty;
            this.BalanceAlert = string.Empty;
            this.VirtualNumbers = new List<SelectListItem>();
            this.Messages = new List<BulkUploadMessageItem>();
        }

        public BulkUpload(int accountId)
        {
            this.AccountId = accountId;
            this.VirtualNumberId = 0;
            this.SubmitOperation = string.Empty;
            this.ProcessingState = string.Empty;
            this.Messages = new List<BulkUploadMessageItem>();
            this.VirtualNumbers = new List<SelectListItem>();

            using (TextPortDA da = new TextPortDA())
            {
                this.Balance = 0;
                List<DedicatedVirtualNumber> dvns = da.GetNumbersForBranch(accountId, false);
                foreach (DedicatedVirtualNumber dvn in dvns)
                {
                    this.VirtualNumbers.Add(new SelectListItem()
                    {
                        Value = dvn.VirtualNumberId.ToString(),
                        Text = dvn.NumberDisplayFormat
                    });
                };
            }
        }
    }

    public class BulkUploadMessageItem
    {
        public string Number { get; set; }
        public string Message { get; set; }

        public BulkUploadMessageItem()
        {
            this.Number = string.Empty;
            this.Message = string.Empty;
        }
    }
}
