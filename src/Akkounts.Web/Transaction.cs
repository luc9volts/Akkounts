using System;

namespace Akkounts.Web
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public int AccountNumber { get; set; }
        
        public enum TransactionType
        {
            Debit,
            Credit
        }
    }
}
