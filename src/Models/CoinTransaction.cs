namespace src.Models
{
    public class CoinTransaction
    {
        public int Id { get; set; }
        public int CoinValue { get; set; }
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
