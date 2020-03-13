using System;

namespace Akkounts.Web.ActorsMessages
{
    public class Credit : TransactionMessage
    {
        public Credit(string account, decimal amount, DateTime startDate) : base(account, amount, startDate)
        {
        }
    }
}