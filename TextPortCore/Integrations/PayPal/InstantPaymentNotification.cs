using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Integrations.PayPal
{
    public class InstantPaymentNotification
    {
        public string ReceiverEmail { get; set; }
        public string ReceivedID { get; set; }
        public string ResidenceCountry { get; set; }
        public string TestIPN { get; set; }
        public string TransactionSubject { get; set; }
        public string TransactionID { get; set; }
        public string TransactionType { get; set; }
        public string PayerEmail { get; set; }
        public string PayerID { get; set; }
        public string PayerStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressCountryCode { get; set; }
        public string AddressName { get; set; }
        public string AddressState { get; set; }
        public string AddressStatus { get; set; }
        public string AddressStreet { get; set; }
        public string AddressZip { get; set; }
        public string CustomField { get; set; }
        public decimal HandlingAmount { get; set; }
        public string ItemName { get; set; }
        public string ItemNumber { get; set; }
        public string Currency { get; set; }
        public decimal Fee { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PaymentFee { get; set; }
        public decimal PaymentGross { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
    }
}
