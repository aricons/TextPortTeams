using System;

namespace TextPortCore.Models
{
    public class CensoredWord
    {
        public int WordId { get; set; }
        public string Word { get; set; }
        public string Replacement { get; set; }

        public CensoredWord()
        {
            this.WordId = 0;
            this.Word = string.Empty;
            this.Replacement = string.Empty;
        }
    }
}