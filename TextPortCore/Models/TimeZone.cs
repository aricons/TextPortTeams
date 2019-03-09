using System;

namespace TextPortCore.Models
{
    public partial class TimeZone
    {
        public short TimeZoneId { get; set; }
        public decimal Utcoffset { get; set; }
        public string Name { get; set; }
    }
}
