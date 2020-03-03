using System.Collections.Generic;

namespace Akkounts.Publisher.AccountTransactions
{
    public class TransactionsFactory
    {
        public IEnumerable<RandomTransaction> GetTransactions()
        {
            while (true)
                yield return new RandomTransaction();
        }
    }
}