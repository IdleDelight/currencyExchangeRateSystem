using ExchangeRateDB.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            // Repeat similar steps for Rates if you have a separate JSON file for that

            modelBuilder.Entity<UpdateLog>().HasData(
                new UpdateLog { Id = 1, DbUpdateDate = DateTime.Now.ToString(), ApiCallSuccess = "NA", Message = "DB Created" }
            );
        }
    }
}