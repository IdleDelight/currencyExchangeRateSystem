using Newtonsoft.Json.Linq;

namespace ExchangeRateConsoleApp
{
    public class CurrencyService
    {
        private const string AccessKey = Constants.FixerApiKey;
        private const string BaseApi = Constants.FixerBaseUrl;

        public bool IsValidCurrency( string currency )
        {
            return !string.IsNullOrEmpty(currency) && Constants.FixerValidSymbols.Contains(currency);
        }

        public JObject FetchExchangeRates( string date )
        {
            using var client = new HttpClient();
            return FetchExchangeRates(date, client);
        }

        public JObject FetchExchangeRates( string date, HttpClient httpClient )
        {
            var result = httpClient.GetStringAsync($"{BaseApi}{date}?access_key={AccessKey}").Result;
            var data = JObject.Parse(result);
            bool success = data["success"]?.ToObject<bool>() ?? false;

            if (!success) {
                throw new Exception("Failed to fetch exchange rates.");
            }

            return data["rates"] as JObject ?? new JObject();
        }

        public decimal ConvertCurrency( string from, string to, decimal amount, JObject rates )
        {
            if (!rates.TryGetValue(from, out JToken? fromRate) || fromRate == null) {
                throw new Exception($"Rate for currency {from} not found.");
            }

            if (!rates.TryGetValue(to, out JToken? toRate) || toRate == null) {
                throw new Exception($"Rate for currency {to} not found.");
            }

            if (from == Constants.FixerBaseCurrency) {
                return amount * toRate.Value<decimal>();
            }

            if (to == Constants.FixerBaseCurrency) {
                return amount / fromRate.Value<decimal>();
            }

            decimal toEuroRate = 1 / fromRate.Value<decimal>();
            return amount * toEuroRate * toRate.Value<decimal>();
        }
    }
}