using System;

namespace TextPortCore.Models
{
    public class NpaNxxCity
    {
        public int CityId { get; set; }

        public short NPA { get; set; }

        public short NXX { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string County { get; set; }

        public string Zip { get; set; }


        public NpaNxxCity()
        {
            this.CityId = 0;
            this.NPA = 0;
            this.NXX = 0;
            this.City = string.Empty;
            this.State = string.Empty;
            this.County = string.Empty;
            this.Zip = string.Empty;
        }
    }
}
