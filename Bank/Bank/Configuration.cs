namespace Bank
{
    public static class Configuration
    {
        public static readonly int BankCode = 30120400; // 8-digit
        public static readonly int BankTransactionsAccountNumber = 0000000001;
        public static readonly IBAN BankIban = new IBAN(CountryCode.DE, IBANStorage.GetIbanCheckSum(BankCode, BankTransactionsAccountNumber, CountryCode.DE), BankCode, BankTransactionsAccountNumber);
    }
}