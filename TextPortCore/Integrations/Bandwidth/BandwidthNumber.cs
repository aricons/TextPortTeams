using System;
using System.Configuration;
using System.Collections.Generic;

namespace TextPortCore.Integrations.Bandwidth
{

    //public class BandwidthNumber
    //{
    //    public string number { get; set; }
    //    public string nationalNumber { get; set; }
    //    public string patternMatch { get; set; }
    //    public string city { get; set; }
    //    public string lata { get; set; }
    //    public string rateCenter { get; set; }
    //    public string state { get; set; }
    //}

    //public class BandwidthNumbersList
    //{
    //    public List<BandwidthNumber> Numbers { get; set; }

    //    public BandwidthNumbersList()
    //    {
    //        this.Numbers = new List<BandwidthNumber>();
    //    }
    //}

    // {"Error":null,"TelephoneNumberList":["9492649943","9493034010","9493171262","9493174386","9493174387","9493174388","9493174389","9493174390","9493174391","9493174392","9493174393","9493174394","9493174395","9493174396","9493174397","9493174398","9493174399","9493174401","9493174402","9493174403"],"TelephoneNumberDetailList":null,"ResultCount":20}

    public class SearchResult
    {
        public int ResultCount { get; set; }
        public string Error { get; set; }
        //public TelephoneNumberList TelephoneNumberList { get; set; }
        public List<string> TelephoneNumberList { get; set; }
        public List<string> TelephoneNumberDetailList { get; set; }

        public SearchResult()
        {
            this.Error = string.Empty;
            this.ResultCount = 0;
            this.TelephoneNumberList = new List<string>();
            this.TelephoneNumberDetailList = new List<string>();
        }
    }

    //public class TelephoneNumberList
    //{
    //    public List<string> TelephoneNumber { get; set; }

    //    public TelephoneNumberList()
    //    {
    //        this.TelephoneNumber = new List<string>();
    //    }
    //}


}