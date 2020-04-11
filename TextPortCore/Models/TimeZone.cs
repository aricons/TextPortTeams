using System;

namespace TextPortCore.Models
{
    public partial class TimeZone
    {
        public int TimeZoneId { get; set; }
        public decimal Utcoffset { get; set; }
        public string Name { get; set; }
    }
}
