using System;
using System.Collections.Generic;

namespace TextPortCore.Models
{
    public partial class ErrorLogItem
    {
        public int ErrorLogId { get; set; }
        public DateTime ErrorDateTime { get; set; }
        public string ProgramName { get; set; }
        public string ErrorMessage { get; set; }
        public string Details { get; set; }
    }
}
