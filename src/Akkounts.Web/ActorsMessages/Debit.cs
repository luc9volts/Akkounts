using System;

namespace Akkounts.Web.ActorsMessages
{
    public class Debit : TransactionMessage
    {
        public Debit(string account, decimal amount, DateTime startDate) : base(account, amount, startDate)
        {
        }
    }
}