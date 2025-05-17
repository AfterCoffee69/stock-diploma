namespace Stock.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public Order Order { get; set; }
    }
}
