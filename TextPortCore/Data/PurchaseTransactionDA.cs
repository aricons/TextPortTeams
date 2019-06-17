using System;
using System.Linq;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"
        #endregion

        #region "Update Methods"
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