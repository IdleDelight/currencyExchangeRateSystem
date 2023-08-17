using ExchangeRateDB.Data;
using ExchangeRateDB.Model;
using ExchangeRateSharedLib;
using Newtonsoft.Json.Linq;

namespace ExchangeRateUpdater
{
    public class Worker : BackgroundService
    {
        private readonly CurrencyService _currencyService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Worker> _logger;
        private readonly string _specificDate;

        public Worker( ILogger<Worker> logger, CurrencyService currencyService, IServiceScopeFactory scopeFactory, string specificDate = "2023-08-09" )
        {
            _logger = logger;
            _currencyService = currencyService;
            _scopeFactory = scopeFactory;
            _specificDate = specificDate;
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
                    JObject apiData = await _currencyService.FetchExchangeRates(_specificDate);

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeRateDbContext>();

                    var ratesDataProcessor = new RatesDataProcessor(dbContext);

                    // Retrieve all currencies currently stored in the database
                    var currenciesList = dbContext.Currencies.ToList();

                    // Process and directly save rates data
                    bool isSuccess = ratesDataProcessor.ProcessRatesData(apiData, currenciesList);

                    // Log the update to the UpdateLog table
                    var updateLog = new UpdateLog
                    {
                        DbUpdateDate = DateTime.UtcNow,
                        ApiCallSuccess = isSuccess,
                        Message = isSuccess ? "Updated rates successfully" : "Error while updating rates"
                    };
                    dbContext.UpdateLogs.Add(updateLog);
                    dbContext.SaveChanges();

                    _logger.LogInformation("Fetched and stored rates: {rates}", apiData.ToString());
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

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

            } while (!stoppingToken.IsCancellationRequested);
        }
    }
}