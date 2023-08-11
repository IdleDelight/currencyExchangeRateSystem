using ExchangeRateSharedLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeRateSharedLib.Utilities;

namespace ExchangeRateSharedLib.Services
{
    internal class FixerApiService
    {
        private readonly string _apiKey = Constants.FIXER_API_KEY;
        private readonly string _baseUrl = Constants.FIXER_BASE_URL;

        private readonly HttpClient _httpClient;

        public FixerApiService() 
        {
            _httpClient = new HttpClient();
        }

        public async Task<ExchangeRateResponse> GetLatesRatesAsync( string baseCurrency )
        {
            try {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/latest?access_key={_apiKey}&base={baseCurrency}");
                return JsonConvert.DeserializeObject<ExchangeRateResponse>(response);
            }
            catch (HttpRequestException httpEx) {
                // You can log the exception or do some other processing
                throw new Exception("Error fetching data from the Fixer API.", httpEx);
            }
            catch (JsonSerializationException jsonEx) {
                // Handle JSON deserialization issues
                throw new Exception("Error deserializing the response from the Fixer API.", jsonEx);
            }
            catch (Exception ex) {
                // General exception catch if there are any other unexpected exceptions
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
