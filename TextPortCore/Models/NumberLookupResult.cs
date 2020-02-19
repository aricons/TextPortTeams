using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public class NumberLookupResult
    {
        [Required(ErrorMessage = "A phone number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }

        public bool HasResult { get; set; }

        public int NpaNxxId { get; set; }

        public short NPA { get; set; }

        public short NXX { get; set; }

        public string Thousands { get; set; }

        public string State { get; set; }

        public string Company { get; set; }

        public string RateCenter { get; set; }

        public string CLLI { get; set; }

        public string PrefixType { get; set; }

        public string LATA { get; set; }

        public int CityId { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Zip { get; set; }

        public int? ZipId { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public bool MatchFound { get; set; }

        public string Message { get; set; }


        public NumberLookupResult()
        {
            this.Number = string.Empty;
            this.HasResult = false;
            this.NpaNxxId = 0;
            this.NPA = 0;
            this.NXX = 0;
            this.Thousands = string.Empty;
            this.State = string.Empty;
            this.Company = string.Empty;
            this.RateCenter = string.Empty;
            this.CLLI = string.Empty;
            this.PrefixType = string.Empty;
            this.LATA = string.Empty;
            this.CityId = 0;
            this.City = string.Empty;
            this.County = string.Empty;
            this.ZipId = 0;
            this.Zip = string.Empty;
            this.Latitude = 0;
            this.Longitude = 0;
            this.MatchFound = false;
            this.Message = string.Empty;
        }
    }
}
