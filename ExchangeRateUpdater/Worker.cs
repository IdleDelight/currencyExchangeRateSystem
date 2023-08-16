using ExchangeRateSharedLib;
using Newtonsoft.Json.Linq;
using ExchangeRateDB.Data;
using ExchangeRateDB.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateUpdater
{
    public class Worker : BackgroundService
    {
        private readonly CurrencyService _currencyService;
        private readonly ExchangeRateDbContext _dbContext;
        private readonly ILogger<Worker> _logger;

        public Worker( ILogger<Worker> logger, CurrencyService currencyService, ExchangeRateDbContext dbContext )
        {
            _logger = logger;
            _currencyService = currencyService;
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try {
                    JObject rates = _currencyService.FetchExchangeRates("latest");

                    // Use the RateDataParser to insert or update rates
                    RateDataParser.UpdateRates(rates, _dbContext, DateTime.UtcNow);

                    // Log the update to the UpdateLog table
                    var updateLog = new UpdateLog
                    {
                        DbUpdateDate = DateTime.UtcNow,
                        ApiCallSuccess = true,
                        Message = "Updated rates successfully"
                    };
                    _dbContext.UpdateLogs.Add(updateLog);
                    _dbContext.SaveChanges();

                    _logger.LogInformation("Fetched and stored rates: {rates}", rates.ToString());
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error fetching rates.");

                    // Log the failed update to the UpdateLog table
                    var updateLog = new UpdateLog
                    {
                        DbUpdateDate = DateTime.UtcNow,
                        ApiCallSuccess = false,
                        Message = ex.Message
                    };
                    _dbContext.UpdateLogs.Add(updateLog);
                    _dbContext.SaveChanges();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
