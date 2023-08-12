namespace ExchangeRateSharedLib.Models
{
    public class ExchangeRateResponse
    {
        public bool Success { get; set; }
        public long Timestamp { get; set; }
        public string Base { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}