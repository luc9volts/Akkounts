namespace Akkounts.Publisher.AccountTransactions
{
    public class Transaction
    {
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }

        public Transaction(decimal amount, TransactionType type)
        {
            Amount = amount;
            Type = type;
        }

        public enum TransactionType
        {
            Debit,
            Credit
        }
    }
}