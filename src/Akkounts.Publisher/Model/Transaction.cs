namespace Akkounts.Publisher.Model
{
    public abstract class Transaction
    {
        public decimal Amount { get; protected set; }
        public TransactionType Type { get; protected set; }
        public int AccountNumber { get; protected set; }

        public enum TransactionType
        {
            Debit,
            Credit
        }
    }
}