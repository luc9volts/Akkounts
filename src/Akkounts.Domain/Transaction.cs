using System;

namespace Akkounts.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public DateTime StartDate { get; set; }
    }
}