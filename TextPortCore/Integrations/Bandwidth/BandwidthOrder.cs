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
            this.Name = $"VN {regData.BranchId} {number}";
            this.CustomerOrderId = $"{regData.BranchId}-{number}";
            this.SiteId = siteId;
            this.ExistingTelephoneNumberOrderType = new ExistingTelephoneNumberOrderTypeItem(number);
            this.PartialAllowed = false;
            this.BackOrderRequested = false;
            if (!string.IsNullOrEmpty(regData.NumberReservationId))
            {
                this.ExistingTelephoneNumberOrderType.ReservationIdList = new ReservationIdListItem(regData.NumberReservationId);
            }
        }
    }

    [Serializable()]
    public class ExistingTelephoneNumberOrderTypeItem
    {
        public TelephoneNumberItem TelephoneNumberList { get; set; }

        public ReservationIdListItem ReservationIdList { get; set; }

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

    [Serializable()]
    public class ReservationIdListItem
    {
        public string ReservationId { get; set; }

        // Default constructor
        public ReservationIdListItem()
        {
            this.ReservationId = string.Empty;
        }

        // Constructor for a reservation id
        public ReservationIdListItem(string reservationId)
        {
            this.ReservationId = reservationId;
        }
    }

    //[Serializable()]
    //public class ReservationIdItem
    //{
    //    public string ReservationId { get; set; }

    //    // Default constructor
    //    public ReservationIdItem()
    //    {
    //        this.ReservationId = string.Empty;
    //    }

    //    // Constructor for a reservation id
    //    public ReservationIdItem(string reservationId)
    //    {
    //        this.ReservationId = reservationId;
    //    }
    //}
}