using System;

namespace TextPortCore.Models
{
    public class ZipLatLong
    {
        public int ZipId { get; set; }

        public string Zip { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }


        public ZipLatLong()
        {
            this.ZipId = 0;
            this.Zip = string.Empty;
            this.Latitude = 0;
            this.Longitude = 0;
        }
    }
}