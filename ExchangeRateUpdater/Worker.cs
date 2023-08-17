using ExchangeRateSharedLib;
using Newtonsoft.Json.Linq;
using ExchangeRateDB.Data;
using ExchangeRateDB.Model;

namespace ExchangeRateUpdater
{
    public class Worker : BackgroundService
    {
        private readonly CurrencyService _currencyService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Worker> _logger;

        public Worker( ILogger<Worker> logger, CurrencyService currencyService, IServiceScopeFactory scopeFactory )
        {
            _logger = logger;
            _currencyService = currencyService;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            do {
                if (stoppingToken.IsCancellationRequested) {
                    _logger.LogInformation("Worker execution cancelled before it started.");
                    return;
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try {
                    JObject rates = await _currencyService.FetchExchangeRates("2023-08-15");

                    // Use the service scope factory to create a scope and get the DB context
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeRateDbContext>();

                    Console.WriteLine(rates); //<<<< I ADDED THIS

                    //// Use the RateDataParser to insert or update rates
                    //RateDataParser.UpdateRates(rates, dbContext);

                    // Log the update to the UpdateLog table
                    var updateLog = new UpdateLog
                    {
                        DbUpdateDate = DateTime.UtcNow,
                        ApiCallSuccess = true,
                        Message = "Updated rates successfully"
                    };
                    dbContext.UpdateLogs.Add(updateLog);
                    dbContext.SaveChanges();

                    _logger.LogInformation("Fetched and stored rates: {rates}", rates.ToString());
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error fetching rates.");

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeRateDbContext>();

                    // Log the failed update to the UpdateLog table
                    var updateLog = new UpdateLog
                    {
                        DbUpdateDate = DateTime.UtcNow,
                        ApiCallSuccess = false,
                        Message = ex.Message
                    };
                    dbContext.UpdateLogs.Add(updateLog);
                    dbContext.SaveChanges();
                }

                // Sleep for 24 hours
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

            } while (!stoppingToken.IsCancellationRequested);
        }
    }
}