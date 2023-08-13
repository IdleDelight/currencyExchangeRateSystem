using System.Globalization;
using Newtonsoft.Json.Linq;

namespace ExchangeRateConsoleApp
{
    class Program
    {
        private static Dictionary<string, JObject> rateCache = new();

        private static CurrencyService currencyService = new CurrencyService();
        private static ConsoleUtility consoleUtility = new ConsoleUtility();

        static void Main( string[] args )
        {
            // Load latest rates on startup
            var latestRates = currencyService.FetchExchangeRates("latest");
            rateCache["latest"] = latestRates;

            while (true) {
                string fromCurrency, toCurrency, dateInput;
                decimal amount;
                DateTime parsedDate;

                Console.Clear();
                consoleUtility.PrintHeader("The Currency Converter");

                do {
                    Console.WriteLine("Enter the source currency code");
                    Console.Write("(ex. EUR, USD, GBP, NOK): ");
                    fromCurrency = Console.ReadLine()!.ToUpper(CultureInfo.InvariantCulture);

                    if (!currencyService.IsValidCurrency(fromCurrency)) {
                        Console.WriteLine($"Invalid currency. Please enter a valid currency code.");
                    }

                } while (!currencyService.IsValidCurrency(fromCurrency));

                Console.Clear();
                consoleUtility.PrintHeader("The Currency Converter");
                consoleUtility.PrintSubHeader($"{fromCurrency}");
                do {
                    Console.Write($"Enter the {fromCurrency} amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out amount) && amount > 0)
                        break;
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                } while (true);

                Console.Clear();
                consoleUtility.PrintHeader("The Currency Converter");
                consoleUtility.PrintSubHeader($"{fromCurrency} {amount} to");
                do {
                    Console.WriteLine("Enter the target currency code");
                    Console.Write("(ex. EUR, USD, GBP, NOK): ");
                    toCurrency = Console.ReadLine()!.ToUpper(CultureInfo.InvariantCulture);
                    if (!currencyService.IsValidCurrency(toCurrency)) {
                        Console.WriteLine($"Invalid currency. Please enter a valid currency code.");
                    }
                } while (!currencyService.IsValidCurrency(toCurrency));

                Console.Clear();
                consoleUtility.PrintHeader("The Currency Converter");
                consoleUtility.PrintSubHeader($"{fromCurrency} {amount} to {toCurrency}");
                do {
                    Console.WriteLine("Choose exchange rates from a specific date (YYYY-MM-DD),");
                    Console.Write("or press [Enter] for latest rates: ");
                    dateInput = Console.ReadLine()!;
                    if (string.IsNullOrWhiteSpace(dateInput)) {
                        dateInput = "latest";
                        break;
                    }
                } while (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, DateTimeStyles.None, out parsedDate));


                if (!rateCache.ContainsKey(dateInput)) {
                    var rates = currencyService.FetchExchangeRates(dateInput);
                    rateCache[dateInput] = rates;
                }

                Console.Clear();
                consoleUtility.PrintHeader("The Currency Converter");
                consoleUtility.PrintSubHeader($"Using {dateInput} rates for {fromCurrency} and {toCurrency}");

                decimal result = currencyService.ConvertCurrency(fromCurrency, toCurrency, amount, rateCache[dateInput]);
                Console.WriteLine($"{fromCurrency} {amount} is approximately {toCurrency} {result:N2}");

                consoleUtility.PrintFooter("\n          Press [Q] to quit\n   Press any other key to continue");

                if (Console.ReadKey(true).Key == ConsoleKey.Q) break;
            }
        }
    }
}
