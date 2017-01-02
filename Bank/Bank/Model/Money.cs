using System;

namespace Bank
{
    public struct Money
    {
        public double Amount { get; private set; }
        public CurrencyCode Currency { get; private set; }

        public Money(double amount, CurrencyCode currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money operator +(Money first, Money second)
        {
            if (first.Currency != second.Currency)
                second.Convert(first.Currency);

            return new Money(first.Amount + second.Amount, first.Currency);
        }

        public static Money operator -(Money first, Money second)
        {
            if (first.Currency != second.Currency)
                second.Convert(first.Currency);

            return new Money(first.Amount - second.Amount, first.Currency);
        }

        public void Convert(CurrencyCode toCurrency)
        {
            if (Currency == toCurrency)
                return;

            double amountInUSD = Amount*Bank.Currency.GetExchangeRate(Currency);
            double amountInDesiredCurrency = amountInUSD/Bank.Currency.GetExchangeRate(toCurrency);

            Amount = amountInDesiredCurrency;
            Currency = toCurrency;
        }

        public override string ToString()
        {
            return $"{Amount} {Enum.GetName(typeof(CurrencyCode), Currency)}";
        }
    }
}
