namespace EbanxApi.Models
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public object Lock { get; } = new object();
    }
}