using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class BulkMessages
    {
        private readonly TextPortContext _context;

        public BulkMessages(TextPortContext context)
        {
            this._context = context;
        }

        private List<int> gridSizeOptions = new List<int>() { 5, 10, 15, 20, 50, 75, 100, 150, 200 };

        public int AccountId { get; set; }

        [Required(ErrorMessage = "A virtual number must be selected")]
        [Display(Name = "Send From Number")]
        public int VirtualNumberId { get; set; }

        [Display(Name = "Number of Rows")]
        public int MessageLimit { get; set; }

        [Display(Name = "Same Message to All Numbers")]
        public bool SameMessageToAllNumbers { get; set; }

        public List<SelectListItem> VirtualNumbers { get; set; }

        public List<SelectListItem> MessageCountOptions { get; set; }

        public List<BulkMessageItem> Messages { get; set; }

        public string SubmitOperation { get; set; }

        public string ProcessingState { get; set; }

        /* Constructors */
        public BulkMessages()
        {
            this.AccountId = 0;
            this.MessageLimit = 0;
            this.Messages = new List<BulkMessageItem>();
            this.MessageCountOptions = new List<SelectListItem>();
            this.VirtualNumbers = new List<SelectListItem>();
            this.ProcessingState = string.Empty;
        }

        public BulkMessages(TextPortContext context, int accId, int gridRows)
        {
            this._context = context;
            this.AccountId = accId;
            this.MessageLimit = gridRows;
            this.Messages = new List<BulkMessageItem>();
            this.MessageCountOptions = new List<SelectListItem>();
            this.VirtualNumbers = new List<SelectListItem>();

            List<DedicatedVirtualNumber> dvns = _context.DedicatedVirtualNumbers.Where(x => x.AccountId == accId && !x.Cancelled).OrderByDescending(x => x.VirtualNumberId).ToList();
            foreach (DedicatedVirtualNumber dvn in dvns)
            {
                this.VirtualNumbers.Add(new SelectListItem()
                {
                    Value = dvn.VirtualNumberId.ToString(),
                    Text = dvn.NumberLocalFormat
                });
            };

            foreach (int opt in this.gridSizeOptions)
            {
                this.MessageCountOptions.Add(new SelectListItem()
                {
                    Value = opt.ToString(),
                    Text = opt.ToString()
                });
            };

            for (int x = 0; x < this.MessageLimit; x++)
            {
                this.Messages.Add(new BulkMessageItem());
            }

            this.ProcessingState = "PENDING";
        }
    }

    public class BulkMessageItem
    {
        public string Number { get; set; }
        public string MessageText { get; set; }
        public string ProcessingStatus { get; set; }
        public string ProcessingResult { get; set; }
    }
}
