using System;
using System.Collections.Generic;
using System.Linq;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using LiteDB;

namespace Akkounts.DataAccess
{
    public sealed class TransactionRepositoryImpl : TransactionRepository, IDisposable
    {
        private const string DbName = "transactions.db";
        private readonly LiteDatabase _db;

        public TransactionRepositoryImpl()
        {
            //_db = new LiteDatabase(DbName);
            _db = new LiteDatabase(new ConnectionString
            {
                Filename = DbName,
                Connection = ConnectionType.Direct
            });
        }

        public void Add(Transaction t)
        {
            var col = _db.GetCollection<Transaction>(t.AccountNumber);
            col.Insert(t);
        }

        public Balance GetBalance(string account)
        {
            var value = GetAllBy(account).Sum(txn => txn.Amount);
            return new Balance(value);
        }

        public IEnumerable<string> GetAccountList()
        {
            return _db.GetCollectionNames().ToList();
        }

        private IEnumerable<Transaction> GetAllBy(string account)
        {
            var col = _db.GetCollection<Transaction>(account);
            return col
                .FindAll()
                .OrderBy(x => x.Id);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}