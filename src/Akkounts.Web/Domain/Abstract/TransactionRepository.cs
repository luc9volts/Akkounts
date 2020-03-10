using System.Collections.Generic;

namespace Akkounts.Web.Domain.Abstract
{
    public interface TransactionRepository
    {
        void Add(Transaction t);

        IEnumerable<Transaction> GetAllBy(string account);
    }
}