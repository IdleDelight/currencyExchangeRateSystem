namespace ExchangeRateDB.Model
{
    public class Currency
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<Rate> Rates { get; set; } = new List<Rate>();
    }

}