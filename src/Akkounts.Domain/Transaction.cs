using System;

namespace Akkounts.Domain
{
    public record Transaction
    {
        public int Id { get; init; }
        public decimal Amount { get; init; }
        public string AccountNumber { get; init; }
        public DateTime StartDate { get; init; }
    }
}