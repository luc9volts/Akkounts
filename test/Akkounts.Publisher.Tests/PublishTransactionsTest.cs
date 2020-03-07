using System.Linq;
using Akkounts.Publisher.Model;
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

        [Fact]
        public void AccountNumberUnder100()
        {
            var transactionsFactory = new TransactionsFactory();
            var transactions = transactionsFactory.GetTransactions().Take(50).ToList();
            Assert.True(transactions.All(t => t.AccountNumber > 0 && t.AccountNumber <= 100));
        }

        [Theory]
        [InlineData(Transaction.TransactionType.Credit)]
        [InlineData(Transaction.TransactionType.Debit)]
        public void TransactionsOfEachType(Transaction.TransactionType type)
        {
            var transactionsFactory = new TransactionsFactory();
            var transactions = transactionsFactory.GetTransactions().Take(50).ToList();
            Assert.Contains(transactions, t => t.Type.Equals(type));
        }
    }
}