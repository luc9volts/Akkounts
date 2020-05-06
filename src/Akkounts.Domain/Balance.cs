using System;

namespace Akkounts.Domain
{
    public class Balance
    {
        public decimal Amount { get; private set; }

        public Balance(decimal balanceAmount)
        {
            Amount = balanceAmount;
        }

        public bool IsTransactionAllowed(decimal debitAmount) => Amount >= Math.Abs(debitAmount);

        public void Update(decimal txnValue) => Amount += txnValue;
    }
}