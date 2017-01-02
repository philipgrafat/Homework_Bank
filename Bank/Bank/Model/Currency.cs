using System;

namespace Bank
{
    public static class Currency
    {
        // Values are in USD (Last update 12/28/2016 by PG) http://www.xe.com/currencytables/?money=USD

        private static double USD = 1.0;          // US Dollar
        private static double EUR = 1.0442232039; // Euro
        private static double GBP = 1.2241724577; // British Pound
        private static double INR = 0.0146787779; // Indian Rupee
        private static double AUD = 0.7193469358; // Australian Dollar
        private static double CAD = 0.7370899493; // Canadian Dollar
        private static double SGD = 0.6892326550; // Singapore Dollar
        private static double CHF = 0.9724892260; // Swiss Franc
        private static double JPY = 0.0084973593; // Japanese Yen
        private static double CNY = 0.1437105571;  //Chinese Yuan Renminbi

        public static double GetExchangeRate(CurrencyCode currencyCode)
        {
            switch (currencyCode)
            {
                case CurrencyCode.USD:
                    return USD;
                case CurrencyCode.EUR:
                    return EUR;
                case CurrencyCode.GBP:
                    return GBP;
                case CurrencyCode.INR:
                    return INR;
                case CurrencyCode.AUD:
                    return AUD;
                case CurrencyCode.CAD:
                    return CAD;
                case CurrencyCode.SGD:
                    return SGD;
                case CurrencyCode.CHF:
                    return CHF;
                case CurrencyCode.JPY:
                    return JPY;
                case CurrencyCode.CNY:
                    return CNY;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currencyCode), currencyCode, "This currency hasn't been defined!");
            }
        }
    }

    public enum CurrencyCode
    {
        USD, EUR, GBP, INR, AUD, CAD, SGD, CHF, JPY, CNY
    }
}