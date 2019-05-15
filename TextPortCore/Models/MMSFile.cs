using System;

namespace TextPortCore.Models
{
    public class MMSFile
    {
        public int FileId { get; set; }
        public int? MessageId { get; set; }
        public int StorageId { get; set; }
        public string FileName { get; set; }
        //public Message Message { get; set; }

        public MMSFile()
        {
            this.FileId = 0;
            this.MessageId = 0;
            this.StorageId = 0;
            this.FileName = string.Empty;
        }
    }
}