using System;
using System.Collections.Generic;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Integrations.Bandwidth
{
    [Serializable()]
    public class Order
    {
        public string Name { get; set; }
        public string CustomerOrderId { get; set; }
        public string SiteId { get; set; }
        public bool PartialAllowed { get; set; }
        public bool BackOrderRequested { get; set; }
        public ExistingTelephoneNumberOrderTypeItem ExistingTelephoneNumberOrderType { get; set; }
        public DisconnectTelephoneNumberOrderTypeItem DisconnectTelephoneNumberOrderType { get; set; }

        // Default constructor
        public Order()
        {
            this.Name = string.Empty;
            this.SiteId = string.Empty;
            this.CustomerOrderId = string.Empty;
            this.PartialAllowed = false;
            this.BackOrderRequested = false;
            this.ExistingTelephoneNumberOrderType = new ExistingTelephoneNumberOrderTypeItem();
            this.DisconnectTelephoneNumberOrderType = new DisconnectTelephoneNumberOrderTypeItem();
        }

        // Constructor for RegData
        public Order(RegistrationData regData, string siteId)
        {
            string number = Utilities.NumberToBandwidthFormat(regData.VirtualNumber);
            this.Name = $"VN {regData.AccountId} {number}";
            this.CustomerOrderId = $"{regData.AccountId}-{number}";
            this.SiteId = siteId;
            this.ExistingTelephoneNumberOrderType = new ExistingTelephoneNumberOrderTypeItem(number);
            this.PartialAllowed = false;
            this.BackOrderRequested = false;
        }
    }

    [Serializable()]
    public class ExistingTelephoneNumberOrderTypeItem
    {
        public TelephoneNumberItem TelephoneNumberList { get; set; }

        public ExistingTelephoneNumberOrderTypeItem()
        {
            this.TelephoneNumberList = new TelephoneNumberItem();
        }

        public ExistingTelephoneNumberOrderTypeItem(string number)
        {
            this.TelephoneNumberList = new TelephoneNumberItem()
            {
                TelephoneNumber = number
            };
        }
    }

    [Serializable()]
    public class TelephoneNumberItem
    {
        public string TelephoneNumber { get; set; }

        // Default constructor
        public TelephoneNumberItem()
        {
            this.TelephoneNumber = string.Empty;
        }

        // Constructor for a number
        public TelephoneNumberItem(string number)
        {
            this.TelephoneNumber = number;
        }
    }
}