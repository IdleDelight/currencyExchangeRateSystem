namespace ExchangeRateDatabaseLib.Model
{
    public class Currency
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public List<Rate> Rates { get; set; }
    }

}
