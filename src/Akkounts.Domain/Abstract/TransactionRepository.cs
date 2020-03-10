using System.Collections.Generic;

namespace Akkounts.Domain.Abstract
{
    public interface TransactionRepository
    {
        void Add(Transaction t);

        IEnumerable<Transaction> GetAllBy(string account);
    }
    
}