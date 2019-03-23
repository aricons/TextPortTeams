using System;

namespace TextPortCore.Models
{
    public class MMSFile
    {
        public string FileName { get; set; }
        public byte[] DataBytes { get; set; }

        public MMSFile()
        {
            this.FileName = string.Empty;
            this.DataBytes = null;
        }
    }
}