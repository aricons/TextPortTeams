using System;

namespace TextPortCore.Models
{
    public class NpaNxxThou
    {
        public int Id { get; set; }

        public short NPA { get; set; }

        public short NXX { get; set; }

        public string Thousands { get; set; }

        public string State { get; set; }

        public string Company { get; set; }

        public string RateCenter { get; set; }

        public string CLLI { get; set; }

        public string PrefixType { get; set; }

        public string LATA { get; set; }


        public NpaNxxThou()
        {
            this.Id = 0;
            this.NPA = 0;
            this.NXX = 0;
            this.Thousands = string.Empty;
            this.State = string.Empty;
            this.Company = string.Empty;
            this.RateCenter = string.Empty;
            this.CLLI = string.Empty;
            this.PrefixType = string.Empty;
            this.LATA = string.Empty;
        }
    }
}
