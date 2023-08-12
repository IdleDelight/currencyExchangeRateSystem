using ExchangeRateSharedLib.Services;

public static class ExchangeRateServiceFactory
{
    public static IExchangeRateService CreateFixerService()
    {
        return new FixerApiService();
    }
}

