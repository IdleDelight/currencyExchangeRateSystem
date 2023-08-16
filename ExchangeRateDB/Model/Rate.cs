namespace ExchangeRateDB.Model
{
    public class Rate
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
