using System;

namespace Akkounts.Domain
{
    public class Balance
    {
        public decimal Amount { get; }

        public Balance(decimal balanceAmount)
        {
            Amount = balanceAmount;
        }

        public bool IsTransactionAllowed(decimal debitAmount)
        {
            return Amount >= Math.Abs(debitAmount);
        }
    }
}