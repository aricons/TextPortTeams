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
        EmailToSMS = 8,
        [Description("FreeTextSend")]
        FreeTextSend = 9
    }

    public enum EventTypes : byte
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("MessageReceived")]
        MessageReceived = 1,
        [Description("MessageDelivered")]
        MessageDelivered = 2,
        [Description("MessageFailed")]
        MessageFailed = 3
    }

    public enum NumberTypes : byte
    {
        [Description("Regular")]
        Regular = 1,
        [Description("Pooled")]
        Pooled = 2,
        [Description("Free")]
        Free = 3,
        [Description("Reserved")]
        Reserved = 4
    }

    public enum Countries : int
    {
        [Description("United States")]
        UnitedStates = 1,
        [Description("United Kingdom")]
        UnitedKingdom = 44,
        [Description("Germany")]
        Germany = 49,
        [Description("Australia")]
        Australia = 61,
        [Description("New Zealand")]
        NewZealand = 64
    }

    public enum Carriers : int
    {
        [Description("Bandwidth.")]
        BandWidth = 1,
        [Description("InfoBip")]
        InfoBip = 2,
        [Description("Nexmo")]
        Nexmo = 3
    }

    public enum QueueStatuses : byte
    {
        [Description("NotProcessed")]
        NotProcessed = 0,
        [Description("Queued")]
        Queued = 1,
        [Description("SentToProvider")]
        SentToProvider = 2,
        [Description("SendToProviderFailed")]
        SendToProviderFailed = 3,
        [Description("DeliveryConfirmed")]
        DeliveryConfirmed = 4,
        [Description("DeliveryFailed")]
        DeliveryFailed = 5,
        [Description("InternalFailure")]
        InternalFailure = 6,
        [Description("Received")]
        Received = 10
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
