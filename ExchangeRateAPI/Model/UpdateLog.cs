namespace ExchangeRateAPI.Model
{
    public class UpdateLog
    {
        public int Id { get; set; }
        public string DbUpdateDate { get; set; } = string.Empty; // Change to DateTime, with '= DateTime.UtcNow;'
        public string ApiCallSuccess { get; set; } = string.Empty; // Change to bool, with '= false;'
        public string Message { get; set; } = string.Empty;
    }
}