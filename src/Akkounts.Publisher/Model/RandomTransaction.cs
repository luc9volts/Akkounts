using System;

namespace Akkounts.Publisher.Model
{
    public class RandomTransaction : Transaction
    {
        private readonly Random _randomGen = new Random();

        public RandomTransaction()
        {
            Amount = (decimal)_randomGen.NextDouble();
            Type = (TransactionType)_randomGen.Next(2);
            AccountNumber = _randomGen.Next(1, 100);//Only few account numbers cos this poc need some repetition from time to time
        }
    }
}