using System;
using System.Text.RegularExpressions;

namespace Bank
{
    class Program
    {
        private static BankClerk currentClerk;

        static void Main(string[] args)
        {
            currentClerk = new BankClerk();
            MainMenu();
        }


        #region Menus


        /// <summary>
        /// Displays the Main menu.
        /// </summary>
        /// <exception cref="InvalidOperationException">In case the selected option does not exist</exception>
        private static void MainMenu()
        {
            while (true)
            {
                string[] availableOptions =
            {
                "Create New Account",
                "Log in to manage your existing account",
                "Show all transactions"
            };

                var selection = PromptUser(availableOptions);

                switch (selection)
                {
                    case 0:
                        AccountCreationDialog();
                        ManagementMenu();
                        break;
                    case 1:
                        LoginDialog();
                        break;
                    case 2:
                        ShowAllTransactions();
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Displays the Management menu.
        /// </summary>
        /// <exception cref="InvalidOperationException">In case the selected option does not exist</exception>
        private static void ManagementMenu()
        {
            var quit = false;

            while (!quit)
            {
                Console.Clear();
                WriteHeader();
                Console.WriteLine($"Hi {currentClerk.SessionAccount.AccountHolder}! Account: {currentClerk.SessionAccount.AccountName}");
                Console.WriteLine($"Your balance: {currentClerk.SessionAccount.Credit}");

                string[] availableOptions =
                {
                "Use ATM to put money",
                "Transfer Money",
                "Delete this account",
                "Change Password",
                "Log out"
                };

                var selection = PromptUser(availableOptions, false);

                switch (selection)
                {
                    case 0:
                        ATMDialog();
                        break;
                    case 1:
                        TransferMoneyDialog();
                        break;
                    case 2:
                        if (DeleteAccountDialog())
                        {
                            currentClerk = null;
                            return;
                        }
                        break;
                    case 3:
                        ChangePasswordDialog();
                        break;
                    case 4:
                        currentClerk = null;
                        quit = true;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        #endregion

        #region Dialogs

        /// <summary>
        /// Displays the creation dialog.
        /// </summary>
        private static void AccountCreationDialog()
        {
            Console.Clear();
            WriteHeader();

            currentClerk = new BankClerk();

            Console.WriteLine("\nCreate new account:");

            Console.Write("First name: ");
            var firstName = Console.ReadLine();

            Console.Write("Last name: ");
            var lastName = Console.ReadLine();

            DateTime? birthDate = null;

            while (!birthDate.HasValue)
            {
                Console.Write("Birth date: ");

                try
                {
                    birthDate = DateTime.Parse(Console.ReadLine());
                }
                catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException)
                {
                    birthDate = null;
                }
            }

            var accountHolder = new Person(firstName, lastName, birthDate.Value);

            Console.Write("Account name(e.g. use case): ");
            var accountName = Console.ReadLine();

            Console.Write("Please enter a password for your account: ");
            var password = Console.ReadLine();

            var iban = currentClerk.CreateBankAccount(accountHolder, accountName, password, null);

            Console.WriteLine("\n\nAccount has been successfully created! Remember these Credentials:\n");

            Console.WriteLine($"IBAN: {iban}");
            Console.WriteLine($"Password: {password}");

            Console.WriteLine("\nPress enter to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the login dialog.
        /// </summary>
        private static void LoginDialog()
        {
            Console.Clear();
            WriteHeader();
            Console.WriteLine("Login\n");

            currentClerk = new BankClerk();

            var iban = PromptForIban();

            if(!iban.HasValue)
                return;

            var loginSuccessful = false;

            while (!loginSuccessful)
            {
                Console.WriteLine("Password: ");
                var password = Console.ReadLine();

                loginSuccessful = currentClerk.Login(iban.Value, password);
            }

            ManagementMenu();
        }

        /// <summary>
        /// Prompts for an valid existing IBAN number.
        /// </summary>
        /// <returns>IBAN. If it has no value, operation should be quit</returns>
        private static IBAN? PromptForIban()
        {
            bool ibanCanLogin = false;
            IBAN? iban = null;

            while (!ibanCanLogin)
            {
                Console.WriteLine("IBAN:[q to exit] ");

                var ibanInput = Console.ReadLine();
                ibanInput = Regex.Replace(ibanInput, @"\s+", ""); // Remove Whitespaces

                if (ibanInput == "q")
                    return null;

                if (ibanInput.Length != 22)
                {
                    Console.WriteLine("IBAN must be 22 characters long!\n");
                    continue;
                }

                try
                {
                    iban = new IBAN(ibanInput);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("IBAN is invalid!\n");
                }

                if (!iban.HasValue)
                    continue;

                if (BankStorage.DoesAccountExist(iban.Value))
                {
                    ibanCanLogin = true;
                }
                else
                {
                    Console.WriteLine("Account does not exist or is not registered!\n");
                }
            }

            return iban;
        }

        /// <summary>
        /// Displays the password change dialog.
        /// </summary>
        private static void ChangePasswordDialog()
        {
            Console.Clear();
            WriteHeader();
            Console.WriteLine("Change your password\n");

            var successfulPasswordChange = false;

            while (!successfulPasswordChange)
            {
                Console.WriteLine("Old password: ");
                var oldPassword = Console.ReadLine();

                Console.WriteLine("New Password: ");
                var newPassword = Console.ReadLine();

                if ((successfulPasswordChange = currentClerk.ChangePassword(oldPassword, newPassword)) == false)
                    Console.WriteLine("The old password is not correct! Please try again.\n");
            }

            Console.WriteLine("Remember your new password! Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the account deletion dialog.
        /// </summary>
        /// <returns>If the Account has successfully been deleted</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the clerk returns an unexpected result</exception>
        private static bool DeleteAccountDialog()
        {
            Console.Clear();
            WriteHeader();
            Console.WriteLine("Delete your account\nBe careful! This cannot be undone.");

            // Check if user is sober

            bool sober = false;

            while (!sober)
            {
                var rand = new Random();
                var firstNumber = rand.Next(0, 50);
                var secondNumber = rand.Next(0, 50);

                int result;
                string input;
                do
                {
                    Console.WriteLine($"Please solve this math operation to prove you're sober[q to exit] :) \n{firstNumber}+{secondNumber}=");
                    input = Console.ReadLine();

                    if (input == "q")
                        return false;
                } while (!int.TryParse(input, out result));

                sober = firstNumber + secondNumber == result;

                if (!sober)
                    Console.WriteLine("\nIt doesn't seem that you are sober!");
            }

            while (true)
            {
                Console.WriteLine("Password: ");
                var password = Console.ReadLine();

                switch (currentClerk.DeleteBankAccount(password))
                {
                    case BankTransactionState.Success:
                        Console.WriteLine("Success! Press any key to continue...");
                        Console.ReadKey();
                        return true;
                    case BankTransactionState.DebtsNotCleared:
                        Console.WriteLine("Could not delete the account because you still owe money to the bank! Please clear your debts and try it again. Press any key to continue...");
                        Console.ReadKey();
                        return false;
                    case BankTransactionState.MoneyRemaining:
                        Console.WriteLine("This account still contains money? Do you really want to delete it? (y/n)");
                        if (Console.ReadLine() == "y")
                        {
                            currentClerk.DeleteBankAccount(password, true);
                            Console.WriteLine("Success! Press any key to continue...");
                            Console.ReadKey();
                            return true;
                        }

                        Console.WriteLine("Aborted! Press any key to continue...");
                        Console.ReadKey();
                        return false;
                    case BankTransactionState.InvalidPassword:
                        Console.WriteLine("Your password didn't match! Please try again.\n");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Displays the dialog to put money on the account.
        /// </summary>
        private static void ATMDialog()
        {
            Console.Clear();
            WriteHeader();

            double amount;
            do
            {
                Console.WriteLine("How much money in euros do you want to put on your account?:");
            } while (!double.TryParse(Console.ReadLine(), out amount) || amount <= 0);

            currentClerk.PutMoney(new Money(amount, CurrencyCode.EUR));

            Console.WriteLine($"Successfully put {amount} EUR on your account. Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the money transfer dialog.
        /// </summary>
        private static void TransferMoneyDialog()
        {
            Console.Clear();
            WriteHeader();
            Console.WriteLine("Transfer money to another account\n");

            double amountValue;
            do
            {
                Console.WriteLine("Enter the amount in euros you want to transfer: ");
            } while (!double.TryParse(Console.ReadLine(), out amountValue) || amountValue <= 0);

            var amount = new Money(amountValue, CurrencyCode.EUR);

            Console.WriteLine("\nEnter the IBAN of the account you want to transfer the money to:");
            var iban = PromptForIban();

            if (!iban.HasValue)
                return;

            Console.WriteLine("\nEnter a payment reference: ");
            var reference = Console.ReadLine();

            Console.Clear();

            Console.WriteLine($"Receiver: {BankStorage.GetAccount(iban.Value).AccountHolder} <{iban.Value}>");
            Console.WriteLine($"Amount: {amount}");
            Console.WriteLine($"Reference: {reference}");

            Console.WriteLine("\nDo you really want to start the transaction? [y/n]");
            var confirmation = Console.ReadLine();

            if (confirmation != "y")
                return;

            Console.WriteLine(currentClerk.TransferMoney(amount, iban.Value, reference) == BankTransactionState.Success
                ? "Success! Press any key to continue..."
                : "Something went wrong! Please try again later. Press any key to continue...");

            Console.ReadKey();
        }

        /// <summary>
        /// Shows all transactions.
        /// </summary>
        private static void ShowAllTransactions()
        {
            Console.Clear();
            WriteHeader();

            if (BankStorage.transactionStorage.Count == 0)
            {
                Console.WriteLine("There are no transactions yet.");
            }
            else
            {
                Console.WriteLine("        Sender IBAN       |      Receiver IBAN       |  Amount  |      Date      |   Reference   ");
                foreach (var transactionPair in BankStorage.transactionStorage)
                {
                    var transaction = transactionPair.Value;
                    Console.WriteLine($"{transaction.SenderIban.ToString().PadLeft(26, ' ')}|{transaction.ReceiverIban.ToString().PadLeft(26, ' ')}|{transaction.Amount.ToString().PadLeft(10, ' ')}|{transaction.DateTime.ToString("g").PadLeft(16, ' ')}| {transaction.ReferenceText.PadLeft(20, ' ')}");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        #endregion

        #region Helper Methods


        static int PromptUser(string[] options) => PromptUser(options, true);

        /// <summary>
        /// Prompts the user for given options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="hasToClear">If set to <c>true</c> the view will be cleared.</param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">In case of an error which should never happen</exception>
        static int PromptUser(string[] options, bool hasToClear)
        {
            if (hasToClear)
            {
                Console.Clear();
                WriteHeader();
            }

            Console.Write("\n");
            Console.WriteLine("Press the up/down arrows or enter to select your choice:\n");

            var startLine = Console.CursorTop;
            var longestStringLength = 0;

            for (int i = 0; i < options.Length; i++)
            {
                var optionText = options[i];
                Console.WriteLine($"{i + 1}. {optionText}");

                if (optionText.Length > longestStringLength)
                    longestStringLength = optionText.Length + 3; // +3 because of index number
            }

            var hasChosenOption = false;
            var currentSelectedLine = startLine;
            var starPositionLeft = longestStringLength + 3;

            // Display Selection *

            while (!hasChosenOption)
            {
                Console.SetCursorPosition(starPositionLeft, currentSelectedLine);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("*");

                var key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.Enter:
                        hasChosenOption = true;

                        Console.ForegroundColor = ConsoleColor.Gray;
                        return currentSelectedLine - startLine;
                    case ConsoleKey.UpArrow:
                        // Delete '*' char
                        DeleteChar(starPositionLeft);

                        if (currentSelectedLine - startLine > 0)
                            currentSelectedLine--;
                        break;
                    case ConsoleKey.DownArrow:
                        // Delete '*' char
                        DeleteChar(starPositionLeft);

                        if (currentSelectedLine - startLine < options.Length - 1)
                            currentSelectedLine++;
                        break;
                }
            }

            // Should never reach this code
            throw new OperationCanceledException();
        }

        /// <summary>
        /// Deletes the character on the current line and the given left position.
        /// </summary>
        /// <param name="positionLeft">The left coordinate.</param>
        private static void DeleteChar(int positionLeft)
        {
            Console.SetCursorPosition(positionLeft, Console.CursorTop);
            Console.Write(" ");
        }

        /// <summary>
        /// Displays the Header.
        /// </summary>
        private static void WriteHeader() => Console.WriteLine("BankManager by Philip Graf\n");
        

        #endregion

    }
}
