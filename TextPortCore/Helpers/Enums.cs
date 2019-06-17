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
        [Description("Notification")]
        Notification = 2
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
        Failed = 2
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
}
