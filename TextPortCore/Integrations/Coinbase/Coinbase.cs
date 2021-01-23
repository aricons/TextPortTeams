using System;
using System.Configuration;

using RestSharp;
using RestSharp.Serialization.Json;

using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Integrations.Coinbase
{
    public class Coinbase : IDisposable
    {
        private string BaseUrl = ConfigurationManager.AppSettings["CoinbaseBaseUrl"];
        private string ApiKey = ConfigurationManager.AppSettings["CoinbaseAPIKey"];
        private string ApiVersion = ConfigurationManager.AppSettings["CoinbaseAPIVersion"];

        private readonly RestClient _client;

        public Coinbase()
        {
            this._client = new RestClient();
            _client.BaseUrl = new Uri(BaseUrl);
        }

        public ChargeResponse CreateCharge(RegistrationData regData)
        {
            charge charge = new charge(regData);
            ChargeResponse response = SubmitCharge(regData, charge);

            return response;
        }

        private ChargeResponse SubmitCharge(RegistrationData regData, charge charge)
        {
            try
            {
                RestRequest request = new RestRequest("/charges", Method.POST)
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new JsonSerializer()
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-CC-Api-Key", ApiKey);
                request.AddHeader("X-CC-Version", ApiVersion);

                request.AddJsonBody(charge);

                // Send the charge request to Coinbase
                ChargeResponse response = _client.Execute<ChargeResponse>(request).Data;

                // Process the response
                if (response != null)
                {
                    // Add transaction IDs for each item purchased. Set status to Open amd record the Coinbase UUID for tracking of subsequent transactions.
                    using (TextPortDA da = new TextPortDA())
                    {
                        if (regData.PurchaseType == "VirtualNumber" || regData.PurchaseType == "VirtualNumberSignUp" || regData.PurchaseType == "VirtualNumberRenew")
                        {
                            string reservationId = string.Empty;

                            // Reserve the virtual number.
                            using (Bandwidth.Bandwidth bw = new Bandwidth.Bandwidth())
                            {
                                string processingMessage = string.Empty;
                                reservationId = bw.ReserveVirtualNumber(regData.VirtualNumber, ref processingMessage);
                            }

                            // Add the purchase transactions
                            if (!string.IsNullOrEmpty(reservationId))
                            {
                                PurchaseTransaction transaction = new PurchaseTransaction()
                                {
                                    PaymentService = "Coinbase",
                                    Status = Helpers.TransactionStatus.Open,
                                    TransactionId = response.data.id, // UIID
                                    TransactionType = "Open",
                                    Description = regData.ProductDescription,
                                    TransactionDate = DateTime.Now,
                                    AccountId = regData.AccountId,
                                    GrossAmount = regData.NumberCost,
                                    Fee = 0,
                                    ItemPurchased = "VMN",
                                    ReservedNumber = regData.VirtualNumber,
                                    NumberReservationId = reservationId,
                                    ReceiverId = response.data.id
                                };
                                da.InsertPurchaseTransaction(transaction);

                                // Add a second transaction if credit was purchased.
                                if (regData.CreditPurchaseAmount > 0)
                                {
                                    PurchaseTransaction creditTransaction = new PurchaseTransaction()
                                    {
                                        PaymentService = "Coinbase",
                                        Status = Helpers.TransactionStatus.Open,
                                        TransactionId = response.data.id,
                                        TransactionType = "Open",
                                        Description = $"Add {regData.CreditPurchaseAmount:C2} credit",
                                        TransactionDate = DateTime.Now,
                                        AccountId = regData.AccountId,
                                        GrossAmount = regData.CreditPurchaseAmount,
                                        Fee = 0,
                                        ItemPurchased = "CREDIT",
                                        ReservedNumber = null,
                                        ReceiverId = response.data.id,
                                    };
                                    da.InsertPurchaseTransaction(creditTransaction);
                                }
                            }
                        }
                        else //Credit purchase only
                        {
                            PurchaseTransaction transaction = new PurchaseTransaction()
                            {
                                PaymentService = "Coinbase",
                                Status = Helpers.TransactionStatus.Open,
                                TransactionId = response.data.id,
                                TransactionType = "Open",
                                Description = $"Add {regData.TotalCost:C2} credit",
                                TransactionDate = DateTime.Now,
                                AccountId = regData.AccountId,
                                GrossAmount = regData.TotalCost,
                                Fee = 0,
                                ItemPurchased = "CREDIT",
                                ReservedNumber = null,
                                ReceiverId = response.data.id,
                            };
                            da.InsertPurchaseTransaction(transaction);
                        }
                    }
                }
                return response;

            }
            catch (Exception ex)
            {
                string foo = ex.Message;
                //message.ProcessingMessage += "Bandwidth gateway delivery failed. Exception: " + ex.Message + ". ";
                //message.QueueStatus = (byte)QueueStatuses.InternalFailure;
                EventLogging.WriteEventLogEntry("An error occurred in Coinbase.SubmitCharge. Message: " + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            return null;
        }

        #region "Disposal"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        #endregion
    }
}
