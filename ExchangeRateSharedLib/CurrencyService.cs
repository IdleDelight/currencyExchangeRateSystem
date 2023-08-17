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

        public async Task<JObject> FetchExchangeRates( string date )
        {
            return await FetchExchangeRates(date, _httpClient);
        }

        public async Task<JObject> FetchExchangeRates( string date, HttpClient httpClient )
        {
            var result = await httpClient.GetStringAsync($"{BaseApi}{date}?access_key={AccessKey}");
            var data = JObject.Parse(result);
            Console.WriteLine($"Fetched Exchange Rates Data: {data}");

            bool success = data["success"]?.ToObject<bool>() ?? false;

            if (!success) {
                throw new Exception("Failed to fetch exchange rates.");
            }

            return data;
        }

        public decimal ConvertCurrency( string from, string to, decimal amount, JObject data )
        {
            bool? success = data["success"]?.Value<bool>();
            if (!success.HasValue || !success.Value) {
                throw new Exception("Data fetch was not successful.");
            }

            JObject rates = data["rates"]?.Value<JObject>()!;
            if (rates == null) {
                throw new Exception("Rates data was not found.");
            }

            if (!rates.TryGetValue(from, out JToken? fromRate) || fromRate == null) {
                throw new Exception($"Rate for currency {from} not found.");
            }

            if (!rates.TryGetValue(to, out JToken? toRate) || toRate == null) {
                throw new Exception($"Rate for currency {to} not found.");
            }

            // Calculate the converted amount.
            if (from == BaseCurrency) {
                return amount * toRate.Value<decimal>();
            }

            if (to == BaseCurrency) {
                return amount / fromRate.Value<decimal>();
            }

            decimal toBaseCurrencyRate = 1 / fromRate.Value<decimal>();
            return amount * toBaseCurrencyRate * toRate.Value<decimal>();
        }
    }
}