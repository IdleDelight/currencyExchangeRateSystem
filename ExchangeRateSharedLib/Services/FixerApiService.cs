using ExchangeRateSharedLib.Models;
using Newtonsoft.Json;
using ExchangeRateSharedLib.Utilities;

namespace ExchangeRateSharedLib.Services
{
    internal class FixerApiService
    {
        private readonly string _apiKey = Constants.FixerApiKey;
        private readonly string _baseUrl = Constants.FixerBaseUrl;

        private readonly HttpClient _httpClient;

        public FixerApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ExchangeRateResponse> GetLatesRatesAsync( string baseCurrency )
        {
            if (string.IsNullOrEmpty(baseCurrency)) {
                throw new ArgumentException("Base currency cannot be null or empty.", nameof(baseCurrency));
            }

            if (!Constants.FixerValidSymbols.Contains(baseCurrency.ToUpper())) {
                throw new ArgumentException($"{baseCurrency} is not a valid currency symbol.", nameof(baseCurrency));
            }

            try {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}latest?access_key={_apiKey}&base={baseCurrency}");

                if (string.IsNullOrEmpty(response)) {
                    throw new Exception("Received an empty response from the Fixer API.");
                }

                var exchangeRateResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);

                if (exchangeRateResponse == null) {
                    throw new DeserializationException("Deserialized response resulted in a null object.");
                }

                return exchangeRateResponse;
            }
            catch (HttpRequestException httpEx) {
                LogError($"Failed to fetch data from Fixer API. Error: {httpEx.Message}");
                throw new ApiException("Error fetching data from the Fixer API.", httpEx);
            }
            catch (JsonSerializationException jsonEx) {
                LogError($"Failed to deserialize response from Fixer API. Error: {jsonEx.Message}");
                throw new DeserializationException("Error deserializing the response from the Fixer API.", jsonEx);
            }
            catch (Exception ex) {
                LogError($"An unexpected error occurred. Error: {ex.Message}");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }


        private static void LogError( string message )
        {
            Console.WriteLine($"[Error] {DateTime.UtcNow}: {message}");
        }

        public class ApiException : Exception
        {
            public ApiException( string message, Exception innerException ) : base(message, innerException) { }
        }

        public class DeserializationException : Exception
        {
            public DeserializationException( string message ) : base(message) { }
            public DeserializationException( string message, Exception innerException ) : base(message, innerException) { }
        }


    }
}
