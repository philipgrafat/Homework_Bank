using System;

namespace Bank
{
    public class BankClerk
    {
        public BankAccount SessionAccount { get; private set; }

        #region Account

        public IBAN CreateBankAccount(Person accountHolder, string accountName, string password, Money? startingCredit)
        {
            var bankAccount = new BankAccount(accountHolder, accountName, password);

            if (DateTime.Now - accountHolder.BirthDate < new TimeSpan(days: 16*365, hours: 0, minutes: 0, seconds: 0))
            {
                // Student Bonus of 50€ if AccountHolder younger than 16
                var transaction = new CashTransaction(new Money(50, CurrencyCode.EUR), bankAccount.Iban, "Youth Bonus");
                bankAccount.Transfer(transaction);
            }

            if (startingCredit.HasValue)
            {
                var transaction = new CashTransaction(startingCredit.Value, bankAccount.Iban, "Starting credit");
                bankAccount.Transfer(transaction);
            }

            SessionAccount = bankAccount;

            BankStorage.AddAccount(bankAccount);

            return bankAccount.Iban;
        }

        public BankTransactionState DeleteBankAccount(string password) => DeleteBankAccount(password, false);

        public BankTransactionState DeleteBankAccount(string password, bool force)
        {
            if (!SessionAccount.IsPasswordValid(password))
                return BankTransactionState.InvalidPassword;

            if (SessionAccount == null)
                return BankTransactionState.NotLoggedIn;

            if (SessionAccount.Credit.Amount > 0 && !force)
                return BankTransactionState.MoneyRemaining;

            if (SessionAccount.Credit.Amount < 0)
                return BankTransactionState.DebtsNotCleared;
            
            

            BankStorage.DeleteAccount(SessionAccount.Iban);

            SessionAccount = null;

            return BankTransactionState.Success;
        }

        #endregion

        #region Money Operations

        public BankTransactionState TransferMoney(Money amount, IBAN to, string referenceText)
        {
            if (SessionAccount == null)
                return BankTransactionState.NotLoggedIn;

            var transaction = new Transaction(amount, SessionAccount.Iban, to, referenceText);

            var receiver = BankStorage.GetAccount(to);

            SessionAccount.Transfer(transaction);
            receiver.Transfer(transaction);

            BankStorage.AddTransaction(transaction);
            BankStorage.AddAccount(receiver);
            
            SaveSession();

            return BankTransactionState.Success;
        }

        public BankTransactionState PutMoney(Money amount)
        {
            if (SessionAccount == null)
                return BankTransactionState.NotLoggedIn;

            var transaction = new CashTransaction(amount, SessionAccount.Iban, "bar deposit");

            var receiver = BankStorage.GetAccount(SessionAccount.Iban);
            receiver.Transfer(transaction);
            
            BankStorage.AddAccount(receiver);

            SaveSession();

            return BankTransactionState.Success;
        }

        #endregion

        #region Password

        public bool Login(IBAN iban, string password)
        {
            var account = BankStorage.GetAccount(iban);

            if (account.IsPasswordValid(password))
            {
                SessionAccount = account;
                return true;
            }

            return false;
        }

        public bool ChangePassword(string oldPassword, string newPassword)
            => SessionAccount.ChangePassword(oldPassword, newPassword);

        #endregion

        #region BankStorage

        private void SaveSession() => BankStorage.AddAccount(SessionAccount);

        #endregion
    }
}

public enum BankTransactionState
{
    Success, DebtsNotCleared, MoneyRemaining, NotLoggedIn, InvalidPassword
}