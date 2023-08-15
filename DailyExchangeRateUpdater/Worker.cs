using ExchangeRateSharedLib;
using Newtonsoft.Json.Linq;

namespace ExchangeRateUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CurrencyService _currencyService;

        public Worker( ILogger<Worker> logger, CurrencyService currencyService )
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try {
                    JObject rates = _currencyService.FetchExchangeRates("latest");

                    // TODO: Store rates into the database.
                    // For now, just log the fetched rates.
                    _logger.LogInformation("Fetched rates: {rates}", rates.ToString());

                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error fetching rates.");
                }

                // Consider adding a delay to run this update process once a day.
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}