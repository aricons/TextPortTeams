using System;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextPortCore.Data;
using TextPortCore.Models;

namespace TextPortCore.Integrations.PayPal
{
    public class IPN
    {
        public bool ParsePayPalIPN(System.Collections.Specialized.NameValueCollection qs)
        {
            string resultMessage = String.Empty;
            bool returnValue = false;
            string[] customFieldValues;
            string virtualMobileNumber = String.Empty;
            string alphaCountryCode = String.Empty;
            string creditCountCode = String.Empty;

            try
            {
                InstantPaymentNotification ipn = new InstantPaymentNotification()
                {
                    ReceiverEmail = parseStringField(qs, "receiver_email"),
                    ReceivedID = parseStringField(qs, "receiver_id"),
                    ResidenceCountry = parseStringField(qs, "residence_country"),
                    TestIPN = parseStringField(qs, "test_ipn"),
                    TransactionSubject = parseStringField(qs, "transaction_subject"),
                    TransactionID = parseStringField(qs, "txn_id"),
                    TransactionType = parseStringField(qs, "txn_type"),
                    PayerEmail = parseStringField(qs, "payer_email"),
                    PayerID = parseStringField(qs, "payer_id"),
                    PayerStatus = parseStringField(qs, "payer_status"),
                    FirstName = parseStringField(qs, "first_name"),
                    LastName = parseStringField(qs, "last_name"),
                    AddressCity = parseStringField(qs, "address_city"),
                    AddressCountry = parseStringField(qs, "address_country"),
                    AddressCountryCode = parseStringField(qs, "address_country_code"),
                    AddressName = parseStringField(qs, "address_name"),
                    AddressState = parseStringField(qs, "address_state"),
                    AddressStatus = parseStringField(qs, "address_status"),
                    AddressStreet = parseStringField(qs, "address_street"),
                    AddressZip = parseStringField(qs, "address_zip"),
                    CustomField = parseStringField(qs, "custom"),
                    ItemName = parseStringField(qs, "item_name"),
                    ItemNumber = parseStringField(qs, "item_number"),
                    Currency = parseStringField(qs, "mc_currency"),
                    PaymentStatus = parseStringField(qs, "payment_status"),
                    PaymentType = parseStringField(qs, "payment_type"),
                    Fee = parseDecimalField(qs, "mc_fee"),
                    HandlingAmount = parseDecimalField(qs, "handling_amount"),
                    GrossAmount = parseDecimalField(qs, "mc_gross"),
                    PaymentDate = parseStringField(qs, "payment_date"),
                    PaymentFee = parseDecimalField(qs, "payment_fee"),
                    PaymentGross = parseDecimalField(qs, "payment_gross"),
                    Quantity = parseDecimalField(qs, "quantity"),
                    Shipping = parseDecimalField(qs, "shipping "),
                    Tax = parseDecimalField(qs, "tax")
                };

                string result = "PayPal Instant Payment Notification received on " + DateTime.Now.ToString() + "\r\n";
                result += "Receiver Email: " + ipn.ReceiverEmail + "\r\n";
                result += "Receiver ID: " + ipn.ReceivedID + "\r\n";
                result += "Residence Country: " + ipn.ResidenceCountry + "\r\n";
                result += "Is Test IPN: " + ipn.TestIPN + "\r\n";
                result += "Transaction Subject: " + ipn.TransactionSubject + "\r\n";
                result += "Transaction ID: " + ipn.TransactionID + "\r\n";
                result += "Transaction Type: " + ipn.TransactionType + "\r\n";
                result += "Payer Email: " + ipn.PayerEmail + "\r\n";
                result += "Payer ID: " + ipn.PayerID + "\r\n";
                result += "Payer Status: " + ipn.PayerStatus + "\r\n";
                result += "First Name: " + ipn.FirstName + "\r\n";
                result += "Last Name: " + ipn.LastName + "\r\n";
                result += "Address City: " + ipn.AddressCity + "\r\n";
                result += "Address Country: " + ipn.AddressCountry + "\r\n";
                result += "Address Country Code: " + ipn.AddressCountryCode + "\r\n";
                result += "Address Name: " + ipn.AddressName + "\r\n";
                result += "Address State: " + ipn.AddressState + "\r\n";
                result += "Address Status: " + ipn.AddressStatus + "\r\n";
                result += "Address Street: " + ipn.AddressStreet + "\r\n";
                result += "Address Zip: " + ipn.AddressZip + "\r\n";
                result += "Custom Field: " + ipn.CustomField + "\r\n";
                result += "Handling Amount: " + ipn.HandlingAmount.ToString() + "\r\n";
                result += "Item Name: " + ipn.ItemName + "\r\n";
                result += "Item Number: " + ipn.ItemNumber + "\r\n";
                result += "Currency: " + ipn.Currency + "\r\n";
                result += "Fee: " + ipn.Fee.ToString() + "\r\n";
                result += "Gross Amount: " + ipn.GrossAmount + "\r\n";
                result += "Payment Date: " + ipn.PaymentDate + "\r\n";
                result += "Payment Fee: " + ipn.PaymentFee.ToString() + "\r\n";
                result += "Payment Gross: " + ipn.PaymentGross.ToString() + "\r\n";
                result += "Payment Status: " + ipn.PaymentStatus + "\r\n";
                result += "Payment Type: " + ipn.PaymentType + "\r\n";
                result += "Quantity: " + ipn.Quantity.ToString() + "\r\n";
                result += "Shipping: " + ipn.Shipping + "\r\n";
                result += "Tax: " + ipn.Tax + "\r\n";
                result += "Original String: " + qs.ToString() + "\r\n";

                writeXMLToDisk(result, "PayPalIPN");

                if (!String.IsNullOrEmpty(ipn.CustomField))
                {
                    customFieldValues = ipn.CustomField.Split('|');
                    if (customFieldValues.Length >= 2)
                    {
                        if (int.Parse(customFieldValues[1]) > 0)
                        {
                            string purchaseType = customFieldValues[0];
                            int accountId = int.Parse(customFieldValues[1]);
                            if (customFieldValues.Length >= 3)
                            {
                                PurchaseTransaction purchaseTrans = new PurchaseTransaction()
                                {
                                    PaymentService = "PayPal",
                                    TransactionId = ipn.TransactionID,
                                    TransactionDate = DateTime.UtcNow,
                                    TransactionType = ipn.PaymentStatus,
                                    ItemPurchased = purchaseType,
                                    AccountId = accountId,
                                    ReceiverId = ipn.ReceivedID,
                                    GrossAmount = ipn.GrossAmount,
                                    Fee = ipn.Fee
                                };

                                using (TextPortDA da = new TextPortDA())
                                {
                                    da.InsertPurchaseTransaction(purchaseTrans);

                                    // If the payment type is eCheck, reverse any credit purchases.
                                    // Can be applied manually later once the cCheck clears.
                                    if (accountId > 0)
                                    {
                                        if (ipn.PaymentType.ToLower() == "echeck")
                                        {
                                            Account acc = da.GetAccountById(accountId);
                                            acc.Balance -= ipn.GrossAmount;
                                            da.SaveChanges();
                                        }
                                    }
                                }

                                // All processing below now handled by the real-time response fromthe PayPal button.
                                //        switch (purchaseType)
                                //        {
                                //            case "CREDITS":
                                //                creditPricingId = (!String.IsNullOrEmpty(customFieldValues[2])) ? int.Parse(customFieldValues[2]) : 1;
                                //                break;
                                //            case "VMN_EXTEND":
                                //                virtualNumberId = (!String.IsNullOrEmpty(customFieldValues[2])) ? int.Parse(customFieldValues[2]) : 0;
                                //                break;
                                //            default:
                                //                virtualMobileNumber = customFieldValues[2];
                                //                break;
                                //        }
                                //    }
                                //    if (customFieldValues.Length >= 4)
                                //    {
                                //        switch (purchaseType)
                                //        {
                                //            case "VMN_EXTEND":
                                //                leasePeriod = (!String.IsNullOrEmpty(customFieldValues[3])) ? int.Parse(customFieldValues[3]) : 0;
                                //                break;
                                //            default:
                                //                virtualNumberCountryId = parseIntValue(customFieldValues[3], 0);
                                //                //SelectVirtualNumberCountryByIdResult vnc = DataAccess.GetVirtualNumberCountryById(virtualNumberCountryId);
                                //                //if (vnc != null)
                                //                //{
                                //                //    alphaCountryCode = vnc.CountryAlphaCode;
                                //                //}
                                //                break;
                                //        }
                                //    }
                                //    if (customFieldValues.Length >= 5)
                                //    {
                                //        leasePeriod = parseIntValue(customFieldValues[4], 1);
                                //    }
                                //    if (customFieldValues.Length >= 6)
                                //    {
                                //        creditCountCode = customFieldValues[5];
                                //    }

                                //    //DataAccess.AddPurchaseTransactionToLog("PayPal", ipn.TransactionID, ipn.PaymentStatus, purchaseType, accountID, ipn.ReceivedID, ipn.GrossAmount, ipn.Fee);

                                //    switch (purchaseType)
                                //    {
                                //        case "CREDITS":
                                //            if (ipn.PaymentStatus == "Completed")
                                //            {
                                //                //DataAccess.UpdateAccountCreditPurchase(accountID, creditPricingId, "ADD");
                                //                returnValue = true;
                                //            }
                                //            else if (ipn.PaymentStatus == "Reversed")
                                //            {
                                //                //DataAccess.UpdateAccountCreditPurchase(accountID, creditPricingId, "DEDUCT");
                                //                returnValue = true;
                                //            }
                                //            break;

                                //        case "VMN":
                                //            // Only purchase the Nexmo number if the status is "Completed".
                                //            if (ipn.PaymentStatus == "Completed")
                                //            {
                                //                switch (alphaCountryCode)
                                //                {
                                //                    case "US": // Bandwidth.com for US numbers
                                //                               //if (BandwidthCom.PurchaseVirtualNumber(virtualMobileNumber, "Test"))
                                //                               //{
                                //                               //    DataAccess.AddDedicatedVirtualNumber(accountID, virtualNumberCountryId, virtualMobileNumber, leasePeriod, ipn.GrossAmount, creditCountCode);
                                //                               //    returnValue = true;
                                //                               //}
                                //                               //break;
                                //                        break;

                                //                    default: // Nexmo
                                //                             //if (Nexmo.PurchaseVirtualNumber(alphaCountryCode, virtualMobileNumber))
                                //                             //{
                                //                             //    DataAccess.AddDedicatedVirtualNumber(accountID, virtualNumberCountryId, virtualMobileNumber, leasePeriod, ipn.GrossAmount, creditCountCode);
                                //                             //    returnValue = true;
                                //                             //}
                                //                        break;
                                //                }
                                //            }

                                //            // If a reversal was received, cancel any virtual number on the account.
                                //            else if (ipn.PaymentStatus == "Reversed")
                                //            {
                                //                //returnValue = DataAccess.ForceVirtualNumberCancellation(virtualMobileNumber);
                                //                returnValue = true;
                                //            }
                                //            else
                                //            {
                                //                return false;
                                //            }
                                //            break;

                                //        case "VMN_EXTEND":
                                //            if (virtualNumberId > 0)
                                //            {
                                //                //returnValue = DataAccess.ExtendVirtualNumberLease(virtualNumberId, accountID, leasePeriod, ipn.GrossAmount);
                                //                returnValue = true;
                                //            }
                                //            else
                                //            {
                                //                returnValue = false;
                                //            }
                                //            break;
                            }
                        }
                    }
                }
                return returnValue;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string parseStringField(System.Collections.Specialized.NameValueCollection queryStringParameters, string fieldName)
        {
            return (!String.IsNullOrEmpty(queryStringParameters[fieldName])) ? queryStringParameters[fieldName].ToString().Trim() : String.Empty;
        }

        private decimal parseDecimalField(System.Collections.Specialized.NameValueCollection queryStringParameters, string fieldName)
        {
            try
            {
                if (!String.IsNullOrEmpty(queryStringParameters[fieldName]))
                {
                    return decimal.Parse(queryStringParameters[fieldName].ToString().Trim());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int parseIntValue(string inputValue, int defaultValue)
        {
            int intValueOut = defaultValue;

            try
            {
                if (!String.IsNullOrEmpty(inputValue))
                {
                    if (int.TryParse(inputValue, out intValueOut))
                    {
                        return intValueOut;
                    }
                }
            }
            catch (Exception)
            {
            }
            return intValueOut;
        }

        private static void writeXMLToDisk(string logData, string filePrefix)
        {
            StreamWriter logFile;

            string baseFolder = ConfigurationManager.AppSettings["PayPalIPNFiles"];
            string fileName = $"{baseFolder}{filePrefix}_{DateTime.Now:yyyy-MM-ddThh-mm-ss}.txt";

            try
            {
                using (logFile = new StreamWriter(fileName, true))
                {
                    logFile.WriteLine(logData);
                    logFile.Flush();
                    logFile.Close();
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }
        }
    }
}
