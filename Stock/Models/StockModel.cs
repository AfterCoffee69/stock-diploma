namespace Stock.Models
{
    public class StockModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Addres { get; set; }

        public List<Product> Products { get; set; }
        public List<Employer> Employers { get; set; }
    }
}
