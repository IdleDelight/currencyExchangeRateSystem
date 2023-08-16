namespace ExchangeRateDB.Model
{
    public class UpdateLog
    {
        public int Id { get; set; }
        public DateTime DbUpdateDate { get; set; } =  DateTime.UtcNow;
        public bool ApiCallSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}