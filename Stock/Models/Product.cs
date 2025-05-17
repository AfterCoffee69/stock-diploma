namespace Stock.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int StockId { get; set; }


        public StockModel Stock { get; set; }
        public Supplier Supplier { get; set; }  
        public Category Category { get; set; }
        public List<Order> Orders { get; set; }
    }
}
