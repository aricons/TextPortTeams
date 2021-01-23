using System;
using System.Collections.Generic;
using System.Linq;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public List<PurchaseTransaction> GetPurchaseTransactionsByTransactionId(string transactionId)
        {
            return _context.PurchaseTransactions.Where(x => x.TransactionId == transactionId).ToList();
        }

        #endregion

        #region "Update Methods"

        public bool UpdateTransactionStatus(string transactionId, TransactionStatus newStatus)
        {
            try
            {
                List<PurchaseTransaction> transactions = _context.PurchaseTransactions.Where(x => x.TransactionId == transactionId).ToList();
                if (transactions != null)
                {
                    foreach (PurchaseTransaction transaction in transactions)
                    {
                        transaction.Status = newStatus;

                        switch (newStatus)
                        {
                            case TransactionStatus.Confirmed:
                                transaction.TransactionType = "Completed";
                                break;
                            case TransactionStatus.Created:
                                transaction.TransactionType = "Created";
                                break;
                            default:
                                transaction.TransactionType = "Open";
                                break;
                        }
                    }

                    SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.UpdateTransactionStatus", ex);
            }

            return false;
        }

        #endregion

        #region "Insert Methods"

        public int InsertPurchaseTransaction(PurchaseTransaction purchaseTrans)
        {
            try
            {
                _context.PurchaseTransactions.Add(purchaseTrans);
                _context.SaveChanges();

                return purchaseTrans.PurchaseId;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("PurchaseTransactionDA.InsertPurchaseTransaction", ex);
            }
            return 0;
        }

        #endregion
    }
}