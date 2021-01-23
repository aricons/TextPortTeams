using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using System.Collections.Generic;

using Newtonsoft.Json;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;
using TextPortCore.Integrations.Coinbase;

namespace TextPortTeams.Controllers
{
    public class CoinbaseController : ApiController
    {
        [HttpGet]
        [ActionName("ping")]
        public string Ping()
        {
            return String.Format("Coinbase endpoint alive at {0}", DateTime.Now);
        }

        [HttpGet]
        [ActionName("pingval")]
        public string PingVal(string value)
        {
            return String.Format("Coinbase PingVal received {0}", value);
        }

        [HttpPost]
        public IHttpActionResult ChargeEvent([FromBody] ChargeEvent chargeEvent)
        {
            if (chargeEvent != null)
            {
                LogRequest(chargeEvent);

                if (chargeEvent.@event != null && chargeEvent?.@event?.data != null)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        switch (chargeEvent.@event.type)
                        {
                            case "charge:created":
                                if (!string.IsNullOrEmpty(chargeEvent.@event.data.id))
                                {
                                    da.UpdateTransactionStatus(chargeEvent.@event.data.id, TransactionStatus.Created);
                                    return Ok();
                                }
                                break;

                            case "charge:pending":
                                if (!string.IsNullOrEmpty(chargeEvent.@event.data.id))
                                {
                                    da.UpdateTransactionStatus(chargeEvent.@event.data.id, TransactionStatus.Pending);
                                    return Ok();
                                }
                                break;

                            case "charge:failed":
                                if (!string.IsNullOrEmpty(chargeEvent.@event.data.id))
                                {
                                    da.UpdateTransactionStatus(chargeEvent.@event.data.id, TransactionStatus.Failed);
                                    return Ok();
                                }
                                break;

                            case "charge:confirmed":
                                if (!string.IsNullOrEmpty(chargeEvent.@event.data.id) && chargeEvent.@event.data.metadata != null)
                                {
                                    Metadata metadata = chargeEvent.@event.data.metadata;

                                    da.UpdateTransactionStatus(chargeEvent.@event.data.id, TransactionStatus.Confirmed);

                                    List<PurchaseTransaction> purchaseTransactions = da.GetPurchaseTransactionsByTransactionId(chargeEvent.@event.data.id);
                                    if (purchaseTransactions != null)
                                    {
                                        int accountId = purchaseTransactions.FirstOrDefault().AccountId;
                                        da.EnableTemporaryAccount(accountId);

                                        foreach (PurchaseTransaction pt in purchaseTransactions)
                                        {
                                            if (pt.ItemPurchased == "VMN")
                                            {
                                                if (!string.IsNullOrEmpty(pt.NumberReservationId) && !string.IsNullOrEmpty(pt.ReservedNumber))
                                                {
                                                    using (Bandwidth bw = new Bandwidth())
                                                    {
                                                        RegistrationData regData = new RegistrationData()
                                                        {
                                                            AccountId = pt.AccountId,
                                                            VirtualNumber = pt.ReservedNumber,
                                                            CarrierId = (int)Carriers.BandWidth,
                                                            NumberType = NumberTypes.Regular,
                                                            NumberCost = metadata.number_cost,
                                                            CountryId = metadata.country_id,
                                                            LeasePeriod = (short)metadata.lease_period,
                                                            LeasePeriodType = metadata.lease_period_type,
                                                            CreditPurchaseAmount = metadata.credit_amount,
                                                            NumberReservationId = pt.NumberReservationId
                                                        };

                                                        if (bw.PurchaseVirtualNumber(regData))
                                                        {
                                                            if (da.AddNumberToAccount(regData))
                                                            {
                                                                regData.CompletionTitle = "Registration Complete";
                                                                regData.CompletionMessage = "Your account and number were successfully registered.";
                                                            }
                                                            else
                                                            {
                                                                regData.CompletionMessage += " The number was unable to be assigned to your account.";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (pt.ItemPurchased == "CREDIT")
                                            {
                                                RegistrationData regData = new RegistrationData()
                                                {
                                                    AccountId = pt.AccountId,
                                                    CreditPurchaseAmount = pt.GrossAmount
                                                };

                                                if (regData.AccountId > 0)
                                                {
                                                    Account acc = da.GetAccountById(regData.AccountId);
                                                    if (acc != null)
                                                    {
                                                        acc.Balance += Convert.ToDecimal(regData.CreditPurchaseAmount);
                                                        da.SaveChanges();
                                                    }

                                                    regData.CompletionTitle = "Credit Purchase Complete";
                                                    regData.CompletionMessage = $"{regData.CreditPurchaseAmount:C2} credit was sucessfully added to your account";
                                                }
                                            }
                                        }
                                    }
                                    return Ok();
                                }
                                break;
                        }
                    }
                }
            }

            return NotFound();
        }

        private void LogRequest(ChargeEvent chargeEvent)
        {
            string json = JsonConvert.SerializeObject(chargeEvent);
            string eventCode = "00000000";

            if (chargeEvent != null)
            {
                eventCode = chargeEvent?.@event?.data?.code;
            }

            StreamWriter logFile;

            string baseFolder = ConfigurationManager.AppSettings["CoinbaseEventLogs"];
            string fileName = $"{baseFolder}ChargeEvent_{DateTime.Now:yyyy-MM-ddThh-mm-ss}_{eventCode}.txt";

            try
            {
                using (logFile = new StreamWriter(fileName, true))
                {
                    logFile.WriteLine(json);
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
