using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class VirtualNumberCountry
    {
        public int VirtualNumberCountryId { get; set; }

        [Display(Name = "Country Name")]
        public string CountryName { get; set; }

        [Display(Name = "Country Alpha Code")]
        public string CountryAlphaCode { get; set; }

        [Display(Name = "Phone Prefix")]
        public string CountryPhoneCode { get; set; }

        [Display(Name = "Provider")]
        public string Provider { get; set; }

        [Display(Name = "Base Cost")]
        public decimal BaseCost { get; set; }

        [Display(Name = "Monthly Cost")]
        public decimal MonthlyRate { get; set; }

        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }
    }
}
