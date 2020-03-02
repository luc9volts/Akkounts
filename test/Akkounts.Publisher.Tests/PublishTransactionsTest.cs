using System.Linq;
using Akkounts.Publisher.AccountTransactions;
using Xunit;

namespace Akkounts.Publisher.Tests
{
    public class PublishTransactions
    {
        [Fact]
        public void TransactionsExists()
        {
            var transactionsFactory = new TransactionsFactory();
            Assert.True(transactionsFactory.GetTransactions().Any());
        }

        [Theory]
        [InlineData(Transaction.TransactionType.Credit)]
        [InlineData(Transaction.TransactionType.Debit)]
        public void TransactionsOfEachType(Transaction.TransactionType type)
        {
            var transactionsFactory = new TransactionsFactory();
            var transactions = transactionsFactory.GetTransactions().Take(50).ToList();
            Assert.Contains(transactions, t =>t.Type.Equals(type));
        }
    }
}