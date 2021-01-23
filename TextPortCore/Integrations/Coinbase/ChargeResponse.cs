using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Integrations.Coinbase
{
    public class ChargeResponse
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string resource { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string logo_url { get; set; }
        public string hosted_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime expires_at { get; set; }
        public List<Timeline> timeline { get; set; }
        public Metadata metadata { get; set; }
        public string pricing_type { get; set; }
        public Pricing pricing { get; set; }
        public List<object> payments { get; set; }
        public PaymentThreshold payment_threshold { get; set; }
        public Addresses addresses { get; set; }
        public string redirect_url { get; set; }
        public string cancel_url { get; set; }
    }

    public class Timeline
    {
        public DateTime time { get; set; }
        public string status { get; set; }
    }

    public class Local
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class Bitcoin
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class Ethereum
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class Pricing
    {
        public Local local { get; set; }
        public Bitcoin bitcoin { get; set; }
        public Ethereum ethereum { get; set; }
    }

    public class OverpaymentAbsoluteThreshold
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class UnderpaymentAbsoluteThreshold
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class PaymentThreshold
    {
        public OverpaymentAbsoluteThreshold overpayment_absolute_threshold { get; set; }
        public string overpayment_relative_threshold { get; set; }
        public UnderpaymentAbsoluteThreshold underpayment_absolute_threshold { get; set; }
        public string underpayment_relative_threshold { get; set; }
    }

    public class Addresses
    {
        public string bitcoin { get; set; }
        public string ethereum { get; set; }
    }
}
