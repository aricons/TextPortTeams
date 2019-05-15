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

    public enum Carriers : int
    {
        [Description("Bandwidth.com")]
        BandWidth = 172
    }

    public enum QueueStatus : int
    {
        [Description("Queued")]
        Queued = 0,
        [Description("Success")]
        Success = 1,
        [Description("Failed")]
        Failed = 2
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
}
