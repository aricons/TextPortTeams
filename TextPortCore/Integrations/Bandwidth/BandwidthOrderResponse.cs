using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TextPortCore.Integrations.Bandwidth
{
    //<? xml version="1.0" encoding="UTF-8" standalone="yes"?><OrderResponse><ErrorList><Error><Code>5087</Code><Description>The input order type is not a valid entry.</Description></Error></ErrorList>
    //    <Order><Name>VN 1 19495551212</Name><OrderCreateDate>2019-05-17T00:28:26.256Z</OrderCreateDate><BackOrderRequested>false<
    //    /BackOrderRequested><PartialAllowed>true</PartialAllowed><SiteId>23092</SiteId></Order></OrderResponse>

    [Serializable()]
    public class BandwidthOrderResponse
    {
        public int CompletedQuantity { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime OrderCompleteDate { get; set; }
        public string OrderStatus { get; set; }
        public CompletedNumbers CompletedNumbers { get; set; }
        public int FailedQuantity { get; set; }
        public List<Error> ErrorList { get; set; }
        public OrderResponseItem Order { get; set; }

        // Default constructor
        public BandwidthOrderResponse()
        {
            this.CompletedQuantity = 0;
            this.CreatedByUser = string.Empty;
            this.OrderStatus = string.Empty;
            this.LastModifiedDate = DateTime.MinValue;
            this.OrderCompleteDate = DateTime.MinValue;
            this.ErrorList = new List<Error>();
            this.Order = new OrderResponseItem();
            this.CompletedNumbers = new CompletedNumbers();
        }
    }

    [Serializable()]
    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }

        // Default constructor
        public Error()
        {
            this.Code = string.Empty;
            this.Description = string.Empty;
        }
    }

    [Serializable()]
    public class CompletedNumbers
    {
        public string TelephoneNumber { get; set; }
    }

    [Serializable()]
    public class OrderResponseItem
    {
        public string CustomerOrderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public string PeerId { get; set; }
        public bool BackOrderRequested { get; set; }
        public bool PartialAllowed { get; set; }
        public string SiteId { get; set; }
        public string id { get; set; }
        public AreaCodeSearchAndOrderType AreaCodeSearchAndOrderType { get; set; }
        public TnAttributes TnAttributes { get; set; }
        public DisconnectTelephoneNumberOrderTypeItem DisconnectTelephoneNumberOrderType { get; set; }

        // Default constructor
        public OrderResponseItem()
        {
            this.CustomerOrderId = string.Empty;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.OrderCreateDate = DateTime.MinValue;
            this.PeerId = string.Empty;
            this.BackOrderRequested = false;
            this.PartialAllowed = false;
            this.SiteId = string.Empty;
            this.id = string.Empty;
            this.AreaCodeSearchAndOrderType = null;
            this.TnAttributes = new TnAttributes();
        }
    }

    [Serializable()]
    public class AreaCodeSearchAndOrderType
    {
        public string AreaCode { get; set; }
        public string Quantity { get; set; }
    }

    [Serializable()]
    public class TnAttributes
    {
        public List<string> TnAttribute { get; set; }

        public TnAttributes()
        {
            this.TnAttribute = new List<string>();
        }
    }

}
