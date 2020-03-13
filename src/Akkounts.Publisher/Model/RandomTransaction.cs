using System;

namespace Akkounts.Publisher.Model
{
    public class RandomTransaction : Transaction
    {
        private readonly Random _randomGen = new Random();

        public RandomTransaction()
        {
            Amount = (decimal) _randomGen.Next(1, 99999) / 100;
            Type = (TransactionType) _randomGen.Next(2);
            AccountNumber = $"ACC{_randomGen.Next(1, 100):000}"; //Only a few accounts
        }
    }
}