using ExchangeRateSharedLib.Services;
using ExchangeRateSharedLib.Utilities;

namespace ExchangeRateConsoleApp
{
    internal class Program
    {
        private static readonly string[] TargetCurrencies = Constants.FixerValidSymbols.Where(symbol=> new[] { "USD", "EUR", "GBP", "JYP" }.Contains(symbol)).ToArray();

        static void Main( string[] args )
        {
            Console.WriteLine("Exchange Rate Console App");
            Console.WriteLine("-------------------------");

            Console.Write("Enter a valid base currency (ex. EUR): ");
            string baseCurrency = Console.ReadLine()!.ToUpper();

            if (string.IsNullOrWhiteSpace(baseCurrency) ) {
                Console.WriteLine("Invalid input.Exiting...");
                return;
            }

            IExchangeRateService fixerService = ExchangeRateServiceFactory.CreateFixerService();

            try {
                var exchangeRates = fixerService.GetLatestRatesAsync(baseCurrency).Result;

                Console.WriteLine($"Exchange rates for {baseCurrency}:");
                foreach ( var targetCurrency in TargetCurrencies) {
                    if (exchangeRates.Rates.TryGetValue(targetCurrency, out var rate)) {
                        Console.WriteLine($"{targetCurrency}: {rate}");
                    }
                    else {
                        Console.WriteLine($"{targetCurrency}: not available.");
                    }
                }
            }
            catch ( Exception ex ) {
                Console.WriteLine($"An error occured: {ex.Message}");
            }
        }
    }
}