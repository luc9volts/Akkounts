using System;

namespace Akkounts.Publisher.Model
{
    public class RandomTransaction : Transaction
    {
        private readonly Random _randomGen = new Random();

        public RandomTransaction()
        {
            Amount = (decimal) _randomGen.Next(-99999, 99999) / 100;
            AccountNumber = $"ACC{_randomGen.Next(1, 50):000}"; //Only a few accounts
        }
    }
}