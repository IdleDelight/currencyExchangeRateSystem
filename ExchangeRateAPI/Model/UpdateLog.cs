namespace ExchangeRateAPI.Model
{
    public class UpdateLog
    {
        public int Id { get; set; }
        public string DbUpdateDate { get; set; }
        public string ApiCallSuccess { get; set; }
        public string Message { get; set; }
    }
}