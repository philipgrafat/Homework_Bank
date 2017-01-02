using System;
using System.Collections.Generic;

namespace Bank
{
    public static class BankStorage
    {
        private static Dictionary<IBAN, BankAccount> accountStorage;
        public static Dictionary<Guid, Transaction> transactionStorage { get; }

        static BankStorage()
        {
            accountStorage = new Dictionary<IBAN, BankAccount>();
            transactionStorage = new Dictionary<Guid, Transaction>(); // Dictionary with GUID to avoid duplicates at money transfer
        }

        public static void AddAccount(BankAccount account) => accountStorage[account.Iban] = account;
        
        public static BankAccount GetAccount(IBAN withIban) => accountStorage[withIban];

        public static void DeleteAccount(IBAN withIban) => accountStorage.Remove(withIban);

        public static bool DoesAccountExist(IBAN withIban) => accountStorage.ContainsKey(withIban);

        public static void AddTransaction(Transaction transaction) => transactionStorage[transaction.ID] = transaction;
    }
}