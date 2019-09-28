using System;
using System.ComponentModel;

namespace TextPortCore.Helpers
{
    public enum MessageDirection : int
    {
        [Description("Message outbound from TextPort")]
        Outbound = 0,
        [Description("Message inbound to TextPort")]
        Inbound = 1
    }

    public enum MessageTypes : byte
    {
        [Description("Normal")]
        Normal = 0,
        [Description("Bulk")]
        Bulk = 1,
        [Description("BulkUpload")]
        BulkUpload = 2,
        [Description("Notification")]
        Notification = 3,
        [Description("Group")]
        Group = 4,
        [Description("API")]
        API = 5,
        [Description("SVC")]
        SVC = 6,
        [Description("ASMX")]
        ASMX = 7,
        [Description("EmailToSMS")]
        EmailToSMS = 8
    }

    public enum NumberTypes : byte
    {
        [Description("Regular")]
        Regular = 1,
        [Description("Pooled")]
        Pooled = 2,
        [Description("Reserved")]
        Reserved = 3
    }

    public enum Countries : int
    {
        [Description("United States")]
        UnitedStates = 22
    }

    public enum Carriers : int
    {
        [Description("Bandwidth.com")]
        BandWidth = 172
    }

    public enum QueueStatuses : byte
    {
        [Description("Queued")]
        Queued = 0,
        [Description("Success")]
        Success = 1,
        [Description("Failed")]
        Failed = 2,
        [Description("Received")]
        Received = 3
    }

    public enum RequestStatus : int
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Success")]
        Success = 1,
        [Description("Failed")]
        Failed = 2,
        [Description("CaptchaFailed")]
        CaptchaFailed = 3
    }

    public enum ProcessingStates : int
    {
        [Description("Unprocessed")]
        Unprocessed = 0,
        [Description("Submitted")]
        Submitted = 1,
        [Description("ProcessedSuccessfully")]
        ProcessedSuccessfully = 2,
        [Description("Failed")]
        Failed = 3
    }

    public enum ImageStorageRepository : int
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Temporary")]
        Temporary = 1,
        [Description("Recent")]
        Recent = 2,
        [Description("Archive")]
        Archive = 3
    }

    public enum SupportRequestType : byte
    {
        [Description("Contact")]
        Contact = 0,
        [Description("Support")]
        Support = 1
    }

    public enum ComplimentaryNumberStatus : byte
    {
        [Description("NotEligible")]
        NotEligible = 0,
        [Description("Eligible")]
        Eligible = 1,
        [Description("FailureEligible")]
        FailureEligible = 2,
        [Description("Claimed")]
        Claimed = 3
    }
}
