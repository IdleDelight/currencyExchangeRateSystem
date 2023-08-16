using ExchangeRateDB.Model;
using Newtonsoft.Json.Linq;

namespace ExchangeRateDB.Data
{
    public static class RateDataParser
    {
        public static void UpdateRates( JObject rates, ExchangeRateDbContext dbContext )
        {
            Console.WriteLine(rates.ToString());
            if (rates == null)
                throw new ArgumentNullException(nameof(rates));

            JToken? dateToken = null; // initialize to null explicitly
            if (!rates.TryGetValue("date", out dateToken) || dateToken == null)
                throw new ArgumentException("The provided rates JObject does not contain a valid date.");

            if (!DateTime.TryParse(dateToken.ToString(), out var rateDate))
                throw new ArgumentException("The provided date value is not valid.");

            Console.WriteLine($"Parsed rate date: {rateDate}"); // **** Checking the date

            var currentCurrencies = dbContext.Currencies.ToDictionary(c => c.Symbol, c => c.Id);
            var currentRates = dbContext.Rates
                .Where(r => r.Date.Date == rateDate.Date)
                .ToDictionary(r => r.CurrencyId, r => r);

            foreach (var rate in rates) {
                if (new[] { "success", "timestamp", "base", "date" }.Contains(rate.Key)) {
                    continue;
                }

                if (rate.Value == null || !rate.Value.HasValues) {
                    continue;
                }

                if (currentCurrencies.TryGetValue(rate.Key, out var currencyId)) {
                    if (currentRates.TryGetValue(currencyId, out var existingRate)) {
                        existingRate.Value = rate.Value.Value<decimal>();
                    }
                    else {
                        var newRate = new Rate
                        {
                            Value = rate.Value.Value<decimal>(),
                            Date = rateDate,
                            CurrencyId = currencyId
                        };
                        Console.WriteLine($"Parsed rate date: {rateDate}");
                        dbContext.Rates.Add(newRate);
                    }
                }
                // Log or handle scenarios where the currency does not exist in the database.
            }

            try {
                dbContext.SaveChanges();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error updating rates: {ex.Message}");
            }
        }
    }
}
