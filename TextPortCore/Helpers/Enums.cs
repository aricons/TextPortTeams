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
}
