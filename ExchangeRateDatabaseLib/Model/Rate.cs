namespace ExchangeRateDatabaseLib.Model
{
    public class Rate
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string TimeStamp { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
