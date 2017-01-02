using System;
using System.Collections.Generic;
using System.Numerics;

namespace Bank
{
    public struct IBAN
    {
        public CountryCode Country { get; }
        public int CheckSum { get; }
        public int BankCode { get; }

        /// <summary>
        /// Gets the account number. Can be 10 digits max. filled up with zeroes
        /// </summary>
        /// <value>
        /// The account number.
        /// </value>
        public long AccountNumber { get; }

        public IBAN(CountryCode country, int checkSum, int bankCode, long accountNumber)
        {
            Country = country;
            CheckSum = checkSum;
            BankCode = bankCode;
            AccountNumber = accountNumber;
        }

        public IBAN(string fromString)
        {
            if(fromString.Length != 22)
                throw new ArgumentException("German IBAN must be 22 characters long");

            var countryCodeInput = fromString.Substring(0, 2);
            var countryCode = (CountryCode) Enum.Parse(typeof(CountryCode), countryCodeInput, true);

            int checkSum;
            if(!int.TryParse(fromString.Substring(2, 2), out checkSum))
                throw new ArgumentException("Checksum not at proper place");

            int bankCode;
            if (!int.TryParse(fromString.Substring(4, 8), out bankCode))
                throw new ArgumentException("Bank Code not at proper place");

            long accountNumber;
            if (!long.TryParse(fromString.Substring(12, 10), out accountNumber))
                throw new ArgumentException("Account Number not at proper place");

            Country = countryCode;
            CheckSum = checkSum;
            BankCode = bankCode;
            AccountNumber = accountNumber;
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(CountryCode), Country)}{CheckSum:00} {BankCode:00000000} {AccountNumber:0000000000}";
        }
    }

    public static class IBANStorage
    {
        private static readonly List<IBAN> UsedIbans;

        static IBANStorage()
        {
            UsedIbans = new List<IBAN>();

            UsedIbans.Add(Configuration.BankIban);
        }

        public static bool DoesIbanAlreadyExist(IBAN toCheck) => UsedIbans.Contains(toCheck);

        public static void AddToStorage(IBAN iban) => UsedIbans.Add(iban);

        /// <summary>
        /// Generates a random account number.
        /// </summary>
        /// <returns></returns>
        public static long GenerateRandomAccountNumber()
        {
            // Generate random 10 digit bank account number

            long accountNumber = 0;
            Random rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                accountNumber += rand.Next(1, 10) * (long)Math.Pow(10, i);
            }

            return accountNumber;
        }

        /// <summary>
        /// Gets the iban check sum.
        /// </summary>
        /// <param name="bankCode">The 8-digit bank code.</param>
        /// <param name="accountNumber">The 10-digit account number.</param>
        /// <param name="countryCode">The country code.</param>
        /// <returns></returns>
        public static int GetIbanCheckSum(int bankCode, long accountNumber, CountryCode countryCode)
        {
            // Generate 2-Digit CheckSum

            BigInteger bban = bankCode * (long)Math.Pow(10, 10) + accountNumber; // accountNumber+bankCode: 18 digit bban 

            char[] country = Enum.GetName(typeof(CountryCode), countryCode).ToCharArray();
            int numericCountry = ((country[0] - 64) + 9) * (int)Math.Pow(10, 2) + (country[1] - 64) + 9; // Position of country code in alphabet + 9, eg. DE = 1314
            numericCountry *= 100; // Add leading zeroes

            BigInteger checkNumber = bban * (int)Math.Pow(10, 6) + numericCountry;

            int checkSum = 98 - (int)(checkNumber % 97);

            return checkSum;
        }

        /// <summary>
        /// Generates a German IBAN Number from a random account number. Uses the Bank Code from the Configuration.cs file
        /// </summary>
        /// <returns>German IBAN number</returns>
        public static IBAN GenerateGermanIban()
        {
            return GenerateGermanIban(Configuration.BankCode);
        }

        /// <summary>
        /// Generates a German IBAN Number from a random account number
        /// </summary>
        /// <param name="bankCode">8-digit Bank Code</param>
        /// <returns>German IBAN-Number</returns>
        /// 
        public static IBAN GenerateGermanIban(int bankCode)
        {
            var accountNumber = GenerateRandomAccountNumber();
            return GenerateGermanIban(bankCode, accountNumber);
        }

        /// <summary>
        /// Generates a German IBAN Number
        /// </summary>
        /// <param name="bankCode">8-digit Bank Code</param>
        /// <param name="accountNumber">10 digit Bank Account Number</param>
        /// <returns>German IBAN Number</returns>
        public static IBAN GenerateGermanIban(int bankCode, long accountNumber)
        {
            // Currently only working for German IBANS as there are different formats in every country
            var countryCode = CountryCode.DE;
            var checkSum = GetIbanCheckSum(bankCode, accountNumber, countryCode);

            var iban = new IBAN(countryCode, checkSum, bankCode, accountNumber);
            AddToStorage(iban);

            return iban;
        }
    }
}