﻿using System.Collections.Generic;
using System.Linq;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using LiteDB;

namespace Akkounts.DataAccess
{
    public class TransactionRepositoryImpl : TransactionRepository
    {
        private const string DbName = "transactions.db";

        public void Add(Transaction t)
        {
            using var db = new LiteDatabase(DbName);
            var col = db.GetCollection<Transaction>(t.AccountNumber);
            col.Insert(t);
        }

        public IEnumerable<Transaction> GetAllBy(string account)
        {
            using var db = new LiteDatabase(DbName);
            var col = db.GetCollection<Transaction>(account);
            col.EnsureIndex(x => x.StartDate);
            return col
                .FindAll()
                .OrderBy(x => x.StartDate)
                .ToList();
        }
    }
}