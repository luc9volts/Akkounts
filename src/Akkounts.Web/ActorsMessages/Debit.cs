using System;

namespace Akkounts.Web.ActorsMessages
{
    public class Debit : TransactionMessage
    {
        public override decimal Amount => base.Amount * -1;

        public Debit(string account, decimal amount, DateTime startDate) : base(account, amount, startDate)
        {
        }
    }
}