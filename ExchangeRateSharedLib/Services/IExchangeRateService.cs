using ExchangeRateSharedLib.Models;

namespace ExchangeRateSharedLib.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateResponse> GetLatestRatesAsync( string baseCurrency );
    }
}
