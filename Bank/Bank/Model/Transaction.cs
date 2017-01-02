using System;

namespace Bank
{
    public class Transaction
    {
        public Money Amount { get; private set; }
        public IBAN SenderIban { get; private set; }
        public IBAN ReceiverIban { get; private set; }
        public string ReferenceText { get; private set; }
        public DateTime DateTime { get; private set; }
        public Guid ID { get; private set; }

        public Transaction(Money amount, IBAN senderIban, IBAN receiverIban, string referenceText)
        {
            Amount = amount;
            SenderIban = senderIban;
            ReceiverIban = receiverIban;
            ReferenceText = referenceText.PadRight(15, ' ').Substring(0, 15); // Max 15 chars
            DateTime = DateTime.Now;
            ID = Guid.NewGuid();
        }
    }

    public class CashTransaction : Transaction
    {
        public CashTransaction(Money amount, IBAN receiverIban, string referenceText) : base(amount, Configuration.BankIban, receiverIban, referenceText) { }
    }
}