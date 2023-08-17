using ExchangeRateDB.Model;
using Newtonsoft.Json.Linq;

namespace ExchangeRateDB.Data
{
    public class RatesDataProcessor
    {
        public List<Rate> ProcessRatesData( JObject ratesData, List<Currency> currenciesList )
        {
            var ratesJson = ratesData["rates"] as JObject;
            var rateDate = ratesData["date"]?.Value<string>();
            var parsedDate = DateTime.Parse(rateDate);

            var ratesList = new List<Rate>();
            int rateIdCount = 1;  // You might want to adjust this if you're storing in a database with auto-increment.

            if (ratesJson != null) {
                foreach (var rate in ratesJson) {
                    var currency = currenciesList.FirstOrDefault(c => c.Symbol == rate.Key);
                    if (currency != null) {
                        ratesList.Add(new Rate
                        {
                            Id = rateIdCount++,  // Adjust as needed
                            Value = rate.Value?.Type == JTokenType.Float ? rate.Value.Value<decimal>() : 0M,
                            Date = parsedDate.Date,
                            CurrencyId = currency.Id
                        });
                    }
                }
            }

            return ratesList;
        }
    }
}
