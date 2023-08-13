using ExchangeRateSharedLib.Services;
using ExchangeRateSharedLib.Utilities;

namespace ExchangeRateSharedLib.Converters
{
    public class CurrencyConverter
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly string _baseCurrency = Constants.FixerBaseCurrency;

        public CurrencyConverter( IExchangeRateService exchangeRateService )
        {
            _exchangeRateService = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
        }

        public async Task<decimal> ConvertAsync( string fromCurrency, string toCurrency, decimal amount )
        {
            // If both currencies are EUR, return the amount unchanged.
            if (fromCurrency == _baseCurrency && toCurrency == _baseCurrency) {
                return amount;
            }

            // Fetch the latest rates for the fromCurrency.
            var fromCurrencyRates = await _exchangeRateService.GetLatestRatesAsync(fromCurrency);

            // If converting from EUR, then set its rate with respect to itself as 1.
            decimal rateFromToEUR = fromCurrency == _baseCurrency ? 1M : fromCurrencyRates.Rates[_baseCurrency];

            // Fetch the latest rates for the toCurrency.
            var toCurrencyRates = await _exchangeRateService.GetLatestRatesAsync(toCurrency);

            // If converting to EUR, then set its rate with respect to itself as 1.
            decimal rateEURToTo = toCurrency == _baseCurrency ? 1M : 1M / toCurrencyRates.Rates[_baseCurrency];

            return amount * rateFromToEUR * rateEURToTo;
        }

    }
}