using System.Collections.Generic;

namespace Akkounts.Domain.Abstract
{
    public interface TransactionRepository
    {
        void Add(Transaction t);
        Balance GetBalance(string account);
        IEnumerable<string> GetAccountList();
    }    
}