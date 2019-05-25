using System;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class AccountView
    {
        private readonly TextPortContext _context;

        public Account Account { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        public RequestStatus Status { get; set; }

        public string ConfirmationMessage { get; set; }

        public AccountView(TextPortContext context)
        {
            this._context = context;
            this.Status = RequestStatus.Pending;
            this.ConfirmationMessage = string.Empty;
        }

        public AccountView()
        {
            this.Account = new Account();
            this.TimeZones = new List<SelectListItem>();
            this.Status = RequestStatus.Pending;
            this.ConfirmationMessage = string.Empty;
        }

        public AccountView(TextPortContext context, int accountId)
        {
            this._context = context;
            this.ConfirmationMessage = string.Empty;
            this.Status = RequestStatus.Pending;
            using (TextPortDA da = new TextPortDA())
            {
                this.Account = da.GetAccountById(accountId);
                this.TimeZones = da.GetTimeZones();
                if (this.Account != null)
                {
                    if (!string.IsNullOrEmpty(this.Account.ForwardVnmessagesTo))
                    {
                        this.Account.ForwardVnmessagesTo = Utilities.NumberToDisplayFormat(this.Account.ForwardVnmessagesTo, 22);
                    }
                }
            }
        }
    }
}