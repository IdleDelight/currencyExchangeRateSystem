using ExchangeRateDatabaseLib.Model;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateDatabaseLib.Data
{
    public class ExchangeRateDbContext : DbContext
    {
        public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options) 
            : base(options) 
        { }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<UpdateLog> UpdateLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasMany (c => c.Rates)
                .WithOne (r => r.Currency)
                .HasForeignKey(r => r.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
