using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class Country
    {
        public int CountryId { get; set; }

        public int CarrierId { get; set; }

        [Display(Name = "Country Name")]
        public string CountryName { get; set; }

        [Display(Name = "Country Alpha Code")]
        public string CountryAlphaCode { get; set; }

        [Display(Name = "Phone Prefix")]
        public string CountryPhoneCode { get; set; }

        [Display(Name = "Provider")]
        public string Provider { get; set; }

        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        public int SortOrder { get; set; }

        public Carrier Carrier { get; set; }
    }
}
