using ExchangeRateDB.Model;
using Newtonsoft.Json.Linq;

namespace ExchangeRateDB.Data
{
    public static class RateDataParser
    {
        public static void UpdateRates( JObject rates, ExchangeRateDbContext dbContext, DateTime rateDate )
        {
            foreach (var rate in rates) {
                if (rate.Value == null || !rate.Value.HasValues) {
                    continue;
                }

                var currency = dbContext.Currencies.FirstOrDefault(c => c.Symbol == rate.Key);
                if (currency != null) {
                    var existingRate = dbContext.Rates.FirstOrDefault(r => r.CurrencyId == currency.Id && r.Date.Date == rateDate.Date);

                    if (existingRate == null) {
                        var newRate = new Rate
                        {
                            Value = rate.Value.Value<decimal>(),
                            Date = rateDate,
                            CurrencyId = currency.Id
                        };
                        dbContext.Rates.Add(newRate);
                    }
                    else {
                        existingRate.Value = rate.Value.Value<decimal>();
                    }
                }
            }
            dbContext.SaveChanges();
        }
    }
}
