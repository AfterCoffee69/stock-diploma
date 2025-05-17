using System.ComponentModel.DataAnnotations.Schema;

namespace Stock.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Amount { get; set; }

        public int CustomerId { get; set; }

        public int  DeliveryId { get; set; }
        public int ProductId {  get; set; }
        public Product Product { get; set; }
        public Delivery Delivery { get; set; }
        public Customer Customer { get; set; }
    }
}
