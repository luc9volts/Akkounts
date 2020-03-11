using System.Linq;
using Akkounts.Publisher.Model;
using Xunit;

namespace Akkounts.Publisher.Tests
{
    public class DataAccessTest
    {
        [Fact]
        public void TransactionsExists()
        {
            var transactionsFactory = new TransactionsFactory();
            Assert.True(transactionsFactory.GetTransactions().Any());
        }        
    }
}