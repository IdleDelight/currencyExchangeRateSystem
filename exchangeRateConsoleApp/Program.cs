using System.Globalization;
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
                string fromCurrency;
                do {
                    Console.WriteLine("####################################");
                    Console.WriteLine("#### The Currency Converter App ####");
                    Console.WriteLine("####################################");
                    Console.Write("Enter the source currency code: ");
                    fromCurrency = Console.ReadLine()!.ToUpper(CultureInfo.InvariantCulture);
                    if (!IsValidCurrency(fromCurrency)) {
                        Console.Write($"Invalid currency. Please enter a valid currency codes: ");
                    }
                } while (!IsValidCurrency(fromCurrency));

                decimal amount;
                do {
                    Console.Write($"Enter the {fromCurrency} amount: ");
                    var input = Console.ReadLine();
                    if (!decimal.TryParse(input, out amount) || amount <= 0) {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                    }
                } while (amount <= 0);

                string toCurrency;
                do {
                    Console.Write("Enter the target currency code: ");
                    toCurrency = Console.ReadLine()!.ToUpper(CultureInfo.InvariantCulture);
                    if (!IsValidCurrency(toCurrency)) {
                        Console.Write($"\nInvalid currency. Please enter a valid currency codes: ");
                    }
                } while (!IsValidCurrency(toCurrency));

                string dateInput;
                DateTime parsedDate;
                do {
                    Console.WriteLine("Choose exchange rates from a specific date (YYYY-MM-DD),");
                    Console.Write("or press [Enter] for latest rates: ");
                    dateInput = Console.ReadLine()!;

                    if (string.IsNullOrWhiteSpace(dateInput)) {
                        dateInput = "latest";
                        break;
                    }

                    if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, DateTimeStyles.None, out parsedDate)) {
                        Console.WriteLine("\nInvalid date format. Please enter a date in the format");
                        Console.Write("YYYY-MM-DD: ");
                    }

                } while (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, DateTimeStyles.None, out parsedDate) && !string.IsNullOrWhiteSpace(dateInput));

                if (!rateCache.ContainsKey(dateInput)) {
                    var rates = FetchExchangeRates(dateInput);
                    rateCache[dateInput] = rates;
                }

                decimal result = ConvertCurrency(fromCurrency, toCurrency, amount, rateCache[dateInput]);
                Console.WriteLine("####################################");
                Console.WriteLine($"\n{fromCurrency} to {toCurrency}, with {dateInput} rates");
                Console.WriteLine($"{fromCurrency} {amount} is approximately {toCurrency} {result:N2}");
                Console.WriteLine("####################################");

                Console.WriteLine("\n\nPress [Q] to quit, or any other key to continue...");
                if (Console.ReadLine()!.ToLower() == "q") break;
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
    }
}