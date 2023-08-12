using ExchangeRateSharedLib.Models;

public interface IExchangeRateService
{
    Task<ExchangeRateResponse> GetLatestRatesAsync( string baseCurrency );
}
