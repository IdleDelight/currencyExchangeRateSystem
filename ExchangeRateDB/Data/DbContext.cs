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
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Rate>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UpdateLog>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Currency>()
                .HasMany(c => c.Rates)
                .WithOne(r => r.Currency)
                .HasForeignKey(r => r.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rate>().Property(r => r.Value)
                .HasColumnType("decimal(20, 10)");

            modelBuilder.Entity<Rate>()
                .HasIndex(r => new { r.CurrencyId, r.Date })
                .IsUnique();

            var currencySeedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "CurrencySeedData.json");
            var currenciesJson = File.ReadAllText(currencySeedDataPath);

            var currencyData = JsonConvert.DeserializeObject<Dictionary<string, string>>(currenciesJson);
            if (currencyData == null) {
                throw new InvalidOperationException("Failed to deserialize the currency seed data.");
            }

            var currenciesList = new List<Currency>();
            int currencyIdCounter = 1;
            foreach (var entry in currencyData) {
                currenciesList.Add(new Currency
                {
                    Id = currencyIdCounter++,
                    Symbol = entry.Key,
                    Name = entry.Value
                });
            }

            modelBuilder.Entity<Currency>().HasData(currenciesList);

            var rateSeedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "RateSeedData.json");
            var rateSeedData = File.ReadAllText(rateSeedDataPath);
            var ratesJson = JObject.Parse(rateSeedData)["rates"] as JObject;
            var rateDate = JObject.Parse(rateSeedData)["date"]?.Value<string>();
            if (string.IsNullOrEmpty(rateDate)) {
                throw new InvalidOperationException("Rate date is missing in the seed data.");
            }
            var parsedDate = DateTime.Parse(rateDate);

            var ratesList = new List<Rate>();
            int rateIdCounter = 1;
            if (ratesJson != null) {
                foreach (var rate in ratesJson) {
                    var currency = currenciesList.FirstOrDefault(c => c.Symbol == rate.Key);
                    if (currency != null) {
                        ratesList.Add(new Rate
                        {
                            Id = rateIdCounter++,
                            Value = rate.Value?.Type == JTokenType.Float ? rate.Value.Value<decimal>() : 0M,
                            Date = parsedDate.Date,
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