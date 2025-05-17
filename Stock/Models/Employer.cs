namespace Stock.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public int Number {  get; set; }
        public DateTime DateConnection { get; set; }
        public int StockId { get; set; }
        
        public StockModel Stock { get; set; }
    }
}
