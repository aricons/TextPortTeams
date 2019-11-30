using System;
using System.Collections.Generic;

using TextPortCore.Models;

namespace TextPortCore.Integrations.Bandwidth
{
    [Serializable()]
    public class DisconnectTelephoneNumberOrder
    {
        public string Name { get; set; }
        //public string CustomerOrderId { get; set; }
        public string Description { get; set; }
        public string DisconnectMode { get; set; }
        public DisconnectTelephoneNumberOrderTypeItem DisconnectTelephoneNumberOrderType { get; set; }

        public DisconnectTelephoneNumberOrder()
        {
            this.Name = string.Empty;
            //this.CustomerOrderId = string.Empty;
            this.DisconnectMode = "normal";
            this.Description = string.Empty;
            this.DisconnectTelephoneNumberOrderType = new DisconnectTelephoneNumberOrderTypeItem();
        }

        public DisconnectTelephoneNumberOrder(DedicatedVirtualNumber number)
        {
            string name = $"{number.NumberBandwidthFormat}-{number.AccountId}-{DateTime.Now:yyMMdd}";
            this.Name = name;
            this.DisconnectMode = "normal";
            //this.CustomerOrderId = name;
            this.Description = $"Disconnect {number.NumberBandwidthFormat}";
            this.DisconnectTelephoneNumberOrderType = new DisconnectTelephoneNumberOrderTypeItem()
            {
                //Protected = null,
                TelephoneNumberList = new TelephoneNumberList()
                {
                    TelephoneNumber = number.NumberBandwidthFormat
                }
            };
        }
    }

    [Serializable()]
    public class TelephoneNumberList
    {
        public string TelephoneNumber { get; set; }

        public TelephoneNumberList()
        {
            this.TelephoneNumber = string.Empty;
        }
    }

    [Serializable()]
    public class DisconnectTelephoneNumberOrderTypeItem
    {
        public TelephoneNumberList TelephoneNumberList { get; set; }
        //public string Protected { get; set; }

        public DisconnectTelephoneNumberOrderTypeItem()
        {
            this.TelephoneNumberList = new TelephoneNumberList();
            //this.Protected = null;
        }
    }

    // Response
    public class DisconnectTelephoneNumberOrderResponse
    {
        public List<Error> ErrorList { get; set; }
        public OrderResponseItem OrderRequest { get; set; }
    }
}