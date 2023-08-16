using ExchangeRateSharedLib;

namespace ExchangeRateUpdater
{
    public class Program
    {
        public static void Main( string[] args )
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("FixerSettings.json", optional: false);

            var configuration = builder.Build();
            var fixerSettings = configuration.GetSection("FixerSettings");

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(( hostContext, services ) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<CurrencyService>(sp =>
                    {
                        var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();

                        var apiKey = fixerSettings["ApiKey"] ?? throw new InvalidOperationException("API key is missing from configuration.");
                        var baseUrl = fixerSettings["BaseUrl"] ?? throw new InvalidOperationException("Base URL is missing from configuration.");
                        var baseCurrency = fixerSettings["BaseCurrency"] ?? throw new InvalidOperationException("Base currency is missing from configuration.");
                        var validSymbols = fixerSettings.GetSection("ValidSymbols").Get<string[]>()
                                           ?? throw new InvalidOperationException("Valid symbols are missing from configuration.");

                        return new CurrencyService(httpClient, apiKey, baseUrl, baseCurrency, validSymbols);
                    });
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}