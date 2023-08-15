using Newtonsoft.Json.Linq;

namespace ExchangeRateSharedLib
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly string AccessKey;
        private readonly string BaseApi;
        private readonly string BaseCurrency;
        private readonly HashSet<string> ValidSymbols;

        public CurrencyService( HttpClient httpClient, string apiKey, string baseUrl, string baseCurrency, string[] validSymbols )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            AccessKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            BaseApi = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            ValidSymbols = new HashSet<string>(validSymbols) ?? throw new ArgumentNullException(nameof(validSymbols));
        }

        public bool IsValidCurrency( string currency )
        {
            return !string.IsNullOrEmpty(currency) && ValidSymbols.Contains(currency);
        }

        public JObject FetchExchangeRates( string date )
        {
            return FetchExchangeRates(date, _httpClient);
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