using ExchangeRateDB.Data;
using ExchangeRateSharedLib;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateUpdater
{
    public class Program
    {
        public static void Main( string[] args )
        {
            // Relative path from ExchangeRateUpdater to FixerSettings.json in ExchangeRateSharedLib
            var relativePathToSettings = @"..\ExchangeRateSharedLib\FixerSettings.json";
            var fullPath = Path.GetFullPath(relativePathToSettings);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile(fullPath, optional: false);

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
                    services.AddDbContext<ExchangeRateDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}