using System;

namespace Akkounts.Publisher.Model
{
    public abstract class Transaction
    {
        public decimal Amount { get; protected set; }
        public TransactionType Type { get; protected set; }
        public string AccountNumber { get; protected set; }
        public DateTime StartDate { get; set; }

        public Transaction()
        {
            StartDate = DateTime.Now;
        }

        public enum TransactionType
        {
            Debit,
            Credit
        }
    }
}