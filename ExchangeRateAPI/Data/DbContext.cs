using ExchangeRateDB.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExchangeRateDB.Data
{
    public class ExchangeRateDbContext : DbContext
    {
        public ExchangeRateDbContext( DbContextOptions<ExchangeRateDbContext> options )
            : base(options)
        { }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<UpdateLog> UpdateLogs { get; set; }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<Currency>()
                .HasMany(c => c.Rates)
                .WithOne(r => r.Currency)
                .HasForeignKey(r => r.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rate>().Property(r => r.Value)
                .HasColumnType("decimal(20, 10)");

            var currencySeedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "CurrencySeedData.json");
            var currenciesJson = File.ReadAllText(currencySeedDataPath);

            var currencyData = JsonConvert.DeserializeObject<Dictionary<string, string>>(currenciesJson);
            if (currencyData == null) {
                throw new InvalidOperationException("Failed to deserialize the currency seed data.");
            }

            var currenciesList = new List<Currency>();
            int idCounter = 1;
            foreach (var entry in currencyData) {
                currenciesList.Add(new Currency
                {
                    Id = idCounter++,
                    Symbol = entry.Key,
                    Name = entry.Value
                });
            }

            modelBuilder.Entity<Currency>().HasData(currenciesList);

            var rateSeedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "RateSeedData.json");
            var rateSeedData = File.ReadAllText(rateSeedDataPath);
            var ratesJson = JObject.Parse(rateSeedData)["rates"] as JObject;
            var ratesList = new List<Rate>();

            if (ratesJson != null) {
                foreach (var rate in ratesJson) {
                    var currency = currenciesList.FirstOrDefault(c => c.Symbol == rate.Key);
                    if (currency != null) {
                        ratesList.Add(new Rate
                        {
                            Value = rate.Value.Value<decimal>(),
                            Date = DateTime.UtcNow,
                            CurrencyId = currency.Id
                        });
                    }
                }
            }

            modelBuilder.Entity<Rate>().HasData(ratesList);

            modelBuilder.Entity<UpdateLog>().HasData(
                new UpdateLog { Id = 1, DbUpdateDate = DateTime.Now, ApiCallSuccess = false, Message = "DB Created" }
            );
        }
    }
}