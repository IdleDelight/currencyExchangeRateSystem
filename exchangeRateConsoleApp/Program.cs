using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ExchangeRateConsoleApp
{
    class Program
    {
        private const string AccessKey = Constants.FixerApiKey;
        private const string BaseApi = Constants.FixerBaseUrl;
        private static Dictionary<string, JObject> rateCache = new Dictionary<string, JObject>();

        static void Main( string[] args )
        {
            // Load latest rates on startup
            var latestRates = FetchExchangeRates("latest");
            rateCache["latest"] = latestRates;

            while (true) {
                string fromCurrency, toCurrency, dateInput;
                decimal amount;
                DateTime parsedDate;

                Console.Clear();
                PrintHeader("The Currency Converter");
                do {
                    Console.Write("Enter the source currency code: ");
                    fromCurrency = Console.ReadLine()!.ToUpper(CultureInfo.InvariantCulture);
                    if (!IsValidCurrency(fromCurrency)) {
                        Console.WriteLine($"Invalid currency. Please enter a valid currency code.");
                    }
                } while (!IsValidCurrency(fromCurrency));

                Console.Clear();
                PrintHeader("The Currency Converter");
                PrintSubHeader($"{fromCurrency}");
                do {
                    Console.Write($"Enter the {fromCurrency} amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out amount) && amount > 0)
                        break;
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                } while (true);

                Console.Clear();
                PrintHeader("The Currency Converter");
                PrintSubHeader($"{fromCurrency} {amount} to");
                do {
                    Console.Write("Enter the target currency code: ");
                    toCurrency = Console.ReadLine()!.ToUpper(CultureInfo.InvariantCulture);
                    if (!IsValidCurrency(toCurrency)) {
                        Console.WriteLine($"Invalid currency. Please enter a valid currency code.");
                    }
                } while (!IsValidCurrency(toCurrency));

                Console.Clear();
                PrintHeader("The Currency Converter");
                PrintSubHeader($"{fromCurrency} {amount} to {toCurrency}");
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
                    var rates = FetchExchangeRates(dateInput);
                    rateCache[dateInput] = rates;
                }

                Console.Clear();
                PrintHeader("The Currency Converter");
                PrintSubHeader($"Using {dateInput} rates for {fromCurrency} and {toCurrency}");
                decimal result = ConvertCurrency(fromCurrency, toCurrency, amount, rateCache[dateInput]);
                Console.WriteLine($"{fromCurrency} {amount} is approximately {toCurrency} {result:N2}");
                PrintFooter("\n          Press [Q] to quit\n   Press any other key to continue");

                if (Console.ReadKey(true).Key == ConsoleKey.Q) break;
            }
        }

        private static bool IsValidCurrency( string currency )
        {
            return !string.IsNullOrEmpty(currency) && Constants.FixerValidSymbols.Contains(currency);
        }

        private static JObject FetchExchangeRates( string date )
        {
            using var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync($"{BaseApi}{date}?access_key={AccessKey}").Result;
            var data = JObject.Parse(result);

            if (!(bool)data["success"]) {
                throw new Exception("Failed to fetch exchange rates.");
            }

            return (JObject)data["rates"];
        }

        private static decimal ConvertCurrency( string from, string to, decimal amount, JObject rates )
        {
            if (from == Constants.FixerBaseCurrency) {
                return amount * rates[to].Value<decimal>();
            }

            if (to == Constants.FixerBaseCurrency) {
                return amount / rates[from].Value<decimal>();
            }

            decimal toEuroRate = 1 / rates[from].Value<decimal>();
            return amount * toEuroRate * rates[to].Value<decimal>();
        }

        private static void PrintHeader( string text )
        {
            Console.WriteLine("======================================");
            Console.WriteLine($"        {text}");
            Console.WriteLine("======================================");
        }

        private static void PrintSubHeader( string text )
        {
            Console.WriteLine(text);
            Console.WriteLine("--------------------------------------");
        }

        private static void PrintFooter( string text )
        {
            Console.WriteLine("======================================");
            Console.WriteLine(text);
        }
    }
}
