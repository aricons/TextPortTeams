using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.API
{
    public class BalanceResult
    {
        [Required]
        /// <summary>The account username</summary>
        public string UserName { get; set; }

        [Required]
        /// <summary>The current balance</summary>
        public decimal Balance { get; set; }


        // Constructors
        public BalanceResult()
        {
            this.UserName = string.Empty;
            this.Balance = 0;
        }

        public BalanceResult(Account account)
        {
            this.UserName = account.UserName;
            this.Balance = account.Balance;
        }
    }
}
