using System;

namespace Bank
{
    public class BankAccount
    {
        public Person AccountHolder { get; private set; }
        public string AccountName { get; private set; }
        public Money Credit { get; private set; }
        public IBAN Iban;
        private string _password;

        public BankAccount(Person accountHolder, string accountName, string password)
        {
            AccountHolder = accountHolder;
            AccountName = accountName;
            Iban = IBANStorage.GenerateGermanIban();
            _password = password;
        }

        public BankAccount(Person accountHolder, string accountName, string password, IBAN iban) : this(accountHolder, accountName, password)
        {
            Iban = iban;
        }

        public bool IsPasswordValid(string passwordTry) => passwordTry == _password;

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (oldPassword == _password)
            {
                _password = newPassword;
                return true;
            }
            return false;
        }

        public void Transfer(Transaction transaction)
        {
            if (transaction.ReceiverIban.Equals(Iban))
            {
                Credit += transaction.Amount;
                BankStorage.AddTransaction(transaction);
            }
            else if (transaction.SenderIban.Equals(Iban))
            {
                Credit -= transaction.Amount;
                BankStorage.AddTransaction(transaction);
            }
            else
            {
                throw new InvalidOperationException("The transaction is not be associated with this account!");
            }
        }
    }
}