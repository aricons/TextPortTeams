using System.Collections.Generic;
using System.Linq;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class PurchasesContainer
    {
        public Account Account { get; set; }
        public List<PurchaseTransaction> PurchaseTransactions { get; set; }

        public PurchasesContainer()
        {
            this.Account = new Account();
            this.PurchaseTransactions = new List<PurchaseTransaction>();
        }

        public PurchasesContainer(int accountId)
        {
            using (TextPortContext _context = new TextPortContext())
            {
                this.Account = _context.Accounts.FirstOrDefault(x => x.AccountId == accountId);
                this.PurchaseTransactions = _context.PurchaseTransactions.Where(x => x.AccountId == accountId).OrderByDescending(x => x.TransactionDate).ToList();
            }
        }
    }
}
