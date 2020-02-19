using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.IPData
{
    public class IPDataResult
    {
        [Required(ErrorMessage = "An ip address is required")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Invalid IP address")]
        [Display(Name = "IP Address")]
        public string ip { get; set; }
        public bool is_eu { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string region_code { get; set; }
        public string country_name { get; set; }
        public string country_code { get; set; }
        public string continent_name { get; set; }
        public string continent_code { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string postal { get; set; }
        public string calling_code { get; set; }
        public string flag { get; set; }
        public string emoji_flag { get; set; }
        public string emoji_unicode { get; set; }
        public Asn asn { get; set; }
        public List<Language> languages { get; set; }
        public Currency currency { get; set; }
        public TimeZone time_zone { get; set; }
        public Threat threat { get; set; }
        public string count { get; set; }
    }

    public class Asn
    {
        public string asn { get; set; }
        public string name { get; set; }
        public string domain { get; set; }
        public string route { get; set; }
        public string type { get; set; }
    }

    public class Language
    {
        public string name { get; set; }
        public string native { get; set; }
    }

    public class Currency
    {
        public string name { get; set; }
        public string code { get; set; }
        public string symbol { get; set; }
        public string native { get; set; }
        public string plural { get; set; }
    }

    public class TimeZone
    {
        public string name { get; set; }
        public string abbr { get; set; }
        public string offset { get; set; }
        public bool is_dst { get; set; }
        public DateTime current_time { get; set; }
    }

    public class Threat
    {
        public bool is_tor { get; set; }
        public bool is_proxy { get; set; }
        public bool is_anonymous { get; set; }
        public bool is_known_attacker { get; set; }
        public bool is_known_abuser { get; set; }
        public bool is_threat { get; set; }
        public bool is_bogon { get; set; }
    }
}

