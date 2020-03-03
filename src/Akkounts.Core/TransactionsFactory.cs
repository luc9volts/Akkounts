using System.Collections.Generic;

namespace Akkounts.Core
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