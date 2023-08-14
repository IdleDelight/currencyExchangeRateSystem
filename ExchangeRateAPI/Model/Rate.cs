namespace ExchangeRateAPI.Model
{
    public class Rate
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string Date { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
