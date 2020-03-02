using System;
using System.Collections.Generic;

namespace Akkounts.Publisher.AccountTransactions
{
    public class TransactionsFactory
    {
        private static readonly Random RandomTypeGen = new Random();
        private static readonly Random RandomAmountGen = new Random();

        public IEnumerable<Transaction> GetTransactions()
        {
            while (true)
                yield return GetRandomTransaction();
        }

        private static Transaction GetRandomTransaction()
        {
            var randomType = (Transaction.TransactionType) RandomTypeGen.Next(2);
            var randomAmount = (decimal) RandomAmountGen.NextDouble();

            return new Transaction(randomAmount, randomType);
        }
    }
}