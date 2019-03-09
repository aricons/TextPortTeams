using System;
using System.Collections.Generic;

namespace TextPortCore
{
    public partial class Accounts
    {
        public int AccountId { get; set; }
        public bool Enabled { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public short TimeZoneId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public int LoginCount { get; set; }
        public int Credits { get; set; }
        public string NotificationsEmailAddress { get; set; }
        public string ForwardVnmessagesTo { get; set; }
        public int MessageOutCount { get; set; }
        public int MessageInCount { get; set; }
        public string AccountValidationKey { get; set; }
        public bool AccountValidated { get; set; }
        public bool Deleted { get; set; }
        public string PasswordSave { get; set; }
        public string PasswordOldDecrypt { get; set; }
    }
}
